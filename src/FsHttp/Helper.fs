module FsHttp.Helper

open System
open System.Text
open FsHttp.HelperInternal
open System

[<RequireQualifiedAccess>]
module Result =
    let getValueOrThrow ex (r: Result<'a, 'b>) =
        match r with
        | Ok value -> value
        | Error value -> raise (ex value)

[<RequireQualifiedAccess>]
module String =   
    let urlEncode (s: string) =
        System.Web.HttpUtility.UrlEncode(s)
    let toBase64 (s: string) =
        s
        |> base64Encoding.GetBytes
        |> Convert.ToBase64String
    let fromBase64 (s: string) =
        s
        |> Convert.FromBase64String
        |> base64Encoding.GetString
    let substring maxLength (s:string) =
        string(s.Substring(0, Math.Min(maxLength, s.Length)))

[<RequireQualifiedAccess>]
module Url =
    let combine (url1: string) (url2: string) =
        let del = '/'
        let sdel = string del
        let norm (s: string) = s.Trim().Replace(@"\", sdel)
        let delTrim = [| del |]
        let a = (norm url1).TrimEnd(delTrim)
        let b = (norm url2).TrimStart(delTrim).TrimEnd(delTrim)
        a + sdel + b


[<RequireQualifiedAccess>]
module Stream =
    open System.IO
    open System.Runtime.InteropServices
    
    // TODO: Inefficient
    type Utf8StringBufferingStream(baseStream: Stream, readBufferLimit: int option) =
        inherit Stream()
        let notSeekable() = failwith "Stream is not seekable."
        let notWritable() = failwith "Stream is not writable."
        let readBuffer = ResizeArray<byte>()
        override _.Flush() = baseStream.Flush()
        override _.Read(buffer, offset, count) =
            let readCount = baseStream.Read(buffer, offset, count)
            match readCount, readBufferLimit with
            | 0, _ -> ()
            | readCount, None -> readBuffer.AddRange(buffer[offset..readCount - 1])
            | readCount, Some limit ->
                let remaining = limit - readBuffer.Count
                let copyCount = min remaining readCount
                if copyCount > 0 then
                    readBuffer.AddRange(buffer[offset..copyCount - 1])
            readCount
        override _.Seek(_, _) = notSeekable()
        override _.SetLength(_) = notWritable()
        override _.Write(_, _, _) = notWritable()
        override _.CanRead with get() = true
        override _.CanSeek with get() = false
        override _.CanWrite with get() = false
        override _.Length with get() = baseStream.Length
        override _.Position with get() = baseStream.Position and set(_) = notSeekable()
        member _.GetUtf8String() =
#if NETSTANDARD2_0 || NETSTANDARD2_1
            let buffer = readBuffer |> Seq.toArray
#else
            let buffer = CollectionsMarshal.AsSpan(readBuffer)
#endif
            let s = Encoding.UTF8.GetString(buffer).AsSpan()
            if s.Length = 0 then
                s.ToString()
            else
                let s = if s[s.Length-1] |> Char.IsHighSurrogate then s.Slice(0, s.Length - 1) else s
                s.ToString()

    let readUtf8StringAsync maxUtf16CharCount (stream: Stream) =
        let sr = new StreamReader(stream, Encoding.UTF8)
        let sb = StringBuilder()
        let charBuffer = Array.zeroCreate<char>(50)
        let mutable lastCharIsHighSurrogate = false
        let rec read() =
            async {
                if sb.Length < maxUtf16CharCount then
                    let! readCount = sr.ReadAsync(charBuffer, 0, charBuffer.Length) |> Async.AwaitTask
                    let remaining = maxUtf16CharCount - sb.Length
                    let copyCount = min remaining readCount
                    if copyCount > 0 then
                        sb.Append(charBuffer, 0, copyCount) |> ignore
                        lastCharIsHighSurrogate <- charBuffer[copyCount-1] |> Char.IsHighSurrogate
                        do! read()
            }
        async {
            do! read()
            do
                sr.Dispose()
                stream.Dispose()
            return sb.ToString(0, if lastCharIsHighSurrogate then sb.Length - 1 else sb.Length)
        }

    let copyToCallbackAsync (target: Stream) callback (source: Stream) = async {
        let buffer = Array.create 1024 (byte 0)
        let logTimeSpan = TimeSpan.FromSeconds 1.5
        let mutable continueLooping = true
        let mutable overallBytesCount = 0
        let mutable lastNotificationTime = DateTime.Now
        while continueLooping do
            let! readCount = source.ReadAsync(buffer, 0, buffer.Length) |> Async.AwaitTask
            do target.Write(buffer, 0, readCount)
            do overallBytesCount <- overallBytesCount + readCount
            let now = DateTime.Now
            if (now - lastNotificationTime) > logTimeSpan then
                do callback overallBytesCount
                do lastNotificationTime <- now
            do continueLooping <- readCount > 0
        callback overallBytesCount
    }

    let copyToCallbackTAsync (target: Stream) callback (source: Stream) =
        copyToCallbackAsync target callback source |> Async.StartAsTask
        
    let copyToAsync target source = async {
        printfn "Download response received - starting download..."
        do! source |> copyToCallbackAsync target (fun read ->
            let mbRead = float read / 1024.0 / 1024.0
            printfn "%f MB" mbRead)
        printfn "Download finished."
    }

    let copyToTAsync target source =
        copyToAsync target source |> Async.StartAsTask

    let toStringUtf8Async source = async {
        use ms = new MemoryStream()
        do! source |> copyToAsync ms
        do ms.Position <- 0L
        use sr = new StreamReader(ms, Encoding.UTF8)
        return sr.ReadToEnd()
    }

    let toStringUtf8TAsync source =
        toStringUtf8Async source |> Async.StartAsTask

    let toBytesAsync source = async {
        use ms = new MemoryStream()
        do! source |> copyToAsync ms
        return ms.ToArray()
    }

    let toBytesTAsync source =
        toBytesAsync source |> Async.StartAsTask

    let saveFileAsync fileName source = async {
        printfn "Download response received (file: %s) - starting download..." fileName
        use fs = File.Open(fileName, FileMode.Create, FileAccess.Write)
        do! source |> copyToAsync fs
        printfn "Download finished."
    }

    let saveFileTAsync fileName source =
        saveFileAsync fileName source |> Async.StartAsTask

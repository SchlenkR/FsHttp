module FsHttp.Helper

open System
open System.Text

let base64Encoding = Encoding.GetEncoding("ISO-8859-1")

[<RequireQualifiedAccess>]
module String =

    let urlEncode (s: string) = System.Web.HttpUtility.UrlEncode(s)

    let toBase64 (s: string) =
        s
        |> base64Encoding.GetBytes
        |> Convert.ToBase64String

    let fromBase64 (s: string) =
        s
        |> Convert.FromBase64String
        |> base64Encoding.GetString

    let substring (s:string) maxLength = string(s.Substring(0, Math.Min(maxLength, s.Length)))

[<RequireQualifiedAccess>]
module Map =
    let union (m1: Map<'k, 'v>) (s: seq<'k * 'v>) =
        seq {
            yield! m1 |> Seq.map (fun kvp -> kvp.Key, kvp.Value)
            yield! s
        }
        |> Map.ofSeq

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
module HttpStatusCode =
    let show (this: System.Net.HttpStatusCode) = $"{int this} ({this})"

[<RequireQualifiedAccess>]
module Testing =
    let inline raiseExn (msg: string) =
        let otype =
            [
                "Xunit.Sdk.XunitException, xunit.assert"
                "NUnit.Framework.AssertionException, nunit.framework"
                "Expecto.AssertException, expecto"
            ]
            |> List.tryPick(System.Type.GetType >> Option.ofObj)
        match otype with
        | None -> failwith msg
        | Some t ->
            let ctor = t.GetConstructor [| typeof<string> |]
            ctor.Invoke [| msg |] :?> exn |> raise

module Result =
    let raiseOnError (r: Result<'a, 'b>) =
        match r with
        | Ok value -> value
        | Error value -> Testing.raiseExn $"{value}"

module Stream =
    open System.IO
    
    let copyToCallbackAsync (target: Stream) callback (source: Stream) = async {
        let buffer = Array.create 1024 (byte 0)
        
        let logTimeSpan = TimeSpan.FromSeconds 1.5
        let mutable continueLooping = true
        let mutable overallBytesCount = 0
        let mutable lastNotificationTime = DateTime.Now
        while continueLooping do
            let! readCount = source.ReadAsync(buffer, 0, buffer.Length) |> Async.AwaitTask
            do target.Write(buffer, 0, readCount)

            overallBytesCount <- overallBytesCount + readCount
                
            let now = DateTime.Now
            if (now - lastNotificationTime) > logTimeSpan then
                do callback overallBytesCount
                lastNotificationTime <- now
                    
            continueLooping <- readCount > 0
        callback overallBytesCount
    }
        
    let copyToAsync target source = async {
        printfn "Download response received - starting download..."
        do! source |> copyToCallbackAsync target (fun read ->
            let mbRead = float read / 1024.0 / 1024.0
            printfn "%f MB" mbRead)
        printfn "Download finished."
    }

    let toStringUtf8Async source = async {
        use ms = new MemoryStream()
        do! source |> copyToAsync ms
        do ms.Position <- 0L
        use sr = new StreamReader(ms, Encoding.UTF8)
        return sr.ReadToEnd()
    }

    let toBytesAsync source = async {
        use ms = new MemoryStream()
        do! source |> copyToAsync ms
        return ms.ToArray()
    }

    let saveFileAsync fileName source = async {
        printfn "Download response received (file: %s) - starting download..." fileName
        use fs = File.Open(fileName, FileMode.Create, FileAccess.Write)
        do! source |> copyToAsync fs
        printfn "Download finished."
    }

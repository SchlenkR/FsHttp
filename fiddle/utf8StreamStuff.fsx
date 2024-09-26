open System
open System.IO
open System.Text

let readUtf8StringAsync maxLen (stream: Stream) =
    async {
        use reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks = false, bufferSize = 1024, leaveOpen = true)
        let sb = StringBuilder()
        let mutable codePointsRead = 0
        let buffer = Array.zeroCreate<char> 1 // Buffer to read one character at a time
        while codePointsRead < maxLen && not reader.EndOfStream do
            // Read the next character asynchronously
            let! charsRead = reader.ReadAsync(buffer, 0, 1) |> Async.AwaitTask
            if charsRead > 0 then
                let c = buffer.[0]
                sb.Append(c) |> ignore
                // Check if the character is a high surrogate
                if Char.IsHighSurrogate(c) && not reader.EndOfStream then
                    // Read the low surrogate asynchronously and append it
                    let! nextCharsRead = reader.ReadAsync(buffer, 0, 1) |> Async.AwaitTask
                    if nextCharsRead > 0 then
                        let nextC = buffer.[0]
                        sb.Append(nextC) |> ignore
                // Increment the code point count
                codePointsRead <- codePointsRead + 1
        return sb.ToString()
    }

let text = "a😉b🙁🙂d"

let test len (expected: string) =
    let res =
        new MemoryStream(Encoding.UTF8.GetBytes(text))
        |> readUtf8StringAsync len
        |> Async.RunSynchronously
    let s1 = Encoding.UTF8.GetBytes res |> Array.toList
    let s2 = Encoding.UTF8.GetBytes expected |> Array.toList
    let res = (s1 = s2)
    if not res then
        printfn ""
        printfn "count = %d" len
        printfn "expected = %s" expected
        printfn ""
        printfn "Expected: %A" s2
        printfn ""
        printfn "Actual  : %A" s1
        printfn ""
        printfn " ----------------------------"

test 0 ""
test 1 "a"
test 2 "a😉"
test 3 "a😉b"
test 4 "a😉b🙁"
test 5 "a😉b🙁🙂"
test 6 "a😉b🙁🙂d"

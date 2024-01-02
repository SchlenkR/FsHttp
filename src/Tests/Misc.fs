module FsHttp.Tests.Misc

open System
open System.IO
open FsUnit
open FsHttp
open FsHttp.Tests
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.Response
open Suave.Writers


[<TestCase>]
let ``Custom HTTP method`` () =
    use server =
        ``method`` (HttpMethod.parse "FLY") >=> request (fun r -> OK "flying") |> serve

    http { Method "FLY" (url @"") }
    |> Request.send
    |> Response.toText
    |> should equal "flying"


[<TestCase>]
let ``Custom Header`` () =
    let customHeaderKey = "X-Custom-Value"

    use server =
        GET
        >=> request (fun r ->
            r.header customHeaderKey
            |> function
                | Choice1Of2 v -> v
                | Choice2Of2 e -> raise (assertionExn $"Failed {e}")
            |> OK
        )
        |> serve

    http {
        GET(url @"")
        header customHeaderKey "hello world"
    }
    |> Request.send
    |> Response.toText
    |> should equal "hello world"

[<TestCase>]
let ``Custom Headers`` () =
    let headersToString =
        List.sort
        >> List.map (fun (key, value) -> $"{key}={value}".ToLower())
        >> (fun h -> String.Join("&", h))

    let headerPrefix = "X-Custom-Value"
    let customHeaders = [ for i in 1..10 -> $"{headerPrefix}{i}", $"{i}" ]
    let expected = headersToString customHeaders

    use server =
        GET
        >=> request (fun r ->
            r.headers
            |> List.filter (fun (k, _) -> k.StartsWith(headerPrefix, StringComparison.OrdinalIgnoreCase))
            |> headersToString
            |> OK
        )
        |> serve

    http {
        GET(url @"")
        headers customHeaders
    }
    |> Request.send
    |> Response.toText
    |> should equal expected


[<TestCase>]
let ``Response Decompression`` () =
    // Why so many chars? Suave has a configured minimum size for compression of 500 bytes!
    let responseText =
        @"
        Hello World Hello World Hello World Hello World Hello World Hello World
        Hello World Hello World Hello World Hello World Hello World Hello World
        Hello World Hello World Hello World Hello World Hello World Hello World
        Hello World Hello World Hello World Hello World Hello World Hello World
        Hello World Hello World Hello World Hello World Hello World Hello World
        Hello World Hello World Hello World Hello World Hello World Hello World
        Hello World Hello World Hello World Hello World Hello World Hello World
        Hello World Hello World Hello World Hello World Hello World Hello World
        Hello World Hello World Hello World Hello World Hello World Hello World
        Hello World Hello World Hello World Hello World Hello World Hello World
    "

    use server =
        GET
        >=> request (fun r ->
            // setting the mime type to "text/html" will cause the response to be decompressed:
            // https://suave.io/files.html
            responseText |> OK >=> setMimeType "text/html"
        )
        |> serve

    let baseRequest =
        http {
            GET(url @"")
            AcceptEncoding "gzip, deflate"
        }

    // automatic response decompression (default)
    baseRequest |> Request.send |> Response.toText |> should equal responseText

    // manual decompression
    baseRequest { config_noDecompression }
    |> Request.send
    |> Response.toBytes
    |> fun responseContent ->
        use ms = new MemoryStream(responseContent)
        use gs = new Compression.GZipStream(ms, Compression.CompressionMode.Decompress)
        use sr = new StreamReader(gs, encoding = Text.Encoding.UTF8)
        sr.ReadToEnd()
    |> should equal responseText


//let [<TestCase>] ``Auto Redirects``() =
//    http {
//        GET (url @"")
//        config_transformHttpClientHandler (fun handler ->
//            handler.AllowAutoRedirect <- false
//            handler
//        )
//    }
//    |> Response.toText
//    |> should equal "hello world"



// TODO:

// let [<TestCase>] ``Http reauest message can be modified``() =
//     use server = GET >=> request (header "accept-language" >> OK) |> serve

//     let lang = "fr"
//     http {
//         GET (url @"")
//         transformHttpRequestMessage (fun httpRequestMessage ->
//             httpRequestMessage
//         )
//     }
//     |> toText
//     |> should equal lang

// TODO: Timeout
// TODO: ToFormattedText
// TODO: transformHttpRequestMessage
// TODO: transformHttpClient
// TODO: Cookie tests (test the overloads)

module FsHttp.Tests.Basic

open System
open System.Text
open System.Threading

open FsUnit
open FsHttp
open FsHttp.Tests.TestHelper
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful


[<TestCase>]
let ``Synchronous calls are invoked immediately`` () =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve

    get (url @"?test=Hallo")
    |> Request.send
    |> Response.toText
    |> should equal "test=Hallo"


[<TestCase>]
let ``Asynchronous calls are sent immediately`` () =

    let mutable time = DateTime.MaxValue

    use server =
        GET
        >=> request (fun r ->
            time <- DateTime.Now
            r.rawQuery |> OK
        )
        |> serve

    let req = get (url "?test=Hallo") |> Request.sendAsync

    Thread.Sleep 3000

    req |> Async.RunSynchronously |> Response.toText |> should equal "test=Hallo"

    (DateTime.Now - time > TimeSpan.FromSeconds 2.0) |> should equal true


[<TestCase>]
let ``Split URL are interpreted correctly`` () =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve

    http {
        GET(
            url
                @"
                    ?test=Hallo
                    &test2=Welt"
        )
    }
    |> Request.send
    |> Response.toText
    |> should equal "test=Hallo&test2=Welt"


[<TestCase>]
let ``Smoke test for a header`` () =
    use server = GET >=> request (header "accept-language" >> OK) |> serve

    let lang = "zh-Hans"

    http {
        GET(url @"")
        AcceptLanguage lang
    }
    |> Request.send
    |> Response.toText
    |> should equal lang

[<TestCase>]
let ``Smoke test for headers`` () =
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
            let headers =
                r.headers
                |> List.filter (fun (k, _) -> k.StartsWith(headerPrefix, StringComparison.OrdinalIgnoreCase))

            headersToString headers |> OK
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
let ``ContentType override`` () =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let contentType = "text/xxx"

    http {
        POST(url @"")
        body
        ContentType contentType
        text "hello world"
    }
    |> Request.send
    |> Response.toText
    |> should contain contentType


[<TestCase>]
let ``ContentType with encoding`` () =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let contentType = "text/xxx"
    let expectedContentTypeHeader = $"{contentType}; charset=utf-8"

    http {
        POST(url @"")
        body
        ContentType contentType Encoding.UTF8
        text "hello world"
    }
    |> Request.send
    |> Response.toText
    |> should contain expectedContentTypeHeader

module FsHttp.Tests.Body

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
let ``POST string data`` () =
    use server = POST >=> request (contentText >> OK) |> serve

    let data = "hello world"

    http {
        POST(url @"")
        body
        text data
    }
    |> Request.send
    |> Response.toText
    |> should equal data


[<TestCase>]
let ``POST binary data`` () =
    use server = POST >=> request (fun r -> r.rawForm |> Suave.Successful.ok) |> serve

    let data = [| 12uy; 22uy; 99uy |]

    http {
        POST(url @"")
        body
        binary data
    }
    |> Request.send
    |> Response.toBytes
    |> should equal data


[<TestCase>]
let ``POST Form url encoded data`` () =
    use server =
        POST >=> request (fun r -> (form "q1" r) + "_" + (form "q2" r) |> OK) |> serve

    http {
        POST(url @"")
        body
        formUrlEncoded [ "q1", "Query1"; "q2", "Query2" ]
    }
    |> Request.send
    |> Response.toText
    |> should equal ("Query1_Query2")


[<TestCase>]
let ``Specify content type explicitly`` () =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let contentType = "text/whatever"

    http {
        POST(url @"")
        body
        ContentType contentType
    }
    |> Request.send
    |> Response.toText
    |> should equal contentType


[<TestCase>]
let ``Default content type for JSON is specified correctly`` () =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    http {
        POST(url @"")
        body
        json " [] "
    }
    |> Request.send
    |> Response.toText
    |> should equal MimeTypes.applicationJson


[<TestCase>]
let ``Explicitly specified content type is dominant`` () =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let explicitContentType = "text/whatever"

    http {
        POST(url @"")
        body
        ContentType explicitContentType
        json " [] "
    }
    |> Request.send
    |> Response.toText
    |> should equal explicitContentType


[<TestCase>]
let ``Content length automatically set`` () =
    use server = POST >=> request (header "content-length" >> OK) |> serve

    let contentData = " [] "

    http {
        POST(url @"")
        body
        json contentData
    }
    |> Request.send
    |> Response.toText
    |> should equal (contentData.Length.ToString())

// TODO: Post single file
// TODO: POST stream

module FsHttp.Tests.Body

open FsUnit
open FsHttp
open FsHttp.DslCE
open FsHttp.Tests.TestHelper
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful


let [<TestCase>] ``POST string data``() =
    use server =
        POST
        >=> request (contentText >> OK)
        |> serve

    let data = "hello world"

    http {
        POST (url @"")
        body
        text data
    }
    |> Response.toText
    |> should equal data


let [<TestCase>] ``POST binary data``() =
    use server =
        POST 
        >=> request (fun r -> r.rawForm |> Suave.Successful.ok)
        |> serve

    let data = [| 12uy; 22uy; 99uy |]

    http {
        POST (url @"")
        body
        binary data
    }
    |> Response.toBytes
    |> should equal data


let [<TestCase>] ``POST Form url encoded data``() =
    use server =
        POST 
        >=> request (fun r -> (form "q1" r) + "_" + (form "q2" r) |> OK) 
        |> serve

    http {
        POST (url @"")
        body
        formUrlEncoded [
            "q1","Query1"
            "q2","Query2"
        ]
    }
    |> Response.toText
    |> should equal ("Query1_Query2")


let [<TestCase>] ``Specify content type explicitly``() =
    use server =
        POST
        >=> request (header "content-type" >> OK)
        |> serve

    let contentType = "text/whatever"
    
    http {
        POST (url @"")
        body
        ContentType contentType
    }
    |> Response.toText
    |> should equal contentType


let [<TestCase>] ``Default content type for JSON is specified correctly``() =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    http {
        POST (url @"")
        body
        json " [] "
    }
    |> Response.toText
    |> should equal MimeTypes.textJson


let [<TestCase>] ``Explicitly specified content type is dominant``() =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let explicitContentType = "text/whatever"

    http {
        POST (url @"")
        body
        ContentType explicitContentType
        json " [] "
    }
    |> Response.toText
    |> should equal explicitContentType


let [<TestCase>] ``Content length automatically set``() =
    use server = POST >=> request (header "content-length" >> OK) |> serve

    let contentData = " [] "
    http {
        POST (url @"")
        body
        json contentData
    }
    |> Response.toText
    |> should equal (contentData.Length.ToString())

// TODO: Post single file
// TODO: POST stream


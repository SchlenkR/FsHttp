module FsHttp.Tests.``Content Type``

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


let [<TestCase>] ``Specify content type explicitly``() =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let contentType = "application/whatever"
    
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
    |> should equal MimeTypes.applicationJson

let [<TestCase>] ``Explicitly specified content type is dominant``() =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let explicitContentType = "application/whatever"

    http {
        POST (url @"")
        body
        ContentType explicitContentType
        json " [] "
    }
    |> Response.toText
    |> should equal explicitContentType

let [<TestCase>] ``Explicitly specified content type part is dominant``() =
    
    let explicitContentType1 = "application/whatever1"
    let explicitContentType2 = "application/whatever2"

    use server =
        POST 
        >=> request (fun r ->
            r.files
            |> List.map (fun f -> f.mimeType)
            |> String.concat ","
            |> OK)
        |> serve

    http {
        POST (url @"")
        multipart

        ContentTypeForPart explicitContentType1
        filePart "Resources/uploadFile.txt"
        
        ContentTypeForPart explicitContentType2
        filePart "Resources/uploadFile2.txt"
    }
    |> Response.toText
    |> should equal (explicitContentType1 + "," + explicitContentType2)

module FsHttp.Tests.Post

open System.IO

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
        >=> request (text >> OK)
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

let [<TestCase>] ``POST Multipart form data``() =
    
    let joinLines =  String.concat "\n"
    
    use server =
        POST 
        >=> request (fun r ->
            let fileContents =
                r.files
                |> List.map (fun f -> File.ReadAllText f.tempFilePath)
                |> joinLines
            let multipartContents =
                r.multiPartFields
                |> List.map (fun (k,v) -> k + "=" + v)
                |> joinLines
            [ fileContents; multipartContents ] |> joinLines |> OK)
        |> serve

    http {
        POST (url @"")
        multipart
        filePart "Resources/uploadFile.txt"
        filePart "Resources/uploadFile2.txt"
        stringPart "hurz1" "das"
        stringPart "hurz2" "Lamm"
        stringPart "hurz3" "schrie"
    }
    |> Response.toText
    |> should equal (joinLines [
        "I'm a chicken and I can fly!"
        "Lemonade was a popular drink, and it still is."
        "hurz1=das"
        "hurz2=Lamm"
        "hurz3=schrie"
    ])

// TODO: Post single file

// TODO: POST stream
// TODO: POST multipart


﻿module FsHttp.Tests.Multipart

open System.IO

open FsUnit
open FsHttp
open FsHttp.Tests.Server
open FsHttp.Tests.TestHelper

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful


[<TestCase>]
let ``POST Multipart form data`` () =
    use server =
        POST
        >=> request (fun r ->
            let fileContents =
                r.files |> List.map (fun f -> File.ReadAllText f.tempFilePath) |> joinLines

            let multipartContents =
                r.multiPartFields |> List.map (fun (k, v) -> k + "=" + v) |> joinLines

            [ fileContents; multipartContents ] |> joinLines |> OK
        )
        |> serve

    http {
        POST(url @"")
        multipart
        filePart "Resources/uploadFile.txt"
        filePart "Resources/uploadFile2.txt"
        stringPart "hurz1" "das"
        stringPart "hurz2" "Lamm"
        stringPart "hurz3" "schrie"
    }
    |> Request.send
    |> Response.toText
    |> should
        equal
        (joinLines [
            "I'm a chicken and I can fly!"
            "Lemonade was a popular drink, and it still is."
            "hurz1=das"
            "hurz2=Lamm"
            "hurz3=schrie"
        ])


[<TestCase>]
let ``POST Multipart bytearray with optional filename`` () =
    let fileName1 = "fileName1"
    let fileName2 = "fileName2"
    let fileName3 = "fileName3"

    use server =
        POST
        >=> request (fun r ->
            let fileNames = r.files |> List.map (fun f -> f.fileName) |> joinLines

            fileNames |> OK
        )
        |> serve

    http {
        POST(url @"")
        multipart

        ContentTypeForPart "application/json"
        byteArrayPart "theFieldName" [| byte 0xff |] fileName1

        ContentTypeForPart "application/json"
        byteArrayPart "theFieldName" [| byte 0xff |] fileName2

        ContentTypeForPart "application/json"
        byteArrayPart "theFieldName" [| byte 0xff |] fileName3

        ContentTypeForPart "application/json"
        byteArrayPart "theFieldName" [| byte 0xff |]
    }
    |> Request.send
    |> Response.toText
    |> should equal (joinLines [ fileName1; fileName2; fileName3 ])


[<TestCase>]
let ``Explicitly specified content type part is dominant`` () =

    let explicitContentType1 = "text/whatever1"
    let explicitContentType2 = "text/whatever2"

    use server =
        POST
        >=> request (fun r -> r.files |> List.map (fun f -> f.mimeType) |> String.concat "," |> OK)
        |> serve

    http {
        POST(url @"")
        multipart

        ContentTypeForPart explicitContentType1
        filePart "Resources/uploadFile.txt"

        ContentTypeForPart explicitContentType2
        filePart "Resources/uploadFile2.txt"
    }
    |> Request.send
    |> Response.toText
    |> should equal (explicitContentType1 + "," + explicitContentType2)

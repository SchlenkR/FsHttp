module FsHttp.Tests.Multipart

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
        textPart "das" "hurz1"
        textPart "Lamm" "hurz2"
        textPart "schrie" "hurz3"
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

        filePart "Resources/uploadFile.txt"
        ContentType explicitContentType1

        filePart "Resources/uploadFile2.txt"
        ContentType explicitContentType2
    }
    |> Request.send
    |> Response.toText
    |> should equal (explicitContentType1 + "," + explicitContentType2)


[<TestCase>]
let ``POST Multipart part binary with optional filename`` () =
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

        binaryPart [| byte 0xff |] "photo" fileName1
        ContentType "image/jpeg"

        binaryPart [| byte 0xff |] "photo" fileName2
        ContentType "image/jpeg"

        binaryPart [| byte 0xff |] "photo" fileName3
        ContentType "image/jpeg"

        binaryPart [| byte 0xff |] "photo"
        ContentType "image/jpeg"
    }
    |> Request.send
    |> Response.toText
    |> should equal (joinLines [ fileName1; fileName2; fileName3 ])


[<TestCase>]
let ``POST Multipart textPart with optional filename`` () =
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

        textPart "the_value" "the_name" fileName1
        ContentType "application/json"

        filePart "Resources/uploadFile.txt" "theName" fileName2
        ContentType "application/json"

        binaryPart [| byte 0xff |] "theName" fileName3
        ContentType "application/json"
    }
    |> Request.send
    |> Response.toText
    |> should equal (joinLines [ fileName1; fileName2; fileName3 ])

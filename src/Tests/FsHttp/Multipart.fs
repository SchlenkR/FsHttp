module FsHttp.Tests.Multipart

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

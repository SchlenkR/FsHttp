module FsHttp.Tests.Body

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

// TODO: Post single file
// TODO: POST stream


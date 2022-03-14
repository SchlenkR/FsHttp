module FsHttp.Tests.Printing

open FsUnit
open FsHttp
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful

let [<TestCase>] ``Print Config on Request``() =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve

    let resonseLines =
        http {
            GET (url @"?test=Hallo")
            print_withResponseBodyLength 3
        }
        |> Request.send
        |> Response.print
        |> String.replace "\r" ""
        |> String.split '\n'
    printfn "%A" resonseLines

    resonseLines
    |> List.skipWhile (fun x -> x <> "RESPONSE")
    |> List.skip 1
    |> List.skipWhile (fun x -> x <> "===content===")
    |> List.skip 1
    |> List.head
    |> should equal "tes"
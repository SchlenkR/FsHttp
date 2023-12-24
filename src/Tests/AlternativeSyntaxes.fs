module RestInPeace.Tests.``Alternative Syntaxes``

open FsUnit
open RestInPeace
open RestInPeace.Tests.Server

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful

[<TestCase>]
let ``Shortcut for GET works`` () =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve

    get (url @"?test=Hallo")
    |> Request.send
    |> Response.toText
    |> shouldEqual "test=Hallo"

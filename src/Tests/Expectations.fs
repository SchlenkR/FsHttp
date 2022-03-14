module FsHttp.Tests.Expectations

open NUnit.Framework

open FsHttp
open FsHttp.Tests.Server

open Suave.ServerErrors
open Suave.Operators
open Suave.Filters

// TODO: exactMatch = true

let [<TestCase>] ``Expect status code``() =
    use server = GET >=> BAD_GATEWAY "" |> serve

    http { GET (url @"") }
    |> Request.send
    |> Response.assertHttpStatusCode System.Net.HttpStatusCode.BadGateway
    |> ignore

    Assert.Throws<StatusCodeExpectedxception>(fun() ->
        http { GET (url @"") }
        |> Request.send
        |> Response.assertHttpStatusCode System.Net.HttpStatusCode.Ambiguous
        |> ignore
    )
    |> ignore

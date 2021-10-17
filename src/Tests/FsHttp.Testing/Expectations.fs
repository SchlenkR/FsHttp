module FsHttp.Testing.Tests.Expectations

open FsHttp.Testing
open NUnit.Framework

open FsHttp.DslCE
open FsHttp.Tests.Server

open Suave.ServerErrors
open Suave.Operators
open Suave.Filters

// TODO: exactMatch = true

let [<TestCase>] ``Expect status code``() =
    use server = GET >=> BAD_GATEWAY "" |> serve

    http { GET (url @"") }
    |> statusCodeShouldBe System.Net.HttpStatusCode.BadGateway
    |> ignore

    Assert.Throws<AssertionException>(fun() ->
        http { GET (url @"") }
        |> statusCodeShouldBe System.Net.HttpStatusCode.Ambiguous
        |> ignore
    )
    |> ignore

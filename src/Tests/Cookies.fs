module FsHttp.Tests.Cookies

open FsUnit
open FsHttp
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Cookie
open Suave.Operators
open Suave.Filters
open Suave.Successful


[<TestCase>]
let ``Cookies can be sent`` () =
    use server =
        GET
        >=> request (fun r -> r.cookies |> Map.find "test" |> (fun httpCookie -> httpCookie.value) |> OK)
        |> serve

    http {
        GET(url @"")
        Cookie "test" "hello world"
    }
    |> Request.send
    |> Response.toText
    |> should equal "hello world"

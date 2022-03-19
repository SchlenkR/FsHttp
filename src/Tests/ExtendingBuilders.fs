module FsHttp.Tests.``Extending Builders``

open FsUnit
open FsHttp
open FsHttp.Tests.TestHelper
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful

let superBodyContentType = "text/superString"

type IRequestContext<'self> with
    [<CustomOperation("superBody")>]
    member this.SuperBody(context: IRequestContext<BodyContext>, csvContent: string) =
        FsHttp.Dsl.Body.content superBodyContentType (StringContent csvContent) context.Self


let [<TestCase>] ``Extending builder with custom content``() =
    
    let dummyContent = "Hello"

    use server =
        POST
        >=> request (
            fun r ->
                let header = header "content-type" r
                let content = contentText r
                $"{header} - {content}"
            >> OK)
        |> serve
    
    http {
        POST (url @"")
        body
        superBody dummyContent
    }
    |> Request.send
    |> Response.toText
    |> should equal $"{superBodyContentType} - {dummyContent}"

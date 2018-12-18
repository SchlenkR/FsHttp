

module ``Integration tests for FsHttp``

open FsUnit
open FsHttp
open NUnit.Framework
open Server
open Suave
open Suave.Filters
open Suave.Utils.Collections
open Suave.Successful


[<AutoOpen>]
module Helper =

    let keyNotFoundString = "KEY_NOT_FOUND"
    let query key (r: HttpRequest) = defaultArg (Option.ofChoice (r.query ^^ key)) keyNotFoundString
    let header key (r: HttpRequest) = defaultArg (Option.ofChoice (r.header key)) keyNotFoundString
    let form key (r: HttpRequest) = defaultArg (Option.ofChoice (r.form ^^ key)) keyNotFoundString

    let httpGetRoute = 
        {
            method = GET;
            route = "/";
            handler = (fun r -> "" |> OK)
        }

    let httpPostRoute = 
        {
            method = POST;
            route = "/";
            handler = (fun r -> "" |> OK)
        }

    let httpGet handler =
        [
            { httpGetRoute with handler = handler }
        ]

    let httpPost handler =
        [
            { httpPostRoute with handler = handler }
        ]

    let httpWithOk handler = handler >> OK
    let httpGetWithOk handler = httpWithOk handler |> httpGet
    let httpPostWithOk handler = httpWithOk handler |> httpPost


[<TestCase>]
let ``Synchronous GET call is invoked immediately``() =
    use server = (fun r -> r.rawQuery) |> httpGetWithOk |> serve

    http { GET (url @"?test=Hallo") }
    |> toText
    |> should equal "test=Hallo"

[<TestCase>]
let ``Split URL are interpreted correctly``() =
    use server = (fun r -> r.rawQuery) |> httpGetWithOk |> serve

    http { GET (url @"
                    ?test=Hallo
                    &test2=Welt")
    }
    |> toText
    |> should equal "test=Hallo&test2=Welt"

[<TestCase>]
let ``Smoke test for a header``() =
    use server = (fun r -> r |> header "accept-language") |> httpGetWithOk |> serve

    let lang = "zh-Hans"
    
    http {
        GET (url @"")
        AcceptLanguage lang
    }
    |> toText
    |> should equal lang

[<TestCase>]
let ``ContentType override``() =
    use server = (fun r -> r |> header "content-type") |> httpPostWithOk |> serve

    let contentType = "application/xxx"

    http {
        POST (url @"")
        body
        ContentType contentType
        text "hello world"
    }
    |> toText
    |> should contain contentType

[<TestCase>]
let ``Multiline urls``() =
    use server = (fun r -> (query "q1" r) + "_" + (query "q2" r)) |> httpGetWithOk |> serve

    http {
        GET (url @"
                    ?q1=Query1
                    &q2=Query2")
    }
    |> toText
    |> should equal "Query1_Query2"

[<TestCase>]
let ``Comments in urls are discarded``() =
    use server = (fun r -> (query "q1" r) + "_" + (query "q2" r) + "_" + (query "q3" r)) |> httpGetWithOk |> serve

    http {
        GET (url @"
                    ?q1=Query1
                    //&q2=Query2
                    &q3=Query3")
    }
    |> toText
    |> should equal ("Query1_" + keyNotFoundString + "_Query3")

[<TestCase>]
let ``Form url encoded POSTs``() =
    use server = (fun r -> (form "q1" r) + "_" + (form "q2" r)) |> httpPostWithOk |> serve

    http {
        POST (url @"")
        body
        formUrlEncoded [
            "q1","Query1"
            "q2","Query2"
        ]
    }
    |> toText
    |> should equal ("Query1_Query2")

[<TestCase>]
let ``Expect status code``() =
    use server = (fun r -> Suave.ServerErrors.BAD_GATEWAY "") |> httpGet |> serve

    http { GET (url @"") }
    |> shouldHaveCode System.Net.HttpStatusCode.BadGateway
    |> ignore

    Assert.Throws<AssertionException>(fun() ->
        http { GET (url @"") }
        |> shouldHaveCode System.Net.HttpStatusCode.Ambiguous
        |> ignore
    )
    |> ignore

[<TestCase>]
let ``Specify content type explicitly``() =
    use server = (fun r -> r |> header "content-type") |> httpPostWithOk |> serve

    let contentType = "application/whatever"
    
    http {
        POST (url @"")
        body
        ContentType contentType
    }
    |> toText
    |> should contain contentType

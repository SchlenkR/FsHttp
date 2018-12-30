
#if INTERACTIVE
#r @"../../packages/fsharp.data/lib/net45/FSharp.Data.dll"
#r @"../../packages/NUnit/lib/netstandard2.0/nunit.framework.dll"
#r @"../../packages/fsunit/lib/netstandard2.0/FsUnit.NUnit.dll"
#r @"../../packages/Suave/lib/netstandard2.0\Suave.dll"
#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"
#load @"./Server.fs"
#else
module ``Integration tests for FsHttp``
#endif

open FsUnit
open FsHttp
open NUnit.Framework
open Server
open Suave
open Suave.ServerErrors
open Suave.Operators
open Suave.Filters
open Suave.Utils.Collections
open Suave.Successful

[<AutoOpen>]
module Helper =

    let keyNotFoundString = "KEY_NOT_FOUND"
    let query key (r: HttpRequest) = defaultArg (Option.ofChoice (r.query ^^ key)) keyNotFoundString
    let header key (r: HttpRequest) = defaultArg (Option.ofChoice (r.header key)) keyNotFoundString
    let form key (r: HttpRequest) = defaultArg (Option.ofChoice (r.form ^^ key)) keyNotFoundString


[<TestCase>]
let ``Synchronous GET call is invoked immediately``() =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve

    http { GET (url @"?test=Hallo") }
    |> toText
    |> should equal "test=Hallo"

[<TestCase>]
let ``Split URL are interpreted correctly``() =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve

    http { GET (url @"
                    ?test=Hallo
                    &test2=Welt")
    }
    |> toText
    |> should equal "test=Hallo&test2=Welt"

[<TestCase>]
let ``Smoke test for a header``() =
    use server = GET >=> request (header "accept-language" >> OK) |> serve

    let lang = "zh-Hans"
    
    http {
        GET (url @"")
        AcceptLanguage lang
    }
    |> toText
    |> should equal lang

[<TestCase>]
let ``ContentType override``() =
    use server = POST >=> request (header "content-type" >> OK) |> serve

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
    use server = 
        GET
        >=> request (fun r -> (query "q1" r) + "_" + (query "q2" r) |> OK)
        |> serve

    http {
        GET (url @"
                    ?q1=Query1
                    &q2=Query2")
    }
    |> toText
    |> should equal "Query1_Query2"

[<TestCase>]
let ``Comments in urls are discarded``() =
    use server =
        GET 
        >=> request (fun r -> (query "q1" r) + "_" + (query "q2" r) + "_" + (query "q3" r) |> OK)
        |> serve

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
    |> toText
    |> should equal ("Query1_Query2")

[<TestCase>]
let ``Expect status code``() =
    use server = GET >=> BAD_GATEWAY "" |> serve

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
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let contentType = "application/whatever"
    
    http {
        POST (url @"")
        body
        ContentType contentType
    }
    |> toText
    |> should contain contentType

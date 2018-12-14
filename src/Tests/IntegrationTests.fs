
#if INTERACTIVE
#r @"..\packages\NUnit\lib\netstandard2.0\NUnit.Framework.dll"
#r @"..\packages\FsUnit\lib\netstandard2.0\FsUnit.NUnit.dll"
#r @"..\packages\FSharp.Data\lib\netstandard2.0\FSharp.Data.dll"
#r @"..\packages\Suave\lib\netstandard2.0\Suave.dll"
#r @"..\FsHttp\bin\Debug\netstandard2.0\FsHttp.dll"
#load "Server.fs"
#else
module ``Integration tests for FsHttp``
#endif


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
    
    //use server = query "test" |> testGet |> serve
    use server = (fun r -> r.rawQuery) |> httpGetWithOk |> serve

    http { GET (url @"?test=Hallo")
    }
    |> toText
    |> should equal "test=Hallo"

[<TestCase>]
let ``Split URL are interpreted correctly``() =

    //use server = query "test" |> testGet |> serve
    use server = (fun r -> r.rawQuery) |> httpGetWithOk |> serve

    http { GET (url @"
                    ?test=Hallo
                    &test2=Welt")
    }
    |> toText
    |> should equal "test=Hallo&test2=Welt"

[<TestCase>]
let ``Smoke test for a header``() =

    //use server = query "test" |> testGet |> serve
    use server = (fun r -> r |> header "accept-language") |> httpGetWithOk |> serve

    let lang = "zh-Hans"
    
    http {  GET (url @"")
            AcceptLanguage lang
    }
    |> toText
    |> should equal lang

[<TestCase>]
let ``Expect status code``() =

    //use server = query "test" |> testGet |> serve
    use server = (fun r -> Suave.ServerErrors.BAD_GATEWAY "") |> httpGet |> serve

    http {  GET (url @"")
    }
    |> shouldHaveCode System.Net.HttpStatusCode.BadGateway
    |> ignore

    Assert.Throws<AssertionException>(fun() ->
        http {  GET (url @"")
        }
        |> shouldHaveCode System.Net.HttpStatusCode.Ambiguous
        |> ignore
    )
    |> ignore

[<TestCase>]
let ``Specify content type explicitly``() =

    //use server = query "test" |> testGet |> serve
    use server = (fun r -> r |> header "content-type") |> httpPostWithOk |> serve

    let contentType = "application/whatever"
    
    http {  POST (url @"")
            body
            ContentType contentType
    }
    |> toText
    |> should contain contentType

module FsHttp.Tests.Cookies

open System
open FsUnit
open FsHttp
open FsHttp.Tests
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Cookie
open Suave.Operators
open Suave.Filters
open Suave.Successful


let [<TestCase>] ``Cookies can be sent``() =
    use server =
        GET
        >=> request (fun r ->
            r.cookies
            |> Map.find "test"
            |> fun httpCookie -> httpCookie.value
            |> OK)
        |> serve

    http {
        GET (url @"")
        Cookie "test" "hello world"
    }
    |> Request.send
    |> Response.toText
    |> should equal "hello world"


let [<TestCase>] ``Custom HTTP method``() =
    use server =
        ``method`` (HttpMethod.parse "FLY")
        >=> request (fun r -> OK "flying")
        |> serve

    http {
        Method "FLY" (url @"")
    }
    |> Request.send
    |> Response.toText
    |> should equal "flying"


let [<TestCase>] ``Custom Headers``() =
    let customHeaderKey = "X-Custom-Value"

    use server =
        GET
        >=> request (fun r ->
            r.header customHeaderKey
            |> function 
                | Choice1Of2 v -> v 
                | Choice2Of2 e -> raise (TestHelper.raiseExn $"Failed {e}")
            |> OK)
        |> serve

    http {
        GET (url @"")
        header customHeaderKey "hello world"
    }
    |> Request.send
    |> Response.toText
    |> should equal "hello world"
    
let [<TestCase>] ``Custom Headers``() =
    let headersToString  =
        
        List.sort
        >> List.map (fun (key, value) -> $"{key}={value}".ToLower())
        >> (fun h -> String.Join("&", h))
    
    let headerPrefix = "X-Custom-Value"
    let customHeaders = [for i in 1..10 -> $"{headerPrefix}{i}", $"{i}"]
    let expected = headersToString customHeaders

    use server =
        GET
        >=> request (fun r ->
            let headers = r.headers |> List.filter (fun (k, _) -> k.StartsWith(headerPrefix, StringComparison.InvariantCultureIgnoreCase))
            headersToString headers
            |> OK)
        |> serve

    http {
        GET (url @"")
        headers customHeaders
    }
    |> Request.send
    |> Response.toText
    |> should equal expected    

//let [<TestCase>] ``Auto Redirects``() =
//    http {
//        GET (url @"")
//        config_transformHttpClientHandler (fun handler ->
//            handler.AllowAutoRedirect <- false
//            handler
//        )
//    }
//    |> Response.toText
//    |> should equal "hello world"
    


// TODO: 

// let [<TestCase>] ``Http reauest message can be modified``() =
//     use server = GET >=> request (header "accept-language" >> OK) |> serve
    
//     let lang = "fr"
//     http {
//         GET (url @"")
//         transformHttpRequestMessage (fun httpRequestMessage ->
//             httpRequestMessage
//         )
//     }
//     |> toText
//     |> should equal lang

// TODO: Timeout
// TODO: ToFormattedText
// TODO: transformHttpRequestMessage
// TODO: transformHttpClient
// TODO: Cookie tests (test the overloads)


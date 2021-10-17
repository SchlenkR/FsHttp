module FsHttp.Tests.``Builders and Signatures``

open FsHttp
open FsHttp.DslCE

open NUnit.Framework

    
let [<TestCase>] ``httpLazy and invocation signatures are correct``() =
    let request : LazyHttpBuilder<HeaderContext> =
        httpLazy {
            GET "https://www.wikipedia.de"
        }
    
    let (response: Response) = request |> Request.send
    let (asyncResponse: Async<Response>) = request |> Request.sendAsync
    
    ()
        
let [<TestCase>] ``httpMsg and invocation signatures are correct``() =
    let request : System.Net.Http.HttpRequestMessage = 
        httpMsg {
            GET "https://www.wikipedia.de"
        }
    
    ()
    

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


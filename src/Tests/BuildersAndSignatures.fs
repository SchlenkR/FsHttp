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

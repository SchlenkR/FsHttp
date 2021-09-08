module ``Unit tests for FsHttp``

open FsHttp
open FsHttp.DslCE
open NUnit.Framework

[<TestCase>]
let ``httpLazy and invocation signatures are correct``() =
    let request : LazyHttpBuilder<HeaderContext> =
        httpLazy {
            GET "http://www.google.de"
        }

    let (response: Response) = request |> Request.send
    let (asyncResponse: Async<Response>) = request |> Request.sendAsync

    ()
    
[<TestCase>]
let ``httpMsg and invocation signatures are correct``() =
    let request : System.Net.Http.HttpRequestMessage = 
        httpMsg {
            GET "http://www.google.de"
        }

    ()

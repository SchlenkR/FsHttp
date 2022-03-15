module FsHttp.Tests.DotNetHttp

open System.Net.Http
open System.Threading

open FsUnit
open FsHttp
open FsHttp.Tests.Server

open NUnit.Framework

open Suave.Operators
open Suave.Filters
open Suave.Successful


let [<TestCase>] ``Inject custom HttpClient`` () =
    let executedFlag = "executed"

    use server = GET >=> OK executedFlag |> serve

    let mutable intercepted = false
    
    let interceptor =
        { new DelegatingHandler(InnerHandler = new HttpClientHandler()) with
            member _.SendAsync(request: HttpRequestMessage, cancellationToken: CancellationToken) =
                intercepted <- true
                base.SendAsync(request, cancellationToken) }
    let httpClient = new HttpClient(interceptor)

    intercepted |> should equal false

    http {
        config_setHttpClient httpClient
        GET (url "")
    }
    |> Request.send
    |> Response.toText
    |> should equal executedFlag
    
    intercepted |> should equal true
   

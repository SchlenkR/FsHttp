module FsHttp.Tests.DotNetHttp

open System.Net.Http
open System.Threading

open FsUnit
open FsHttp
open FsHttp.DslCE
open FsHttp.Tests.Server

open NUnit.Framework

open Suave.Operators
open Suave.Filters
open Suave.Successful


let [<TestCase>] ``Inject custom HttpClient`` () =
    use server = GET >=> OK "" |> serve

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
        GET "http://google.com"
    }
    |> Request.send
    |> ignore
    
    intercepted |> should equal true
   

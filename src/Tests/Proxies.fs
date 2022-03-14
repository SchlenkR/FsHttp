module FsHttp.Tests.Proxies

open System
open System.Net

open FsUnit
open FsHttp
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful
open Suave.Response
open Suave.Writers


let [<TestCase>] ``Proxy usage works`` () =
    use server = GET >=> OK "proxified" |> serve

    http {
        GET "http://google.com"
        config_proxy (url "")
    }
    |> Request.send
    |> Response.toText
    |> should equal "proxified"

let [<TestCase>] ``Proxy usage with credentials works`` () =
    use server =
        GET >=> request (fun r ->
            printfn "Headers: %A" r.headers
            
            match r.header "Proxy-Authorization" with
            | Choice1Of2 cred -> cred |> OK
            | _ ->
                response HTTP_407 (Text.Encoding.UTF8.GetBytes "No credentials")
                >=> setHeader "Proxy-Authenticate" "Basic")
        |> serve
    let credentials = NetworkCredential("test", "password")

    http {
        GET "http://google.com"
        config_proxyWithCredentials (url "") credentials
    }
    |> Request.send
    |> Response.toText
    |> should equal ("Basic " + ("test:password" |> Text.Encoding.UTF8.GetBytes |> Convert.ToBase64String))


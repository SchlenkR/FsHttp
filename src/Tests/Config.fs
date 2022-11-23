module FsHttp.Tests.Config

open System
open System.Text
open System.Threading
open System.Threading.Tasks

open FsUnit
open FsHttp
open FsHttp.Tests.TestHelper
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful

open NUnit.Framework
open System.Net.Http

let timeoutEquals (config: Config) totalSeconds =
    config.timeout
    |> Option.map (fun x -> x.TotalSeconds)
    |> should equal (Some totalSeconds)

let [<TestCase>] ``Global config snapshot is used in moment of request creation`` () =

    let setTimeout t =
        GlobalConfig.defaults
        |> Config.timeoutInSeconds t
        |> GlobalConfig.set
    
    let t1 = 11.5
    let t2 = 22.5

    do GlobalConfig.defaults.Config.timeout |> should not' (equal (Some t1))
    do GlobalConfig.defaults.Config.timeout |> should not' (equal (Some t2))

    setTimeout t1

    let r1 = http {
        GET (url @"")
    }

    setTimeout t2

    // still t1
    do timeoutEquals r1.config t1
    
    let r2 = http {
        GET (url @"")
    }

    do timeoutEquals r2.config t2

let private serverWithRequestDuration requestTime =
    GET
    >=> request (fun r ->
        Thread.Sleep (TimeSpan.FromSeconds requestTime)
        "" |> OK)
    |> serve


let sendRequestWithTimeout timeout =
    fun () ->
        let req = get (url "")
        let req =
            match timeout with
            | Some timeout -> 
                req {
                    config_timeoutInSeconds timeout
                }
            | None -> req
        req {
            config_transformHttpClient (fun (client : HttpClient) ->
                printfn "TIMEOUT: %A" client.Timeout
                client)
        }
        |> Request.send
        |> ignore


let [<TestCase>] ``Timeout config per request - success expected``() =
    use server = serverWithRequestDuration 1.0

    (sendRequestWithTimeout (Some 20.0))()


let [<TestCase>] ``Timeout config per request - timeout expected``() =
    use server = serverWithRequestDuration 10.0

    sendRequestWithTimeout (Some 1.0)
    |> should throw typeof<TaskCanceledException>


let [<TestCase>] ``Timeout config global - success expected``() =
    use server = serverWithRequestDuration 1.0

    GlobalConfig.defaults
    |> Config.timeoutInSeconds 20.0
    |> GlobalConfig.set

    (sendRequestWithTimeout None)()


let [<TestCase>] ``Timeout config global - timeout expected``() =
    use server = serverWithRequestDuration 10.0

    GlobalConfig.defaults
    |> Config.timeoutInSeconds 20.0
    |> GlobalConfig.set

    (sendRequestWithTimeout None)()

    GlobalConfig.defaults
    |> Config.timeoutInSeconds 1.0
    |> GlobalConfig.set

    sendRequestWithTimeout None
    |> should throw typeof<TaskCanceledException>
    

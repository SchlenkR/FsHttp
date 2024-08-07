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

[<TestCase>]
let ``Global config snapshot is used in moment of request creation`` () =

    let setTimeout t = GlobalConfig.defaults |> Config.timeoutInSeconds t |> GlobalConfig.set

    let t1 = 11.5
    let t2 = 22.5

    do GlobalConfig.defaults.Config.timeout |> should not' (equal (Some t1))
    do GlobalConfig.defaults.Config.timeout |> should not' (equal (Some t2))

    setTimeout t1

    let r1 = http { GET(url @"") }

    setTimeout t2

    // still t1
    do timeoutEquals r1.config t1

    let r2 = http { GET(url @"") }

    do timeoutEquals r2.config t2

let private serverWithRequestDuration (requestTime: TimeSpan) =
    GET
    >=> request (fun r ->
        Thread.Sleep requestTime
        "" |> OK
    )
    |> serve


let sendRequestWithTimeout timeout =
    fun () ->
        let req = get (url "")

        let req =
            match timeout with
            | Some timeout -> req { config_timeoutInSeconds timeout }
            | None -> req

        req {
            config_transformHttpClient (fun (client: HttpClient) ->
                printfn "TIMEOUT: %A" client.Timeout
                client
            )
        }
        |> Request.send
        |> ignore


[<TestCase>]
let ``Timeout config per request - success expected`` () =
    use server = serverWithRequestDuration (TimeSpan.FromSeconds 1.0)

    (sendRequestWithTimeout (Some 20.0)) ()


[<TestCase>]
let ``Timeout config per request - timeout expected`` () =
    use server = serverWithRequestDuration (TimeSpan.FromSeconds 10.0)
    sendRequestWithTimeout (Some 1.0) |> should throw typeof<TaskCanceledException>


[<TestCase>]
let ``Timeout config global - success expected`` () =
    use server = serverWithRequestDuration (TimeSpan.FromSeconds 1.0)

    GlobalConfig.defaults |> Config.timeoutInSeconds 20.0 |> GlobalConfig.set

    (sendRequestWithTimeout None) ()


[<TestCase>]
let ``Timeout config global - timeout expected`` () =
    use server = serverWithRequestDuration (TimeSpan.FromSeconds 10.0)

    GlobalConfig.defaults |> Config.timeoutInSeconds 20.0 |> GlobalConfig.set

    (sendRequestWithTimeout None) ()

    GlobalConfig.defaults |> Config.timeoutInSeconds 1.0 |> GlobalConfig.set

    sendRequestWithTimeout None |> should throw typeof<TaskCanceledException>


[<TestCase>]
let ``Cancellation token can be supplied by user`` () =
    let serverRequestDuration = TimeSpan.FromSeconds 10.0
    let clientRequestDuration = TimeSpan.FromSeconds 3.0
    let expectedOverheadTime = TimeSpan.FromSeconds 2.0

    use server = serverWithRequestDuration serverRequestDuration

    (sendRequestWithTimeout None) ()

    use cs = new CancellationTokenSource()

    Thread(fun () ->
        Thread.Sleep clientRequestDuration
        cs.Cancel()
    )
        .Start()

    let requestStartTime = DateTime.Now

    let mutable wasCancelled = false

    try
        get (url "") { config_cancellationToken cs.Token } |> Request.send |> ignore
    with :? TaskCanceledException ->
        wasCancelled <- true

    let requestDuration = DateTime.Now - requestStartTime

    (requestDuration + expectedOverheadTime < serverRequestDuration)
    |> should equal true

    wasCancelled |> should equal true


let [<TestCase>] ``Pre-Configured Requests``() =
    let headerName = "X-Custom-Value"
    let headerValue = "Hallo Welt"

    use server = GET >=> request (header headerName >> OK) |> serve

    let httpSpecial =
        http {
            header headerName headerValue
        }

    let response =
        httpSpecial {
            GET (url @"")
        }
        |> Request.send
        |> Response.toText
    
    do printfn "RESPONSE: %A" response

    response |> should equal headerValue


let [<TestCase>] ``Header Transformer``() =
    let url = "http://"
    let urlSuffix1 = "suffix1"
    let urlSuffix2 = "suffix2"

    let httpSpecial =
        let transformWith suffix =
            fun (header: Header) ->
                let address = (header.target.address |> Option.defaultValue "")
                { header with target.address = Some $"{address}{suffix}" }
        http {
            config_transformHeader (transformWith urlSuffix1)
            config_transformHeader (transformWith urlSuffix2)
        }

    httpSpecial {
        GET url
    }
    |> Request.toRequestAndMessage
    |> fst
    |> _.header.target.address
    |> Option.defaultValue ""
    |> should equal $"{url}{urlSuffix1}{urlSuffix2}"

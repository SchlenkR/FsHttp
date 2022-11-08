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

let [<TestCase>] ``Global config snapshot is used in moment of request creation`` () =

    let setTimeout t =
        GlobalConfig.defaults
        |> Config.timeoutInSeconds t
        |> GlobalConfig.set
    
    let t1 = 11.5
    let t2 = 22.5

    do GlobalConfig.defaults.Config.timeout |> should not' (equal t1)
    do GlobalConfig.defaults.Config.timeout |> should not' (equal t2)

    setTimeout t1

    let r1 = http {
        GET (url @"")
    }

    do r1.config.timeout.TotalSeconds |> should equal t1

    setTimeout t2

    // still t1
    do r1.config.timeout.TotalSeconds |> should equal t1
    
    let r2 = http {
        GET (url @"")
    }

    do r2.config.timeout.TotalSeconds |> should equal t2

let private longRunningRequestServer requestTime =
    GET
    >=> request (fun r ->
        Thread.Sleep (TimeSpan.FromSeconds requestTime)
        "" |> OK)
    |> serve

let [<TestCase>] ``Timeout with Dotnet defaults``() =
    use server = longRunningRequestServer 60.0

    (fun () ->
        get (url "")
        |> Request.send
        |> ignore
    )
    |> should throw (typeof<TaskCanceledException>)

let [<TestCase>] ``Timeout config per request``() =
    use server = longRunningRequestServer 60.0

    (fun () ->
        get (url "") {
            config_timeoutInSeconds 5.0
        }
        |> Request.send
        |> ignore
    )
    |> should throw (typeof<TaskCanceledException>)
    
let [<TestCase>] ``No timeout config per request``() =
    use server = longRunningRequestServer 60.0

    get (url "") {
        config_timeoutInSeconds 110.0
    }
    |> Request.send
    |> ignore

let [<TestCase>] ``Timeout config global``() =
    use server = longRunningRequestServer 60.0

    GlobalConfig.defaults
    |> Config.timeoutInSeconds 5.0
    |> GlobalConfig.set

    (fun () ->
        get (url "")
        |> Request.send
        |> ignore
    )
    |> should throw (typeof<TaskCanceledException>)
    

let [<TestCase>] ``No timeout config global``() =
    use server = longRunningRequestServer 60.0

    GlobalConfig.defaults
    |> Config.timeoutInSeconds 110.0
    |> GlobalConfig.set

    get (url "")
    |> Request.send
    |> ignore
    

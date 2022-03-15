module FsHttp.Tests.GlobalConfig

open FsUnit
open FsHttp
open FsHttp.Tests.Server

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

    // still t1"
    do r1.config.timeout.TotalSeconds |> should equal t1
    
    let r2 = http {
        GET (url @"")
    }

    do r2.config.timeout.TotalSeconds |> should equal t2

module FsHttp.Tests.``Builders and Signatures``

open System
open System.Net.Http
open FsHttp
open NUnit.Framework
open FsUnit

let signatures () =
    let _: IToRequest = http { GET "" }
    let _: Request = http { GET "" } |> Request.toRequest
    let _: HttpRequestMessage = http { GET "" } |> Request.toHttpRequestMessage
    let _: Async<Response> = http { GET "" } |> Request.toAsync None
    let _: Async<Response> = http { GET "" } |> Request.sendAsync
    let _: Response = http { GET "" } |> Request.send
    ()

let ``Shortcuts work - had problems with resolution before`` () =
    get "https://myService" {
        multipart
        textPart "" ""
    }

(*
let ``Explicit 'body' keyword is needed for describing request body`` () =
    http {
        GET ""
        json ""
    }
*)

(*
let ``Explicit 'multibody' keyword is needed for describing request body`` () =
    http {
        GET ""
        textPart ""
    }
*)

let ``General configuration is possible on all builder contextx`` () =
    http {
        config_timeoutInSeconds 1.0
        GET "http://myService.com"
    }
    |> ignore

    http {
        GET "http://myService.com"
        config_timeoutInSeconds 1.0
    }
    |> ignore

    http {
        GET "http://myService.com"
        body
        text ""
        config_timeoutInSeconds 1.0
    }
    |> ignore


    http {
        GET "http://myService.com"
        multipart
        textPart "" ""
        config_timeoutInSeconds 1.0
    }
    |> ignore

let ``Print configuration is possible on all builder contextx`` () =
    http {
        print_headerOnly
        GET "http://myService.com"
    }
    |> ignore

    http {
        GET "http://myService.com"
        print_headerOnly
    }
    |> ignore

    http {
        GET "http://myService.com"
        body
        text ""
        print_headerOnly
    }
    |> ignore

    http {
        GET "http://myService.com"
        multipart
        textPart "" ""
        print_headerOnly
    }
    |> ignore

[<TestCase>]
let ``Config of StartingContext is taken`` () =
    let timeout = TimeSpan.FromSeconds 22.2

    let req =
        http {
            config_timeout timeout
            GET "http://myservice"
        }

    GlobalConfig.defaults.Config.timeout |> should not' (equal (Some timeout))
    req.config.timeout |> should equal (Some timeout)

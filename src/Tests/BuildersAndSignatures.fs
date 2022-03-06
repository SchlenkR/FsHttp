module FsHttp.Tests.``Builders and Signatures``

open System.Net.Http
open FsHttp
open FsHttp.DslCE

let signatures () =
    let _: IToRequest = http { GET "" }
    let _: Request = http { GET "" } |> Request.toRequest
    let _: HttpRequestMessage = http { GET "" } |> Request.toMessage
    let _: Async<Response> = http { GET "" } |> Request.toAsync
    let _: Async<Response> = http { GET "" } |> Request.sendAsync
    let _: Response = http { GET "" } |> Request.send
    ()

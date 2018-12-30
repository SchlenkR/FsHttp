module Server

open System.Threading
open Suave
open Suave.Filters
open Suave.Operators

type Route = {
    method: WebPart;
    route: string;
    handler: HttpRequest -> WebPart }

let url s = sprintf "http://127.0.0.1:8080%s" s

let serve (app:WebPart) =
    let cts = new CancellationTokenSource()
    let conf = { defaultConfig with cancellationToken = cts.Token }
    let listening, server = startWebServerAsync conf app
    
    Async.Start(server, cts.Token)

    let dispose() =
        cts.Cancel()
        cts.Dispose()
    { new System.IDisposable with member this.Dispose() = dispose() }

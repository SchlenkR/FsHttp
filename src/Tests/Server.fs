module Server

open System.Threading
open Suave
open Suave.Filters
open Suave.Operators

type Route = {
    method: WebPart;
    route: string;
    handler: HttpRequest -> WebPart }

////let app =
////    choose [ 
////        GET >=> choose [
////            path "/hello" >=> request (fun r ->
////                defaultArg (Option.ofChoice (r.header "accept-language")) "NOTSET" |> OK)
////            path "/goodbye" >=> OK "Good bye GET" ]
////        POST >=> choose [
////            path "/hello" >=> OK "Hello POST"
////            path "/goodbye" >=> OK "Good bye POST" ]
////    ]

let url s = sprintf "http://127.0.0.1:8080%s" s

let serve (routes: Route list) =
    let app =
        routes
        |> List.map (fun route -> route.method >=> choose [ path route.route >=> request route.handler ])
        |> choose   

    let cts = new CancellationTokenSource()
    let conf = { defaultConfig with cancellationToken = cts.Token }
    let listening, server = startWebServerAsync conf app
    
    Async.Start(server, cts.Token)

    let dispose() =
        cts.Cancel()
        cts.Dispose()
    { new System.IDisposable with member this.Dispose() = dispose() }

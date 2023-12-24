
#r "nuget: RestInPeace"
#r "nuget: Suave"

open System
open System.Threading

open RestInPeace

open Suave
open Suave.Operators
open Suave.Successful

module Intercept =

    let basePort = 8080
    let baseAdapter = "127.0.0.1"

    let makeUrl (s: string) = $"http://{baseAdapter}:{basePort}{s}"

    let serve (app: WebPart) =
        let cts = new CancellationTokenSource()
        let conf = 
            { defaultConfig with 
                cancellationToken = cts.Token
                bindings = [ HttpBinding.createSimple HTTP baseAdapter basePort ]
            }
        let listening, server = startWebServerAsync conf app
        do Async.Start(server, cts.Token)
        do
            listening
            |> Async.RunSynchronously
            |> Array.choose id
            |> Array.map (fun x -> x.binding |> string)
            |> String.concat "; "
            |> printfn "Server ready and listening on: %s"

        let dispose () =
            cts.Cancel()
            cts.Dispose()
        { new System.IDisposable with
            member _.Dispose() = dispose () }

    type Wire = { method: WebPart; url: Uri; rewrite: string; resp: WebPart }

    let rewire (wires: Wire list) =
        do
            GlobalConfig.defaults
            |> Config.transformHttpRequestMessage (fun msg ->
                let requestedUriWithoutQuery = msg.RequestUri.GetLeftPart(UriPartial.Path)
                let wire = 
                    wires
                    |> List.tryFind (fun wire -> Uri(requestedUriWithoutQuery) = wire.url)
                    |> Option.map (fun x -> makeUrl x.rewrite)
                match wire with
                | Some wire -> 
                    msg.RequestUri <- Uri(wire)
                | None ->
                    failwith "NO FOUND"
                    ()
                msg
            )
            |> GlobalConfig.set
        let stopServer =
            wires
            |> List.map (fun x -> x.method >=> Filters.path x.rewrite >=> x.resp)
            |> choose
            |> serve
        let dispose () =
            stopServer.Dispose()
            GlobalConfig.defaults |> Config.transformHttpRequestMessage id |> GlobalConfig.set
        { new System.IDisposable with
            member _.Dispose() = dispose () }

open Intercept

let myTestCase () =
    
    // Intercept a GET on wikipedia and return 200 with content "You know it"
    use _ =
        [
            {
                method = Filters.GET
                url = Uri("https://www.wikipedia.de")
                rewrite = "/"
                resp = OK "You know it" 
            }
        ]
        |> rewire

    let responseText = 
        http {
            GET "https://www.wikipedia.de"
        }
        |> Request.send
        |> Response.toText
    
    if responseText <> "You know it" then
        failwith "Test failed."

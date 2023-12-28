
#r "../src/FsHttp/bin/debug/net7.0/FsHttp.dll"

open System
open System.Net.Http
open FsHttp


let doPrint (client : HttpClient) =
    printfn "TIMEOUT: %A" client.Timeout
    client
    

module A =
    
    GlobalConfig.defaults()
    |> Config.timeoutInSeconds 0.1
    |> GlobalConfig.set
    
    http {
        GET "https://www.google.de"
        config_transformHttpClient doPrint
    }
    |> Request.send


module B =

    GlobalConfig.defaults()
    |> Config.timeoutInSeconds 0.1
    |> GlobalConfig.set

    http {
        GET "https://www.google.de"
        //config_transformHttpClient doPrint
    }
    |> Request.send


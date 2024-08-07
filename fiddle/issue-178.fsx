
#r "../src/FsHttp/bin/debug/net6.0/FsHttp.dll"

open System.IO
open System.Net.Http
open System.Net.Http.Headers
open FsHttp
open FsHttp.Operators


let httpd0 =
    http {
        config_transformHeader (fun (header: Header) ->
            printfn "header.target: %A" header.target
            printfn "header.target.address: %A" header.target.address

            let address = "http://aaaa:5000" </> (header.target.address |> Option.defaultValue "")
            { header with target.address = Some address })
    }



let httpd1 =
    http {
        config_transformUrl (fun url -> "http://aaaa:5000" </> url)
    }

let httpd2 =
    http {
        config_useBaseUrl "http://aaaa:5000"
    }

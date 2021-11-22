(**
---
title: HttpClient And HttpMessage
index: 5
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "nuget: FSharp.Data"
#r "../FsHttp/bin/Release/net5.0/publish/FsHttp.dll"
open FsHttp
open FsHttp.DslCE


(**
## Access HttpClient and HttpMessage

Transform underlying http client and do whatever you feel you gave to do:
*)
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
    configure_transformHttpClient (fun httpClient ->
        // this will cause a timeout exception
        httpClient.Timeout <- System.TimeSpan.FromMilliseconds 1.0
        httpClient)
}

(**
Transform underlying http request message:
*)
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
    transformHttpRequestMessage (fun msg ->
        printfn "HTTP message: %A" msg
        msg)
}

(**
---
title: HttpClient And HttpMessage
category: Documentation
categoryindex: 1
index: 11
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../src/FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp


(**
## Access HttpClient and HttpMessage

Transform underlying http client and do whatever you feel you gave to do:
*)
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
    config_transformHttpClient (fun httpClient ->
        // this will cause a timeout exception
        httpClient.Timeout <- System.TimeSpan.FromMilliseconds 1.0
        httpClient)
}

(**
Transform underlying http request message:
*)
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
    config_transformHttpRequestMessage (fun msg ->
        printfn "HTTP message: %A" msg
        msg)
}

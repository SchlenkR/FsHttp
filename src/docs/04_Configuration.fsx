(**
---
title: Configuration
index: 4
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "nuget: FSharp.Data"
#r "../FsHttp/bin/Release/netstandard2.1/publish/FsHttp.dll"
open FsHttp
open FsHttp.DslCE


(**
## Configuration: Timeouts, etc.

You can specify a timeout:
*)
// should throw because it's very short
http {
    GET "http://www.google.de"
    timeoutInSeconds 0.1
}

(**
You can also set config values globally (inherited when requests are created):
*)
FsHttp.Config.setDefaultConfig (fun config ->
    { config with timeout = System.TimeSpan.FromSeconds 15.0 })


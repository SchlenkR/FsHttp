(**
---
title: Configuration
category: Documentation
categoryindex: 1
index: 12
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../src/FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp


(**
## Per request configuration

It's possible to configure requests per instance by the use of `config_`
methods in any stage of the request definition:
*)
http {
    config_timeoutInSeconds 11.1
    GET "http://myService"
}

// or

get "http://myService"
|> Config.timeoutInSeconds 11.1

(**
## Global configuration

You can also set config values globally (inherited when requests are created):
*)
GlobalConfig.defaults
|> Config.timeoutInSeconds 11.1
|> GlobalConfig.set

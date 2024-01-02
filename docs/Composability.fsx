(**
---
title: Composability
category: Documentation
categoryindex: 1
index: 15
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../src/FsHttp/bin/Release/net8.0/FsHttp.dll"
open FsHttp


(**
## Composability 

TODO: Use transformHeader to pre-configure URLs

Currently, see tests/Config.fs (Header Transformer) to get an idea of how to tweak a builder for - e.g - a specific environment.

An example with comments:
*)

open FsHttp
open FsHttp.Operators

let httpForMySpecialEnvironment =
    let baseUrl = "http://my-special-environment"
    http {
        // we would like to have a fixed URL prefix for all requests.
        // So we define a new builder that actually transforms the header.
        // Since the description of method is a special thing,
        // we have to change the URL for any method using a header transformer,
        // like so:
        config_transformHeader (fun (header: Header) ->
            let address = baseUrl </> header.target.address.Value
            { header with target.address = Some address })

        // other header values can be just configured as usual:
        AuthorizationBearer "**************"
    }

let response =
    httpForMySpecialEnvironment {
        GET "/api/v1/users"
    }
    |> Request.sendAsync

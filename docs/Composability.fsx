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
#r "../src/FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp


(**
## Composability 

There are many ways to compose HTTP requests with FsHttp, depending on your needs. A common pattern is to define a base request builder that is then used to create more specific request builders. This is useful when targeting different environments, and don't want to repeat the same configuration for each request.

An example with comments:
*)

open FsHttp.Operators

let httpForMySpecialEnvironment =
    let baseUrl = "http://my-special-environment"
    http {
        // we would like to have a fixed URL prefix for all requests.
        // So we define a new builder that actually uses a base url, like so:
        config_useBaseUrl baseUrl

        // ...in case you need more control, you can also transform the URL:
        config_transformUrl (fun url -> baseUrl </> url)

        // ...or you can transform the header in a similar way.
        // Since the description of method is a special thing,
        // we have to change the URL for any method using a header transformer,
        // like so:
        config_transformHeader (fun (header: Header) ->
            let address = baseUrl </> (header.target.address |> Option.defaultValue "")
            { header with target.address = Some address })

        // other header values can be just configured as usual:
        AuthorizationBearer "**************"
    }

let response =
    httpForMySpecialEnvironment {
        GET "/api/v1/users"
    }
    |> Request.sendAsync

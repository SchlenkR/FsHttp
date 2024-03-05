(**
---
title: Overview
category: Documentation
categoryindex: 1
index: 2
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../src/FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp



(**
## Installing
*)
// Reference the 'FsHttp' package from NuGet in your script or project
#r "nuget: FsHttp"

open FsHttp

(**
## Performing a GET request:
*)
http {
    GET "https://mysite"
    AcceptLanguage "en-US"
}
|> Request.send

(**
The request is sent synchronously in the example above. See (TODO: `async` / `task`) section to see how requests and responses can be processed using `async` or `task` abstractions.

> **For production use, it is recommended using `async` or `task` based functions!**

## Performing a POST request with JSON object content:
*)
http {
    POST "https://mysite"

    body
    jsonSerialize
        {|
            name = "morpheus"
            job = "leader"
        |}
}
|> Request.send

(**
There are more ways of how requests definition can look: See (here)TODO for an explanation of how to multipart, form data, file upload, streaming, and more.

## Process response content as JSON:
*)

// Assume this returns: { "name": "Paul"; "age": 54 }
let name,age =
    http {
        GET "https://mysite"
        AcceptLanguage "en-US"
    }
    |> Request.send
    |> Response.toJson
    |> fun json -> json?name.GetString(), json?age.GetInt32()

(**

Use the `?` operator to access JSON properties. The `GetString()`, `GetInt32()` and similar methods are used to convert the JSON values to the desired type. They are defined as extension methods in `System.Text.Json.JsonElement`.

**FSharp.Data and Newtonsoft.Json**

Per default, `System.Text.Json` is used as backend for dealing with JSON responses. If prefer `FSharp.Data` or `Newtonsoft.Json`, you can use the extension packages (see here(TODO)).

## Configuration
*)

// A configuration per request
http {
    GET "https://mysite"
    AcceptLanguage "en-US"

    // This can be placed anywhere in the request definition.
    config_timeoutInSeconds 10.0
}
|> Request.send

(**
There are many ways of configuring a request - from simple config values like above, to changing or replacing the underlying `System.Net.Http.HttpClient` and `System.Net.Http.HttpRequestMessage` (have a look here()TODO).

It is also possible to set configuration values globally:
*)

GlobalConfig.defaults
|> Config.timeoutInSeconds 11.1
|> GlobalConfig.set

(**
---
title: Response Handling
category: Documentation
categoryindex: 1
index: 6
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp


(**
## Response Content Transformations

There are several ways transforming the content of the returned response to
something like text or JSON:

See also: [Response](reference/fshttp-response.html)
*)
http {
    POST "https://reqres.in/api/users"
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}
|> Request.send
|> Response.toJson



(**
## JSON dynamic processing:
*)

http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> Request.send
|> Response.toJson
|> fun json -> json?page.GetInt32()



(**
## JsonSerializerOptions / Using Tarmil-FSharp.SystemTextJson

FSharp.SystemTextJson enables JSON (de)serialization of F# types like tuples, DUs and others.
To do so, use the `JsonSerializeWith` or one of the `Response.toJsonWith` functions and pass
`JsonSerializerOptions`. Instead, it's also possible to globally configure the `JsonSerializerOptions`
that will be used as default for any request where JSON (de)serialization is involved:
*)

#r "nuget: FSharp.SystemTextJson"

// ---------------------------------
// Prepare global JSON configuration
// ---------------------------------

open System.Text.Json
open System.Text.Json.Serialization

FsHttp.GlobalConfig.Json.defaultJsonSerializerOptions <-
    let options = JsonSerializerOptions()
    options.Converters.Add(JsonFSharpConverter())
    options

// -----------------
// ... make requests
// -----------------

type Person = { name: string; age: int; address: string option }
let john = { name ="John"; age = 23; address = Some "whereever" }

http {
    POST "loopback body"
    body
    jsonSerialize john
}
|> Request.send
|> Response.deserializeJson<Person>
|> fun p -> p.address = john.address // true

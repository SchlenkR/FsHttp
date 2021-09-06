(**
---
title: Response Handling
index: 3
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "nuget: FSharp.Data"
#r "../FsHttp/bin/Release/netstandard2.1/publish/FsHttp.dll"
open FsHttp
open FsHttp.DslCE


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
|> Response.toJson

(**
Works of course also like this:
*)
post "https://reqres.in/api/users" {
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
    send
}
|> Response.toJson


(**
Use FSharp.Data.JsonExtensions to do JSON processing:
*)
open FSharp.Data
open FSharp.Data.JsonExtensions

http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> Response.toJson
|> fun json -> json?page.AsInteger()


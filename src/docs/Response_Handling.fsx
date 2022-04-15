(**
---
title: Response Handling
category: Documentation
categoryindex: 1
index: 5
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
JSON dynamic processing:
*)
open System.Text.Json

http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> Request.send
|> Response.toJson
|> fun json -> json?page.GetInt32()

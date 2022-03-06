(**
---
title: Making Requests
category: Documentation
categoryindex: 1
index: 2
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "nuget: FSharp.Data"
#r "../FsHttp/bin/Release/netstandard2.0/FsHttp.dll"
open FsHttp
open FsHttp.DslCE

(**
## Basics
*)

(**
Build up a GET request:
*)

http {
    GET "https://myApi"
}
|> Request.send

(**
Add headers:
*)
http {
    GET "https://myApi"
    CacheControl "no-cache"
}
|> Request.send

(**
POST a JSON:
*)
http {
    POST "https://myApi"

    // use "body" keyword to start specifying body properties
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}
|> Request.send


(**
POST of form data:
*)
http {
    POST "https://myApi"

    // use "body" keyword to start specifying body properties
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}
|> Request.send



(**
## URL Formatting (Line Breaks and Comments)

You can split URL query parameters or comment lines out by using F# line-comment syntax.
Line breaks and trailing or leading spaces will be removed:
*)
http {
    GET "https://myApi
            ?page=2
            //&skip=5
            &delay=3"
}
|> Request.send


// TODO: Document 'query' operation

// TODO: A config example


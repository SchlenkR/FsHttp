(**
---
title: URLs and Query Params
category: Documentation
categoryindex: 1
index: 3
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../src/FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp



(**
## URLs (Line Breaks and Comments):

You can split URL query parameters or comment lines out by using F# line-comment syntax. Line breaks and trailing or leading spaces will be removed:
*)

http {
    GET "https://mysite
            ?page=2
            //&skip=5
            &name=Hans"
}
|> Request.send


(**
## Query Parameters

It's also possible to specify query params in a list:
*)

http {
    GET "https://mysite"
    query [
        "page", "2"
        "name", "Hans"
    ]
}
|> Request.send

(**
**Please note:** Using F# version 5 or lower, an upcast of the parameter values is needed!
*)
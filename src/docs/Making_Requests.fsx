(**
---
title: Making Requests
_category: Some Category
_categoryindex: 2
index: 2
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../FsHttp/bin/Release/netstandard2.1/publish/FSharp.Data.dll"
#r "../FsHttp/bin/Release/netstandard2.1/publish/FsHttp.dll"
open FsHttp
open FsHttp.DslCE


(**
## Quick Start: Build up a GET request
*)

http {
    GET "https://reqres.in/api/users"
}

(**
add a header...
*)
http {
    GET "https://reqres.in/api/users"
    CacheControl "no-cache"
}

(**
Here is an example of a POST with JSON as body:
*)
http {
    POST "https://reqres.in/api/users"
    // after the HTTP verb, specify header properties
    CacheControl "no-cache"
    // use "body" keyword to start specifying body properties
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}


(**
## Verb-First Requests (Syntax)

Alternatively, you can write the verb first.
Note that computation expressions must not be empty, so you
have to write at lease something, like 'id', 'go', 'exp', etc.

Have a look at: ```./src/FsHttp/DslCE.fs, module Shortcuts```
*)

get "https://reqres.in/api/users" { send }

(**
Inside the ```{ }```, you can place headers as usual...
*)
get "https://reqres.in/api/users" {
    CacheControl "no-cache"
    send
}


(**
## URL Formatting (Line Breaks and Comments)

You can split URL query parameters or comment lines out by using F# line-comment syntax.
Line breaks and trailing or leading spaces will be removed:
*)
get "https://reqres.in/api/users
            ?page=2
            //&skip=5
            &delay=3" {
    send }



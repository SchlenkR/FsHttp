(**
---
title: Making Requests
index: 2
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "nuget: FSharp.Data"
#r "../FsHttp/bin/Release/netstandard2.1/publish/FsHttp.dll"
open FsHttp
open FsHttp.DslCE

(**
## Basics
*)

(**
Build up a GET request:
*)

http {
    GET "https://reqres.in/api/users"
}

(**
Add headers:
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
have to write at least 'send'.

*)

get "https://reqres.in/api/users" { send }

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


// TODO: Document 'query' operation

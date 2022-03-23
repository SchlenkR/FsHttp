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
#r "../FsHttp/bin/Release/net6.0/FsHttp.dll"

(**
## Basics
*)

open FsHttp

(**
Build up a GET request (don't send it yet):
*)

http {
    GET "https://myApi"
}

(**
There is also a short form for this:
*)
get "https://myApi"

(**
No matter which form you choose, you can add headers:
*)
http {
    GET "https://myApi"
    CacheControl "no-cache"
}

(**
POST JSON content:
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


(**
## Sending requests
*)

// Sends a request immediately and blocks until it returns:
http {
    GET "https://myApi"
}
|> Request.send

// Sends a request immediately asynchronousely ("hot" async):
http {
    GET "https://myApi"
}
|> Request.sendAsync

// Builds an async request, but doesn't start sending ("cold" async - idiomatic F#):
http {
    GET "https://myApi"
}
|> Request.toAsync

(**
There is also a shortcut for sending a request (immediately; blocking - like `Request.send`),
which is the `%` operator. The intention of this operator is simplifying FSI usage.
*)

open FsHttp.Operators

% get "http://myApi"

// or

% http {
    GET "http://myApi"
}

(**
## Multipart request

POST form data:
*)
http {
    POST "https://myApi"

    // use "multipart" keyword to start specifying multiple parts
    multipart
    stringPart "user" "morpheus"
    stringPart "secret" "redpill"
}
|> Request.send


(**
> Please have a look at the [./src/Tests/Multipart.fs](multipart tests) for more documentation.

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


(**
### Query parameters

It's also possible to specify query params in a list:
*)

http {
    GET "https://myApi"
    query [
        "page", 2
        "skip", 5
        "name", "Hans"
    ]
}

(**
**Please note:** Up to including F# 5, an upcast of the parameter values is needed!
*)

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
#r "../FsHttp/bin/Release/net6.0/FsHttp.dll"

(**
Installing
*)
// Reference the 'FsHttp' package from NuGet in your script or project
#r "nuget: FsHttp"

// Opening 'FsHttp' is sufficient
// (no need for FsHttp.DSL or others anymore).
open FsHttp

(**
Performing a GET request:
*)
http {
    GET "https://mysite"
    AcceptLanguage "en-US"
}
|> Request.sendAsync

(**
An alternative way: Verb-first functions
*)
get "https://mysite" {
    AcceptLanguage "en-US"
}
|> Request.sendAsync

(**
Working in F# Interactive or notebooks, a short form for sending requests can be used: The `%` operator.

> Note: Since the `%` operator send a synchronous request (blocking the caller thread),
> it is only recommended for using in an interactive environment.
*)
open FsHttp.Operators

% http {
    GET "https://mysite"
    AcceptLanguage "en-US"
}

(**
Performing a POST request with JSON string content:
*)
http {
    POST "https://mysite"
    
    // use "body" keyword to start specifying body properties
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}
|> Request.sendAsync


(**
Performing a POST request with JSON object content:
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
|> Request.sendAsync



(**
Performing a POST multipart request:

> Please have a look at the [https://github.com/fsprojects/FsHttp/blob/master/src/Tests/Multipart.fs](multipart tests) for more documentation.
*)
http {
    POST "https://mysite"

    // use "multipart" keyword (instead of 'body') to start specifying multiple parts
    multipart
    stringPart "user" "morpheus"
    stringPart "secret" "redpill"
    filePartWithName "super.txt" "F# rocks!"
}
|> Request.sendAsync


(**
URL Formatting (Line Breaks and Comments): You can split URL query parameters or comment 
lines out by using F# line-comment syntax.
Line breaks and trailing or leading spaces will be removed:
*)
http {
    GET "https://mysite
            ?page=2
            //&skip=5
            &name=Hans"
}
|> Request.sendAsync


(**
### Query parameters

It's also possible to specify query params in a list:
> Please note:** Up to including F# 5, an upcast of the parameter values is needed!
*)

http {
    GET "https://mysite"
    query [
        "page", 2
        "name", "Hans"
    ]
}
|> Request.sendAsync

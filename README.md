
# FsHttp

FsHttp is a convenient library for consuming HTTP/REST endpoints via F#. It is based on System.Net.Http.

[![NuGet Badge](http://img.shields.io/nuget/v/SchlenkR.FsHttp.svg?style=flat)](https://www.nuget.org/packages/SchlenkR.FsHttp) [![Build Status](https://travis-ci.org/ronaldschlenker/FsHttp.svg?branch=master)](https://travis-ci.org/ronaldschlenker/FsHttp)

The goal of FsHttp is to provide ways for describing HTTP requests in a convenient way, and it is inspired by the RestClient VSCode extension. It can be used in production code, in tests, and in F# interactive.

Parts of the code is taken from the [HTTP utilities of FSharp.Data](http://fsharp.github.io/FSharp.Data/library/Http.html).

FsHttp comes in 2 'flavours' that can be used to describe HTTP requests. Although it is a good thing to have 1 solution for a problem instead of 2, it's up to you which style you prefer.

## Building

You need to have dotnet SDK 3.1.202 installed (as specified in global.json).

## Sources and Demos

Have a look at these files for more use cases:

* [Demo script for CE Dsl](src/Samples/Demo.DslCE.fsx)
  This file demonstrates the use of the CE (computation expression) syntax.

* [Demo script for op-less Dsl](src/Samples/Demo.Dsl.fsx)
  This file demonstrates the use of the op-less (operator-less) syntax.

* [Integration Tests](src/Tests/IntegrationTests.fs)
  The tests show various use cases.


## Setup (including FSI)


```fsharp

// Inside F# Interactive, load the FsHttp script instead of referencing the dll.
// This will register pretty output printers for HTTP requests and responses.
#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp

// Choose your style (here: Computation Expression)
open FsHttp.DslCE
```

## Getting Started: Build up a GET request

*Hint:* The request will be sent immediately and synchronous.


```fsharp

http {
    GET "https://reqres.in/api/users"
}
```
add a header...

```fsharp
http {
    GET "https://reqres.in/api/users"
    CacheControl "no-cache"
}
```
Here is an example of a POST with JSON as body:

```fsharp
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
```
## FSI Request/Response Formatting

When you work in FSI, you can control the output formatting with special keywords.

Some predefined printers are defined in ```./src/FsHttp/DslCE.fs, module Fsi```

2 most common printers are:

 - 'go' (alias: 'preview'): This will render a small part of the response content.
 - 'exp' (alias: 'expand'): This will render the whole response content.


```fsharp

http {
    GET "https://reqres.in/api/users"
    CacheControl "no-cache"
    exp
}
```
## Verb-First Requests (Syntax)

Alternatively, you can write the verb first.
Note that computation expressions must not be empty, so you
have to write at lease something, like 'id', 'go', 'exp', etc.

Have a look at: ```./src/FsHttp/DslCE.fs, module Shortcuts```

```fsharp

get "https://reqres.in/api/users" { exp }
```
Inside the ```{ }```, you can place headers as usual...

```fsharp
get "https://reqres.in/api/users" {
    CacheControl "no-cache"
    exp
}
```
## URL Formatting (Line Breaks and Comments)

You can split URL query parameters or comment lines out by using F# line-comment syntax.
Line breaks and trailing or leading spaces will be removed:

```fsharp
get "https://reqres.in/api/users
            ?page=2
            //&skip=5
            &delay=3"
            { go }
```
## Response Content Transformations

There are several ways transforming the content of the returned response to
something like text or JSON:

See also: ```./src/FsHttp/ResponseHandling.fs```

```fsharp

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
|> toJson
```
Works of course also like this:

```fsharp
post "https://reqres.in/api/users" {
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}
|> toJson
```
Use FSharp.Data.JsonExtensions to do JSON stuff:

```fsharp
open FSharp.Data
open FSharp.Data.JsonExtensions

http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
|> fun json -> json?page.AsInteger()
```
## Configuration: Timeouts, etc.

You can specify a timeout:

```fsharp
// should throw because it's very short
http {
    GET "http://www.google.de"
    timeoutInSeconds 0.1
}
```
You can also set config values globally (inherited when requests are created):

```fsharp
FsHttp.Config.setTimeout (System.TimeSpan.FromSeconds 15.0)
```
## Access HttpClient and HttpMessage

Transform underlying http client and do whatever you feel you gave to do:

```fsharp
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
    transformHttpClient (fun httpClient ->
        // this will cause a timeout exception
        httpClient.Timeout <- System.TimeSpan.FromMilliseconds 1.0
        httpClient)
}
```
Transform underlying http request message:

```fsharp
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
    transformHttpRequestMessage (fun msg ->
        printfn "HTTP message: %A" msg
        msg)
}
```
## Lazy Evaluation / Chaining Builders

*Hint:* Have a look at: ```./src/FsHttp/DslCE.fs, module Fsi'```

There is not only the immediate + synchronous way of specifying requests. It's also possible to
simply build a request, pass it around and send it later or to warp it in async.

Chaining builders together: First, use a httpLazy to create a 'HeaderContext'

*Hint:* ```httpLazy { ... }``` is just a shortcut for ```httpRequest StartingContext { ... }```

```fsharp
let postOnly =
    httpLazy {
        POST "https://reqres.in/api/users"
    }
```
Add some HTTP headers to the context:

```fsharp
let postWithCacheControlBut =
    httpRequest postOnly {
        CacheControl "no-cache"
    }
```
Transform the HeaderContext to a BodyContext and add JSON content:

```fsharp
let finalPostWithBody =
    httpRequest postWithCacheControlBut {
        body
        json """
        {
            "name": "morpheus",
            "job": "leader"
        }
        """
    }
```
Finally, send the request (sync or async):

```fsharp
let finalPostResponse = finalPostWithBody |> send
let finalPostResponseAsync = finalPostWithBody |> sendAsync
```
### Async Builder

HTTP in an async context:

```fsharp
let pageAsync =
    async {
        let! response = 
            httpAsync {
                GET "https://reqres.in/api/users?page=2&delay=3"
            }
        let page =
            response
            |> toJson
            |> fun json -> json?page.AsInteger()
        return page
    }

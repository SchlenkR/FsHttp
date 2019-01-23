# FsHttp

FsHttp is a lightweight library for accessing HTTP/REST endpoints via F#.

[![NuGet Badge](http://img.shields.io/nuget/v/SchlenkR.FsHttp.svg?style=flat)](https://www.nuget.org/packages/SchlenkR.FsHttp)
[![Build Status](https://travis-ci.org/ronaldschlenker/FsHttp.svg?branch=master)](https://travis-ci.org/ronaldschlenker/FsHttp)

It can looks like this:

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

...or use an alternative syntax like this:

```fsharp
post "https://reqres.in/api/users"
--cacheControl "no-cache"
--body
--json """
{
    "name": "morpheus",
    "job": "leader"
}
"""
.> go
```

## TOC

- [FsHttp](#fshttp)
    - [TOC](#toc)
    - [Synopsis](#synopsis)
    - [Setup (F# Interactive)](#setup-f-interactive)
    - [Custom Operations Style](#custom-operations-style)
        - [Different Types of Builders](#different-types-of-builders)
    - [Alternative Style](#alternative-style)
    - [Response Handling](#response-handling)
    - [Testing](#testing)
    - [TODO](#todo)

## Synopsis

This library provides a convenient way of interacting with HTTP endpoints.

The focus of FsHttp is:

- Exploring HTTP services interactively by sending HTTP requests and viewing the response in F# interactive.
- Testing of web APIs by sending requests and assert expectations.
- Usage as an HTTP client library in applications.

Parts of the code is taken from the [HTTP utilities of FSharp.Data](http://fsharp.github.io/FSharp.Data/library/Http.html).

## Setup (F# Interactive)

Using FsHttp in F# interactive, the ```FsHttp.fsx``` file should be loaded instead of referencing the dll directly. This will enable pretty printing of a response in the FSI output. The ```FSharp.Data.dll``` has to be referenced in order to use the JSON functionality (e.g. ```toJson```respones function).

```fsharp
#r @".\packages\fsharp.data\lib\net45\FSharp.Data.dll"
#load @".\packages\schlenkr.fshttp\lib\netstandard2.0\FsHttp.fsx"

open FsHttp
open FsHttp.Dsl // enables alternative syntax; see below...
open FSharp.Data.JsonExtensions
```

## Custom Operations Style

A simple GET request looks like this:

```fsharp
http {
    GET "https://reqres.in/api/users?page=2&delay=3"
}
```

You can split query parameters like this:

```fsharp
http {
    GET "https://reqres.in/api/users
            ?page=2
            &skip=5
            &delay=3"
}
```

F# line-comment syntax is supported in urls: skip is not contained in the query string.

```fsharp
http {
    GET "https://reqres.in/api/users
            ?page=2
            //&skip=5
            &delay=3"
}
```

You can set header parameters like this:

```fsharp
http {
    GET "http://www.google.com"
    AcceptLanguage "de-DE"
}
```

Post data like this:

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

### Different Types of Builders

The examples shown here use the **http** builder, which evaluates requests immediately and is executed synchronousely. There are more builders that can be used to achieve a different behavior:

- **http** Immediately evaluated, synchronous
- **httpAsync** Immediately evaluated, asynchronous
- **httpLazy** Manually evaluated (use '|> send' or '|> sendAsync')

The inner DSL is the same for all builders.


The httpLazy builder results in a Request that has to be sent to the server:

```fsharp
httpLazy {
    GET "http://www.google.de"
}
.> go
```

The ```.> go``` function sends the request to the server and returns a response object.

## Alternative Style

FsHttp comes in 2 flavors:

- A 'Custom Operation' syntax ```http { GET ... }``` (as shown above).
- An alternative point free notation like ```get "http://..." .> go```.

To enable the alternative syntax, you have to open the ```FsHttp.Dsl``` module:

```fsharp
open FsHttp.Dsl
```

This will make all the HTTP method-, header-, and body-functions available. It will also import the operators:
- ```(--)``` (alias for pipe forward)
- ```(.>)``` (synchronous request invocation)
- ```(>.)``` (asynchronous request invocation)

Now, you can do things like

```fsharp
get "http://www.google.com" --acceptLanguage "de-DE" .> go
```

## Response Handling 

No matter which syntax you choose, there are several TODO

Convert a response to a JsonValue:

```fsharp
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
```

// TODO: all response functions


## Testing

For using the JSON testing functions, additional references to ```NUnit```, ```FSUnit``` and ```FsHttp.NUnit``` libraries are required as shown here:

```fsharp
#r @".\packages\fsharp.data\lib\net45\FSharp.Data.dll"
#load @".\packages\schlenkr.fshttp\lib\netstandard2.0\FsHttp.fsx"

// additional libs for testing
#r @".\packages\schlenkr.fshttp.nunit\lib\netstandard2.0\FsHttp.nunit.dll"
#r @".\packages\NUnit\lib\netstandard2.0\nunit.framework.dll"
#r @".\packages\fsunit\lib\netstandard2.0\FsUnit.NUnit.dll"

open FsHttp
open FsUnit
open FSharp.Data
open FSharp.Data.JsonExtensions
```

Testing response data by asserting JSON expectations:

```fsharp
http {
    GET "https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
|> (fun json -> json?data.AsArray() |> should haveLength 3)
```

Testing response data by asserting JSON expectations and example:

```fsharp
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
||> (fun json -> json?data.AsArray() |> should haveLength 3)
|> jsonShouldLookLike IgnoreOrder Subset
    """
    {
        "data": [
            {
                "id": 4
            }
        ]
    }
    """
```

The `||>` operator means 'tee' [have a look at](https://fsharpforfunandprofit.com/rop/): It is useful when you want to chain expectations together that all work on the http response.

## TODO

* form url encoded
    * Alternative: ContentType mit "text" body und string
    * document .> and >. operators
* content-type
* edit raw request
* a word to ContentType / Body
* explain: expand, preview, raw, etc.
* explain the DSL operators
* Write Response to a file
* 

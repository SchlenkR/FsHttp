# FsHttp

FsHttp is a lightweight library for accessing HTTP/REST endpoints via F#.

[![NuGet Badge](http://img.shields.io/nuget/v/SchlenkR.FsHttp.svg?style=flat)](https://www.nuget.org/packages/SchlenkR.FsHttp)

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

...or this:

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
}
```

## TOC

- [FsHttp](#fshttp)
    - [TOC](#toc)
    - [Synopsis](#synopsis)
    - [Examples](#examples)
        - [F# Interactive Usage](#f-interactive-usage)
        - [Basics](#basics)
        - [Response Handling and Testing](#response-handling-and-testing)
    - [Hints](#hints)
    - [TODO](#todo)

## Synopsis

This library provides a convenient way of interacting with HTTP endpoints.

The focus of FsHttp is:

- Exploring HTTP services interactively by sending HTTP requests and viewing the response in F# interactive.
- Test web APIs by sending requests and assert expectations.

Parts of the code is taken from the [HTTP utilities of FSharp.Data](http://fsharp.github.io/FSharp.Data/library/Http.html).

## Examples

### F# Interactive Usage

Using FsHttp in F# interactive, you should load the 'FsHttp.fsx' instead of referencing the dll directly. This will enable pretty printing of a response in the FSI output.

For using the JSON and testing functions, reference the FSharp.Data, NUnit and FSUnit libraries. Have a look at the setup shown in the **Tests\IntegrationTests.fs** folder for an example.

```fsharp
#r @".\packages\fsharp.data\lib\net45\FSharp.Data.dll"
#r @".\packages\NUnit\lib\netstandard2.0\nunit.framework.dll"
#r @".\packages\fsunit\lib\netstandard2.0\FsUnit.NUnit.dll"
#load @".\packages\schlenkr.fshttp\lib\netstandard2.0\FsHttp.fsx"

open FsHttp
open FsUnit
open FSharp.Data
open FSharp.Data.JsonExtensions
```

### Basics

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

### Response Handling and Testing

Convert a response to a JsonValue:

```fsharp
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
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

## Hints

The examples shown here use the **http** builder, which evaluates requests immediately and is executed synchronousely. There are more builders that can be used to achieve a different behavior:

- **http** Immediately evaluated, synchronous
- **httpAsync** Immediately evaluated, asynchronous
- **httpLazy** Manually evaluated (use 'send' or 'sendAsync')

The inner DSL is the same for all builders.

## TODO

* form url encoded
    * Alternative: ContentType mit "text" body und string
    * document .> and >. operators
* content-type
* edit raw request
* a word to ContentType / Body
* explain: expand, preview, raw, etc.
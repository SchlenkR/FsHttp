# FsHttp

TODO: Document % operator

FsHttp is a convenient library for consuming HTTP/REST endpoints via F#. It is based on System.Net.Http.

[![NuGet Badge](http://img.shields.io/nuget/v/SchlenkR.FsHttp.svg?style=flat)](https://www.nuget.org/packages/SchlenkR.FsHttp) [![Build Status](https://travis-ci.org/ronaldschlenker/FsHttp.svg?branch=master)](https://travis-ci.org/ronaldschlenker/FsHttp)

The goal of FsHttp is to provide ways for describing HTTP requests in a convenient way, and it is inspired by the
RestClient VSCode extension. It can be used in production code, in tests, and in F# interactive.

Parts of the code is taken from the [HTTP utilities of FSharp.Data](http://fsharp.github.io/FSharp.Data/library/Http.html).

FsHttp comes in 2 'flavours' that can be used to describe HTTP requests. Although it is a good thing to have 1 solution
for a problem instead of 2, it's up to you which style you prefer.

## Building

You need to have dotnet SDK 3.1.202 installed (as specified in global.json).

There is a F# script that can be used to perform several build tasks from command line. It can be executed in this way:

// can't yet format InlineHtmlBlock ("`dotnet fsi .\build.fsx [task]`
", None, None) to pynb markdown

Common tasks are:

// can't yet format Span ([Literal ("test", Some { StartLine = 28 StartColumn = 2 EndLine = 28 EndColumn = 6 })], Some { StartLine = 28 StartColumn = 0 EndLine = 28 EndColumn = 6 }) to pynb markdown
// can't yet format Span ([Literal ("docu", Some { StartLine = 29 StartColumn = 2 EndLine = 29 EndColumn = 6 })], Some { StartLine = 28 StartColumn = 0 EndLine = 28 EndColumn = 6 }) to pynb markdown
// can't yet format Span ([Literal ("publish", Some { StartLine = 30 StartColumn = 2 EndLine = 31 EndColumn = 9 })], Some { StartLine = 28 StartColumn = 0 EndLine = 28 EndColumn = 6 }) to pynb markdown
### Building Readme.md

The content of Readme.md is auto-generated from the file `.\src\Docu\Demo.DslCE.fsx`. In order to build the Readme.md, run the command `dotnet fsi .\build.fsx docu`.

## Sources and Demos

Have a look at these files for more use cases:

[Demo script for CE Dsl](src/Docu/Demo.DslCE.fsx)
This file demonstrates the use of the CE (computation expression) syntax.
[Demo script for op-less Dsl](src/Docu/Demo.Dsl.fsx)
This file demonstrates the use of pipe-style syntax.
[Integration Tests](src/Tests/IntegrationTests.fs)
The tests show various use cases.
## Setup (including FSI)

// can't yet format InlineHtmlBlock ("#r @"../FsHttp/bin/Debug/net5.0/FsHttp.dll"

open FsHttp

// Choose your style (here: Computation Expression)
open FsHttp.DslCE", None, None) to pynb markdown

## Getting Started: Build up a GET request

**Hint:** The request built in this way will be sent immediately and synchronous.

// can't yet format InlineHtmlBlock ("http {
    GET "https://reqres.in/api/users"
}", None, None) to pynb markdown

add a header...

// can't yet format InlineHtmlBlock ("http {
    GET "https://reqres.in/api/users"
    CacheControl "no-cache"
}", None, None) to pynb markdown

Here is an example of a POST with JSON as body:

// can't yet format InlineHtmlBlock ("http {
    POST "https://reqres.in/api/users"
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}", None, None) to pynb markdown

## FSI Request/Response Formatting

When you work in FSI, you can control the output formatting with special keywords.

Some predefined printers are defined in `./src/FsHttp/DslCE.fs, module Fsi`

2 most common printers are:

// can't yet format Span ([Literal ("'prv' (alias: 'preview'): This will render a small part of the response content.", Some { StartLine = 10 StartColumn = 3 EndLine = 10 EndColumn = 83 })], Some { StartLine = 10 StartColumn = 0 EndLine = 10 EndColumn = 83 }) to pynb markdown
// can't yet format Span ([Literal ("'exp' (alias: 'expand'): This will render the whole response content.", Some { StartLine = 11 StartColumn = 3 EndLine = 12 EndColumn = 72 })], Some { StartLine = 10 StartColumn = 0 EndLine = 10 EndColumn = 83 }) to pynb markdown
// can't yet format InlineHtmlBlock ("http {
    GET "https://reqres.in/api/users"
    CacheControl "no-cache"
    exp
}", None, None) to pynb markdown

## Verb-First Requests (Syntax)

Alternatively, you can write the verb first.
Note that computation expressions must not be empty, so you
have to write at lease something, like 'id', 'go', 'exp', etc.

Have a look at: `./src/FsHttp/DslCE.fs, module Shortcuts`

// can't yet format InlineHtmlBlock ("get "https://reqres.in/api/users" { send }", None, None) to pynb markdown

Inside the `{ }`, you can place headers as usual...

// can't yet format InlineHtmlBlock ("get "https://reqres.in/api/users" {
    CacheControl "no-cache"
    exp
}", None, None) to pynb markdown

## URL Formatting (Line Breaks and Comments)

You can split URL query parameters or comment lines out by using F# line-comment syntax.
Line breaks and trailing or leading spaces will be removed:

// can't yet format InlineHtmlBlock ("get "https://reqres.in/api/users
            ?page=2
            //&skip=5
            &delay=3" {
    send }", None, None) to pynb markdown

## Response Content Transformations

There are several ways transforming the content of the returned response to
something like text or JSON:

See also: `./src/FsHttp/ResponseHandling.fs`

// can't yet format InlineHtmlBlock ("http {
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
|> Response.toJson", None, None) to pynb markdown

Works of course also like this:

// can't yet format InlineHtmlBlock ("post "https://reqres.in/api/users" {
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
    send
}
|> Response.toJson", None, None) to pynb markdown

Use FSharp.Data.JsonExtensions to do JSON processing:

// can't yet format InlineHtmlBlock ("open FSharp.Data
open FSharp.Data.JsonExtensions

http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> Response.toJson
|> fun json -> json?page.AsInteger()", None, None) to pynb markdown

## Configuration: Timeouts, etc.

You can specify a timeout:

// can't yet format InlineHtmlBlock ("// should throw because it's very short
http {
    GET "http://www.google.de"
    timeoutInSeconds 0.1
}", None, None) to pynb markdown

You can also set config values globally (inherited when requests are created):

// can't yet format InlineHtmlBlock ("FsHttp.Config.setDefaultConfig (fun config ->
    { config with timeout = System.TimeSpan.FromSeconds 15.0 })", None, None) to pynb markdown

## Access HttpClient and HttpMessage

Transform underlying http client and do whatever you feel you gave to do:

// can't yet format InlineHtmlBlock ("http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
    transformHttpClient (fun httpClient ->
        // this will cause a timeout exception
        httpClient.Timeout <- System.TimeSpan.FromMilliseconds 1.0
        httpClient)
}", None, None) to pynb markdown

Transform underlying http request message:

// can't yet format InlineHtmlBlock ("http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
    transformHttpRequestMessage (fun msg ->
        printfn "HTTP message: %A" msg
        msg)
}", None, None) to pynb markdown

## Lazy Evaluation / Chaining Builders

**Hint:** Have a look at: `./src/FsHttp/DslCE.fs, module Fsi'`

There is not only the immediate + synchronous way of specifying requests. It's also possible to
simply build a request, pass it around and send it later or to warp it in async.

Chaining builders together: First, use a httpLazy to create a 'HeaderContext'

**Hint:** `httpLazy { ... }` is just a shortcut for `httpRequest StartingContext { ... }`

// can't yet format InlineHtmlBlock ("let postOnly =
    httpLazy {
        POST "https://reqres.in/api/users"
    }", None, None) to pynb markdown

Add some HTTP headers to the context:

// can't yet format InlineHtmlBlock ("let postWithCacheControlBut =
    postOnly {
        CacheControl "no-cache"
    }", None, None) to pynb markdown

Transform the HeaderContext to a BodyContext and add JSON content:

// can't yet format InlineHtmlBlock ("let finalPostWithBody =
    postWithCacheControlBut {
        body
        json """
        {
            "name": "morpheus",
            "job": "leader"
        }
        """
    }", None, None) to pynb markdown

Finally, send the request (sync or async):

// can't yet format InlineHtmlBlock ("let finalPostResponse = finalPostWithBody |> Request.send
let finalPostResponseAsync = finalPostWithBody |> Request.sendAsync", None, None) to pynb markdown

### Async Builder

HTTP in an async context:

// can't yet format InlineHtmlBlock ("let pageAsync =
    async {
        let! response = 
            httpAsync {
                GET "https://reqres.in/api/users?page=2&delay=3"
            }
        let page =
            response
            |> Response.toJson
            |> fun json -> json?page.AsInteger()
        return page
    }


// TODO Document naming conventions according to: https://github.com/ronaldschlenker/FsHttp/issues/48", None, None) to pynb markdown

## Naming Conventions

**Names for naming conventions according to: [https://en.wikipedia.org/wiki/Naming_convention_(programming)#Lisp](https://en.wikipedia.org/wiki/Naming_convention_(programming)#Lisp)**

Naming of **HTTP methods inside of a builder** are **upper flat case** (following [https://tools.ietf.org/html/rfc7231#section-4).](https://tools.ietf.org/html/rfc7231#section-4).)
**Example:**
// can't yet format InlineHtmlBlock ("http {
    GET "http://www.whatever.com"
}
", None, None) to pynb markdown
Naming of **HTTP methods used outside of a builder** follow the F# naming convention and are **flat case**.
**Example:**
// can't yet format InlineHtmlBlock ("let request = get "http://www.whatever.com"
", None, None) to pynb markdown
Naming of **HTTP headers inside of a builder** are **PascalCase**. Even though they should be named **train case** (according to [https://tools.ietf.org/html/rfc7231#section-5),](https://tools.ietf.org/html/rfc7231#section-5),) it would require a double backtic using it in F#, which might be uncomfortable.
**Example:**
// can't yet format InlineHtmlBlock ("http {
    // ...
    CacheControl "no-cache"
}
", None, None) to pynb markdown
Naming of **all other constructs** are **lower camel case**. This applies to:
config methods
type transformer (like "body")
content annotations (like "json" or "text")
FSI print modifiers like "expand" or "preview"
invocations like "send"
**Example:**
// can't yet format InlineHtmlBlock ("http {
  // ...
  timeoutInSeconds 10.0
  body
  json """ { ... } """
  expand
}
", None, None) to pynb markdown
## Examples for building, chaining and sending requests

// can't yet format InlineHtmlBlock ("let getUsers1 : LazyHttpBuilder<HeaderContext> = get "https://reqres.in/api/users"
let getUsers2 : LazyHttpBuilder<HeaderContext> = httpLazy { GET "https://reqres.in/api/users" }
let _ : Response = getUsers1 { send }
let _ : Response = get "https://reqres.in/api/users" { send }
let _ : Response = getUsers1 |> Request.send
let _ : Response = http { GET "https://reqres.in/api/users" }
let _ : Async<Response> = httpAsync { GET "https://reqres.in/api/users" }
let _ : Response =
    httpLazy {
        GET "https://reqres.in/api/users"
        send
    }
let _ : Async<Response> =
    httpLazy {
        GET "https://reqres.in/api/users"
        sendAsync
    }

// FSI
let _ : Response =
    http {
        GET "https://reqres.in/api/users"
        CacheControl "no-cache"
        exp
    }

let _ : Response =
    get "https://reqres.in/api/users" {
        CacheControl "no-cache"
        exp
        send
    }", None, None) to pynb markdown



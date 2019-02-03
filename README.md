
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

For a complete list of keyword used in the DSL, have a look at [the builder methods](src/FsHttp/Builder.fs) or [the DSL functions](src/FsHttp/Dsl.fs).

Or have a look at the [Demos](src/Samples/Demo.fsx)

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

## Examples

A simple GET request looks like this:

```fsharp
http {
    GET "https://reqres.in/api/users?page=2&delay=3"
}
```

```fsharp
get "https://reqres.in/api/users?page=2&delay=3" .> go
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

```fsharp
get "https://reqres.in/api/users
            ?page=2
            &skip=5
            &delay=3"
.> go
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

```fsharp
get "https://reqres.in/api/users
            ?page=2
            //&skip=5
            &delay=3"
.> go
```

You can set header parameters like this:

```fsharp
http {
    GET "http://www.google.com"
    AcceptLanguage "de-DE"
}
```

```fsharp
get "http://www.google.com"
--acceptLanguage "de-DE"
.> go
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

## Sync, Async and Lazy: Different Types of Builders

The examples shown here use the **http** builder, which evaluates requests immediately and is executed synchronousely. There are more builders that can be used to achieve a different behavior:

**Hint:** The inner DSL is the same for all builders.

### http

- Immediately invoked
- evaluates to ```Response```

***Example:***

```fsharp
let (response:Response) = http {
    GET "http://www.google.de"
}
```

### httpAsync

- Immediately invoked
- evaluates to ```Async<Response>```

***Example:***

```fsharp
let (response:Async<Response>) = httpAsync {
    GET "http://www.google.de"
}
```

### httpLazy

- Must be invoked manually
- evaluates to a request (represented by ```HeaderContext```or ```BodyContext```)
- Can be invoked by using ```.> go``` (synchronous) or ```>. go``` (asynchronous)

***Example:***

```fsharp
let request = httpLazy {
    GET "http://www.google.de"
}

// invoke the request synchronousely
let (syncResponse:Response) = request .> go

// invoke the request asynchronousely
let (asyncResponse:Async<Response>) = request >. go
```

### Alternative Style

FsHttp comes in 2 flavors:

- A 'Custom Operation' syntax ```http { GET ... }``` (as shown above).
- An alternative point free notation like ```get "http://..." .> go```.

To enable the alternative syntax, you have to open the ```FsHttp.Dsl``` module:

```fsharp
open FsHttp.Dsl
```

Now, you can do things like

```fsharp
get "http://www.google.com" --acceptLanguage "de-DE" .> go
```

There are some new operators that will look requests more like command-line style.

This will make all the HTTP method-, header-, and body-functions available. It will also import the operators:
- ```(--)``` (alias for ```|>``` pipe forward)
- ```(%%)``` (alias for ```<|``` pipe backward)
- ```(.>)``` (synchronous request invocation)
- ```(>.)``` (asynchronous request invocation)

The reason why there are aliases for ```|>``` and ```<| ```is the different precedence, that enables writing requests in a fluent way with less parenthesis.

**Example**


```fsharp
let myOftenUsedBaseUrl url = "http://www.google.de" </> url

get %% myOftenUsedBaseUrl "relativeUri?Hello=World"
--acceptLanguage "de-DE"
.> go
```

## Response Handling 

No matter which syntax you choose, there are several (sync + async) functions that transform the response content. Here are some examples:

Convert a response to a JsonValue:

```fsharp
open FSharp.Data
open FSharp.Data.JsonExtensions

http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
|> fun json -> json?page.AsInteger()
```

```fsharp
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toFormattedText
```


## FSharp Interactive Response Printing

When working inside FSI, there are several 'hints' that can be given to specify the way FSI will print the response. Have a look at [FsiPrinting](src/FsHttp/FsiPrinting.fs) for details.

**Examples**

```fsharp
// Default print options (don't print request; print response headers, but no content)
get @"https://reqres.in/api/users?page=2&delay=3" .> go

// Default print options (don't print request; print response headers + a formatted preview of the content)
get @"https://reqres.in/api/users?page=2&delay=3" .> preview

// Default print options (see above) + max. content length of 100
get @"https://reqres.in/api/users?page=2&delay=3" .> show 100
```

There are switches in the ```PrintModifier``` module that can be chained together for a fine grained control over the print style:

```fsharp
get @"https://reqres.in/api/users?page=2&delay=3"
.> print (noRequest >> withResponseContentMaxLength 500)
```



## Others

### Timeout

Set a **timeout** like this:

```fsharp
get "http://www.google.de" --timeoutInSeconds 5.0 .> go

// or:

http {
    GET "http://www.google.de"
    timeoutInSeconds 5.0
}

```

The default timeout for every request is set to TODO seconds and can be changed like this:


```fsharp
// all request created after this line is evaluated will have a timeout of 15s.
FsHttp.Config.setTimeout (TimeSpan.FromSeconds 15.0)
```

## Testing

**Testing functions are included in the FsHttp.NUnit Nuget package.***

For using the JSON testing functions, additional references to ```NUnit```, ```FSUnit``` and ```FsHttp.NUnit``` libraries are required as shown here:

```fsharp
#r @".\packages\fsharp.data\lib\net45\FSharp.Data.dll"
#load @".\packages\schlenkr.fshttp\lib\netstandard2.0\FsHttp.fsx"

// additional libs for testing
#r @".\packages\schlenkr.fshttp.nunit\lib\netstandard2.0\FsHttp.NUnit.dll"
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

The `||>` operator means 'tee' [have a look at](https://fsharpforfunandprofit.com/rop/): It is useful when you want to chain expectation-functions together that all work on the http response.

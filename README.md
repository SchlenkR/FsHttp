
# FsHttp

FsHttp is a convenient library for consuming HTTP/REST endpoints via F#. It is based on System.Net.Http.

[![NuGet Badge](http://img.shields.io/nuget/v/SchlenkR.FsHttp.svg?style=flat)](https://www.nuget.org/packages/SchlenkR.FsHttp) [![Build Status](https://travis-ci.org/ronaldschlenker/FsHttp.svg?branch=master)](https://travis-ci.org/ronaldschlenker/FsHttp)

The goal of FsHttp is to provide ways for describing HTTP requests in a convenient way, and it is inspired by the RestClient VSCode extension. It can be used in production code, in tests, and in F# interactive.

Parts of the code is taken from the [HTTP utilities of FSharp.Data](http://fsharp.github.io/FSharp.Data/library/Http.html).

FsHttp comes in 2 'flavours' that can be used to describe HTTP requests. Although it is a good thing to have 1 solution for a problem instead of 2, it's up to you which style you prefer.


## Getting Started

Have a look at a simple use case using a POST and json as data. Each style is handles more detailed in the upcoming sections. All flavours are equivalent in functionality; they only differ in syntax.

See `src\Samples` folder for use cases. There are 2 demo files:

* Demo.DslCE.fsx
  This file demonstrates the use of the CE (computation expression) syntax.

* Demo.Dsl.fsx
  This file demonstrates the use of the op-less (operator-less) syntax.

### CE Flavour

```fsharp
http {
    POST "https://reqres.in/api/users"
    CacheControl "no-cache"
    body json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}
```

The CE flavour uses F# Computation Expressions to describe requests. Have a look at the [Sample Script for CE DSL](Samples/Demo.DslCE.fsx) for detailed use cases.

### Pipe Flavour

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
|> send
```

Have a look at the [Sample Script for Pipe DSL](Samples/Demo.DslPipe.fsx) for detailed use cases.

### Op-Less Flavour

```fsharp
post "https://reqres.in/api/users"
    cacheControl "no-cache"
    body json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
    send
```

Have a look at the [Sample Script for OP-less DSL](Samples/Demo.Dsl.fsx) for detailed use cases.


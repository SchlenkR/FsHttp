
# FsHttp

FsHttp is a convenient HTTP client library written in F#. It aims for describing and executing HTTP requests in idiomatic and convenient ways that can be used for production, tests and FSI (F# Interactive).

[![NuGet Badge](http://img.shields.io/nuget/v/SchlenkR.FsHttp.svg?style=flat)](https://www.nuget.org/packages/SchlenkR.FsHttp) [![Build Status](https://travis-ci.org/ronaldschlenker/FsHttp.svg?branch=master)](https://travis-ci.org/ronaldschlenker/FsHttp)

**Example:**

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

## Credits

Parts of the code is taken from the [HTTP utilities of FSharp.Data](http://fsharp.github.io/FSharp.Data/library/Http.html).

## Documentation

FsHttp comes in 2 'flavours' - "Computation Expression" and "Pipe", which are both idiomatic in F#. Both flavours are equally powerful, and it depends on your taste which one you prefer.

* API Docu and Reference
  * Alternative 1: [Computation Expression Flavour](Docu/ComputationExpressionFlavour.md)
  * Alternative 2: [Pipe Flavour](Docu/PipeFlavour.fsx)
* [F# Interactive Usage](Docu/Fsi.fsx)
* [Integration Tests](src/Tests/IntegrationTests.fs) The tests show various use cases.

## Building

You need to have the latest .Net 5 SDK installed.

There is a F# script that can be used to perform several build tasks from command line. It can be executed in this way:

    `dotnet fsi .\build.fsx [task]`

Common tasks are:

* build
* test
* docu
* publish

### Building Readme.md

The `Readme.md` and the content in `./docs` is auto-generated from the files in `./src/Docu`. In order to build them, run the command `dotnet fsi .\build.fsx docu`.


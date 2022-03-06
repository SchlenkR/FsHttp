
FsHttp
======

FsHttp is a HTTP client library written in F#. It aims for describing and executing HTTP requests in convenient ways that can be used in production and interactive environments.

**FsHttp** is authored by [@ronaldschlenker](https://github.com/ronaldschlenker). Feel free to leave a message.

[![NuGet Badge](http://img.shields.io/nuget/v/FsHttp.svg?style=flat)](https://www.nuget.org/packages/FsHttp) ![build status](https://github.com/fsprojects/FsHttp/actions/workflows/push-master_pull-request.yml/badge.svg?event=push)

**Package name change / deprecation hint**

The NuGet package `SchlenkR.FsHttp` is _deprecated_. Please use the package [FsHttp](https://www.nuget.org/packages/FsHttp) for releases >= 6.x


Documentation
-------------

Please see [FsHttp Documentation](https://fsprojects.github.io/FsHttp) site for a detailed documentation.


A Simple Example
----------------

```fsharp
#r "nuget: FsHttp"

open FsHttp
open FsHttp.DslCE

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

Upgrading to v9
---

* `http...` Builders: There is now only a single `http` builder, that is equivalent to the former `httpLazy` builder. To achieve the behaviour of the removed builders, please use:
    * `httpLazy` -> `http { ... }`
    * `http` -> `http { ... } |> Request.send`
    * `httpAsync` -> `http { ... } |> Request.sendAsync`
    * `httpLazyAsync` -> `http { ... } |> Request.toAsync`
    * `httpMessage` -> `http { ... } |> Request.toMessage`
    * see also: [./src/Tests/BuildersAndSignatures.fs](Tests in BuildersAndSignatures.fs)
* Renamed type `LazyHttpBuilder` -> `HttpBuilder`
* Renamed `Request.buildAsync` -> `Request.toAsync`
* Removed `send` and `sendAsync` builder methods


Building
--------

You need to have the latest .Net 5 SDK installed.

### Building binaries, publish, and test

There is a F# script that can be used to perform several build tasks from command line. It can be executed in this way:

`PS> dotnet fsi .\build.fsx [task]`

Common tasks are:

* build
* test
* publish

### Building the documentation

The documentation in `./docs` is auto-generated from the files in `./src/Docu`. In order to build them, run:

`PS> .\docu.ps1`


Credits
-------

Parts of the code is taken from the [HTTP utilities of FSharp.Data](https://fsprojects.github.io/FSharp.Data/library/Http.html).

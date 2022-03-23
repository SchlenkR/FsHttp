
FsHttp
======

FsHttp is a .Net HTTP client library For C# and F#. It aims for describing and executing HTTP requests in convenient ways that can be used in production and interactive environments.

**FsHttp** is authored by [@ronaldschlenker](https://github.com/ronaldschlenker). Feel free to leave a message.

[![NuGet Badge](http://img.shields.io/nuget/v/FsHttp.svg?style=flat)](https://www.nuget.org/packages/FsHttp) ![build status](https://github.com/fsprojects/FsHttp/actions/workflows/push-master_pull-request.yml/badge.svg?event=push)


Documentation
-------------

Please see [FsHttp Documentation](https://fsprojects.github.io/FsHttp) site for a detailed documentation.


A Simple Example
----------------

An example in F#:

```fsharp
#r "nuget: FsHttp"

open FsHttp

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
|> Request.send
```

An example in C#:

```csharp
#r "nuget: FsHttp"

using FsHttp.CSharp;

await "https://reqres.in/api/users".Post()
    .CacheControl("no-cache")
    .Body()
    .Json(@"
        {
            ""name"": ""morpheus"",
            ""job"": ""leader""
        }
    ")
    .SendAsync();
```

Upgrading to v9 (Breaking Changes)
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
* Changed request and response printing (mostly used in FSI)
* Printing related custom operations change in names and behaviour
* `Dsl` / `DslCE` namespaces: There is no need for distinction of both namespaces. It is now sufficient to `open FsHttp` only.
* The `HttpBuilder<'context>` is replaced by `IBuilder<'self>`, so that the CE methods work directly on the `HeaderContext`, `BodyContext`, and `MultipartContext` directly. This simplifies things like mixing Dsl and DslCE, pre-configuring and chaining requests.
* The global configuration is now in the `FsHttp.GlobalConfig` module. The `Config` module is only for functions on request contexts.
* QueryParams is `(string * obj) list` now
* Use of System.Text.Json as a standard JSON library and created separate Newtonsoft and FSharp.Data JSON packages.
* Smaller breaking changes


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

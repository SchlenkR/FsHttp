
FsHttp
======

FsHttp is a .Net HTTP client library for C# and F#. It aims for describing and executing HTTP requests in convenient ways that can be used in production and interactive environments.

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
    jsonSerialize
        {|
            name = "morpheus"
            job = "leader"
        |}
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
    .JsonSerialize(new
        {
            name = "morpheus",
            job = "leader"
        }
    )
    .SendAsync();
```


Migrating to new versions
---

For different upgrade paths, please read the [Migrations section](https://fsprojects.github.io/FsHttp/08_Migrations.html) in the docu.


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

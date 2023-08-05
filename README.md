
FsHttp
======

FsHttp is a .Net HTTP client library for C# and F#. It aims for describing and executing HTTP requests in convenient ways that can be used in production and interactive environments.

The design principle behind FsHttp is:

> Specify common HTTP requests in a most convenient and readable way, while still being able to access the underlying .Net Http representations for covering unusual cases.

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

For different upgrade paths, please read the [Migrations section](https://fsprojects.github.io/FsHttp/Migrations.html) in the docu.


Building
--------

**.Net SDK:**

You need to have the latest .Net SDK installed, which is specified in `./global.json`.

**Build Tasks**

There is a F# build script (`./build.fsx`) that can be used to perform several build tasks from command line.

For common tasks, there are powershell files located in the repo root:

* `./test.ps1`: Runs all tests (sources in `./src/Tests`).
  * You can pass args to this task (in general, to all tasks). E.g. for executing only some tests: `./test.ps1 --filter whatever`
* `./docu.ps1`: Rebuilds the FsHttp documentation site (sources in `./src/docs`; output goes to `./docs`).
* `./docu-watch.ps1`: Run it if you are working on the documentation sources, and want to see the result in a browser.
* `./publish.ps1`: Publishes all packages (FsHttp and it's integration packages for Newtonsoft and FSharp.Data) to NuGet.
  * Always have a look at `./src/Directory.Build.props` and keep the file up-to-date.

Credits
-------

* Parts of the code is taken from the [HTTP utilities of FSharp.Data](https://fsprojects.github.io/FSharp.Data/library/Http.html).
* Credits to all critics, supporters, contributors, promoters, users, and friends.

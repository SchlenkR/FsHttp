<p align="center">
    <img src='https://raw.githubusercontent.com/schlenkr/RestInPeace/master/docs/img/logo.png' alt='logo' width='300' />
</p>

RestInPeace is a .Net HTTP client library for C# and F#. It aims for describing and executing HTTP requests in convenient ways that can be used in production and interactive environments.

The design principle behind RestInPeace is:

> Specify common HTTP requests in a most convenient and readable way, while still being able to access the underlying .Net Http representations for covering unusual cases.

**RestInPeace** is developed and maintained by [@SchlenkR](https://github.com/schlenkr) and [@dawedawe](https://github.com/dawedawe). Feel free to leave us a message.

[![NuGet Badge](http://img.shields.io/nuget/v/RestInPeace.svg?style=flat)](https://www.nuget.org/packages/RestInPeace) ![build status](https://github.com/SchlenkR/RestInPeace/actions/workflows/push-master_pull-request.yml/badge.svg?event=push)


A Simple Example
----------------

An example in F#:

```fsharp
#r "nuget: RestInPeace"

open RestInPeace

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
#r "nuget: RestInPeace"

using RestInPeace.CSharp;

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


Documentation
-------------

* 📖 Please see [RestInPeace Documentation](https://schlenkr.github.io/RestInPeace) site for a detailed documentation.
* 🧪 In addition, have a look at the [Integration Tests](https://github.com/SchlenkR/RestInPeace/tree/master/src/Tests) that show various library details.


Release Notes / Migrating to new versions
---

* See https://www.nuget.org/packages/RestInPeace#release-body-tab
* For different upgrade paths, please read the [Migrations section](https://schlenkr.github.io/RestInPeace/Migrations.html) in the docu.


GitHub
-------------

Please see [RestInPeace on GitHub](https://github.com/SchlenkR/RestInPeace).


Building
--------

**.Net SDK:**

You need to have the latest .Net SDK installed, which is specified in `./global.json`.

**Build Tasks**

There is a F# build script (`./build.fsx`) that can be used to perform several build tasks from command line.

For common tasks, there are powershell files located in the repo root:

* `./test.ps1`: Runs all tests (sources in `./src/Tests`).
  * You can pass args to this task. E.g. for executing only some tests:
    `./test.ps1 --filter Name~'Response Decompression'`
* `./docu.ps1`: Rebuilds the RestInPeace documentation site (sources in `./src/docs`).
* `./docu-watch.ps1`: Run it if you are working on the documentation sources, and want to see the result in a browser.
* `./publish.ps1`: Publishes all packages (RestInPeace and it's integration packages for Newtonsoft and FSharp.Data) to NuGet.
  * Always have a look at `./src/Directory.Build.props` and keep the file up-to-date.


Credits
-------

* Parts of the code is taken from the [HTTP utilities of FSharp.Data](https://schlenkr.github.io/FSharp.Data/library/Http.html).
* Credits to all critics, supporters, contributors, promoters, users, and friends.

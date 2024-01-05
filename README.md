<p align="center">
    <img src='https://raw.githubusercontent.com/fsprojects/FsHttp/master/docs/img/logo_big.png' alt='logo' width='300' />
</p>

FsHttp ("Full Stack HTTP") is a "hackable HTTP client". It's purposes are:

* Use it as a replacement for `.http` files, *VSCode's REST client*, *Postman*, and other tools as an **interactive and programmable playground** for HTTP requests.
* Usable as a **production-ready HTTP client** for applications powered by .Net (C#, VB, F#).

The design principle behind FsHttp is to specify HTTP requests in a most convenient and readable way, while still being able to access the underlying Http representations for covering unusual cases. It's the best of both worlds: **Convenience and Flexibility**.

**FsHttp** is developed and maintained by [@SchlenkR](https://github.com/schlenkr) and [@dawedawe](https://github.com/dawedawe). Feel free to leave us a message.

[![NuGet Badge](http://img.shields.io/nuget/v/FsHttp.svg?style=flat)](https://www.nuget.org/packages/FsHttp) ![build status](https://github.com/fsprojects/FsHttp/actions/workflows/push-master_pull-request.yml/badge.svg?event=push)


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

using FsHttp;

await Http
    .Post("https://reqres.in/api/users")
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

* 📖 Please see [FsHttp Documentation](https://fsprojects.github.io/FsHttp) site for a detailed documentation.
* 🧪 In addition, have a look at the [Integration Tests](https://github.com/fsprojects/FsHttp/tree/master/src/Tests) that show various library details.


Release Notes / Migrating to new versions
---

* See https://www.nuget.org/packages/FsHttp#release-body-tab
* For different upgrade paths, please read the [Migrations section](https://schlenkr.github.io/FsHttp/Migrations.html) in the docu.


GitHub
-------------

Please see [FsHttp on GitHub](https://github.com/fsprojects/FsHttp).


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
* `./docu.ps1`: Rebuilds the FsHttp documentation site (sources in `./src/docs`).
* `./docu-watch.ps1`: Run it if you are working on the documentation sources, and want to see the result in a browser.
* `./publish.ps1`: Publishes all packages (FsHttp and it's integration packages for Newtonsoft and FSharp.Data) to NuGet.
  * Always have a look at `./src/Directory.Build.props` and keep the file up-to-date.


Credits
-------

* Parts of the code is taken from the [HTTP utilities of FSharp.Data](https://schlenkr.github.io/FSharp.Data/library/Http.html).
* Credits to all critics, supporters, contributors, promoters, users, and friends.

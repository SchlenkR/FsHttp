---
title: FsHttp Overview
index: 1
---


FsHttp
======

FsHttp is a .Net HTTP client library for C# and F#. It aims for describing and executing HTTP requests in convenient ways that can be used in production and interactive environments.

The design principle behind FsHttp is:

> Specify common HTTP requests in a most convenient and readable way, while still being able to access the underlying .Net Http representations for covering unusual cases.

**FsHttp** is developed and maintained by [@ronaldschlenker](https://github.com/ronaldschlenker) and [@dawedawe](https://github.com/dawedawe). Feel free to leave us a message.


Sponsoring
----------

Want to help keep FsHttp alive? Then help keep F# and its ecosystem alive by supporting one of the following developers:

* [@edgarfgp](https://github.com/sponsors/edgarfgp). Why? E.g. for "Amplifying F#", for bringing people together, for his support and his work in the F# community.
* [@TheAngryByrd](https://github.com/sponsors/TheAngryByrd). Why? E.g. for maintaining Ionidem and many, many more.
* [@AngelMunoz](https://github.com/sponsors/AngelMunoz). Why? E.g. for his work in Fable (The F# to JS compiler), his passion and support.
* [@dawedawe](https://github.com/sponsors/dawedawe) and [@nojaf](https://github.com/sponsors/nojaf). Why? E.g. for Fantomas and their support in the F# community.
* [@PawelStadnicki](https://github.com/sponsors/PawelStadnicki). Why? E.g. for his attempt to push F# forward in data science.

For sure, there are many more. If you think someone should appear on that list, leave us a message!

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


Further documentation
---

Have a look at the [Integration Tests](https://github.com/fsprojects/FsHttp/tree/master/src/Tests) that show various library details.


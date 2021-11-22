---
title: FsHttp Overview
index: 1
---

# FsHttp

FsHttp is HTTP client library written in F#. It aims for describing and executing HTTP requests in a convenient ways that can be used in production and interactive environments.

[![NuGet Badge](http://img.shields.io/nuget/v/FsHttp.svg?style=flat)](https://www.nuget.org/packages/FsHttp) ![build status](https://github.com/fsprojects/FsHttp/actions/workflows/push-master_pull-request.yml/badge.svg?event=push)

**NOTE**: The NuGet package [SchlenkR.FsHttp](https://www.nuget.org/packages/SchlenkR.FsHttp) is **deprecated**. Please use the package [FsHttp](https://www.nuget.org/packages/FsHttp) for releases >= 6.x


NuGet
-----

Install FsHttp via NuGet command line:

```
PM> Install-Package FsHttp
```

or via F# Interactive:

```fsharp
#r "nuget: FsHttp"
```


A simple request
----------------

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

---
title: FsHttp Overview
_category: Some Category
_categoryindex: 2
index: 1
---

# FsHttp

FsHttp is an F# / .Net HTTP client library. It aims for describing and executing HTTP 
requests in idiomatic and convenient ways that can be used for production, tests and 
FSI (F# Interactive).

[![NuGet Badge](http://img.shields.io/nuget/v/SchlenkR.FsHttp.svg?style=flat)](https://www.nuget.org/packages/SchlenkR.FsHttp) [![Build Status](https://travis-ci.org/ronaldschlenker/FsHttp.svg?branch=master)](https://travis-ci.org/ronaldschlenker/FsHttp)


NuGet
-----

Install FsHttp via NuGet command line:

```
PM> Install-Package SchlenkR.FsHttp
```

or via F# Interactive:

```fsharp
#r "nuget: SchlenkR.FsHttp"
```


[Nuget SchlenkR.FsHttp](https://www.nuget.org/packages/SchlenkR.FsHttp)

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

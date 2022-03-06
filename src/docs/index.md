---
title: FsHttp Overview
index: 1
---

# FsHttp

FsHttp is a HTTP client library written in F#. It aims for describing and executing HTTP requests in a convenient ways that can be used in production and interactive environments.

[![NuGet Badge](http://img.shields.io/nuget/v/FsHttp.svg?style=flat)](https://www.nuget.org/packages/FsHttp) ![build status](https://github.com/fsprojects/FsHttp/actions/workflows/push-master_pull-request.yml/badge.svg?event=push)


Usage
---

```fsharp
#r "nuget: FsHttp"

let postResponse =
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
    |> Response.toJson


```

Further documentation
---

Have a look at the [Integration Tests](src/Tests) that show various library details.

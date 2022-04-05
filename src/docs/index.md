---
title: FsHttp Overview
index: 1
---

# FsHttp

FsHttp is a .Net HTTP client library for C# and F#. It aims for describing and executing HTTP requests in convenient ways that can be used in production and interactive environments.

**FsHttp** is authored by [@ronaldschlenker](https://github.com/ronaldschlenker). Feel free to leave a message.

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


Further documentation
---

Have a look at the [Integration Tests](https://github.com/fsprojects/FsHttp/tree/master/src/Tests) that show various library details.

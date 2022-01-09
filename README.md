
FsHttp
======

FsHttp is a HTTP client library written in F#. It aims for describing and executing HTTP requests in convenient ways that can be used in production and interactive environments.

**FsHttp** is authored by [@ronaldschlenker](https://github.com/ronaldschlenker). Feel free to leave a message.

[![NuGet Badge](http://img.shields.io/nuget/v/FsHttp.svg?style=flat)](https://www.nuget.org/packages/FsHttp) ![build status](https://github.com/fsprojects/FsHttp/actions/workflows/push-master_pull-request.yml/badge.svg?event=push)

**NOTE**: The NuGet package [SchlenkR.FsHttp](https://www.nuget.org/packages/SchlenkR.FsHttp) is **deprecated**. Please use the package [FsHttp](https://www.nuget.org/packages/FsHttp) for releases >= 6.x

Documentation
-------------

Please visit the [FsHttp Documentation](https://fsprojects.github.io/FsHttp) site.

You can also have a the [Integration Tests](src/Tests) that show various use cases.


A Simple Example
----------------

```fsharp

#r "nuget: FsHttp"

open FsHttp
open FsHttp.DslCE

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


Building
--------

You need to have the latest .Net 5 SDK installed.

There is a F# script that can be used to perform several build tasks from command line. It can be executed in this way:

`dotnet fsi .\build.fsx [task]`

Common tasks are:

* build
* test
* docu
* publish

### Building Readme.md

The `Readme.md` and the content in `./docs` is auto-generated from the files in `./src/Docu`. In order to build them, run the command `dotnet fsi .\build.fsx docu`.


Credits
-------

Parts of the code is taken from the [HTTP utilities of FSharp.Data](https://fsprojects.github.io/FSharp.Data/library/Http.html).

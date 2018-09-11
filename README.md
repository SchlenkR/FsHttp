# FsHttp
A lightweight F# HTTP library.

## Synopsis
This library provides a DSL which is inspired by the VS Code RestClient extension.

Parts of the code is taken from the [HTTP utilities of FSharp.Data](http://fsharp.github.io/FSharp.Data/library/Http.html).

## Examples

```fsharp
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> run
```

## Current State

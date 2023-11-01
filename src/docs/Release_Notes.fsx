(**
---
title: Release Notes / Migrations
category: Documentation
categoryindex: 1
index: 10
---
*)

(**
## Migrations

### To v9 (Breaking Changes)

* `http...` Builders: There is now only a single `http` builder, that is equivalent to the former `httpLazy` builder. To achieve the behaviour of the removed builders, please use:
* `httpLazy` -> `http { ... }`
* `http` -> `http { ... } |> Request.send`
* `httpAsync` -> `http { ... } |> Request.sendAsync`
* `httpLazyAsync` -> `http { ... } |> Request.toAsync`
* `httpMessage` -> `http { ... } |> Request.toMessage`
* see also: [https://github.com/fsprojects/FsHttp/blob/master/src/Tests/BuildersAndSignatures.fs](Tests in BuildersAndSignatures.fs)
* Renamed type `LazyHttpBuilder` -> `HttpBuilder`
* Renamed `Request.buildAsync` -> `Request.toAsync`
* Removed `send` and `sendAsync` builder methods
* Changed request and response printing (mostly used in FSI)
* Printing related custom operations change in names and behaviour
* `Dsl` / `DslCE` namespaces: There is no need for distinction of both namespaces. It is now sufficient to `open FsHttp` only.
* The `HttpBuilder<'context>` is replaced by `IBuilder<'self>`, so that the CE methods work directly on the `HeaderContext`, `BodyContext`, and `MultipartContext` directly. This simplifies things like mixing Dsl and DslCE, pre-configuring and chaining requests.
* The global configuration is now in the `FsHttp.GlobalConfig` module. The `Config` module is only for functions on request contexts.
* QueryParams is `(string * obj) list` now
* Use of System.Text.Json as a standard JSON library and created separate Newtonsoft and FSharp.Data JSON packages.
* Dropped support for .Net Standard 2.0
* Smaller breaking changes

### To > v10

* See [https://www.nuget.org/packages/FsHttp#release-body-tab](Directory.Build.props)

*)

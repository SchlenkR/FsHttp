(**
---
title: F# Interactive Usage
category: Documentation
categoryindex: 1
index: 9
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../src/FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp


(**
## FSI Setup
*)

#r @"nuget: FsHttp"
open FsHttp


(**
## FSI Request/Response Formatting

When you work in FSI, you can control the output formatting with special keywords.

Some predefined printers are defined in ```./src/FsHttp/DslCE.fs, module Fsi```

*)

http {
    GET "https://reqres.in/api/users"
    CacheControl "no-cache"
    print_withResponseBodyExpanded
}

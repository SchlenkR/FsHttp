(**
---
title: F# Interactive Usage
category: Documentation
categoryindex: 1
index: 7
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "nuget: FSharp.Data"
#r "../FsHttp/bin/Release/net6.0/FsHttp.dll"
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

2 most common printers are:

 - 'prv' (alias: 'preview'): This will render a small part of the response content.
 - 'exp' (alias: 'expand'): This will render the whole response content.
*)

http {
    GET "https://reqres.in/api/users"
    CacheControl "no-cache"
    print_withResponseBodyExpanded
}


// TODO: Top Level Ops
// TODO: Alternative STyle + Example

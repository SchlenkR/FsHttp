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
#r "../RestInPeace/bin/Release/net8.0/RestInPeace.dll"
open RestInPeace


(**
## FSI Setup
*)

#r @"nuget: RestInPeace"
open RestInPeace


(**
## FSI Request/Response Formatting

When you work in FSI, you can control the output formatting with special keywords.

Some predefined printers are defined in ```./src/RestInPeace/DslCE.fs, module Fsi```

*)

http {
    GET "https://reqres.in/api/users"
    CacheControl "no-cache"
    print_withResponseBodyExpanded
}

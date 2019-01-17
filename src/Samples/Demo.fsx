
#r @"../../packages/fsharp.data/lib/net45/FSharp.Data.dll"
#r @"../../packages/NUnit/lib/netstandard2.0/nunit.framework.dll"
#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp
open FsHttp.Dsl

get "http://www.google.de" .> preview

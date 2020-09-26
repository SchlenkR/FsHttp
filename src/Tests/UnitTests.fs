
#if INTERACTIVE
#r @"../../packages/fsharp.data/lib/net45/FSharp.Data.dll"
#r @"../../packages/NUnit/lib/netstandard2.0/nunit.framework.dll"
#r @"../../packages/fsunit/lib/netstandard2.0/FsUnit.NUnit.dll"
#r @"../../packages/Suave/lib/netstandard2.0\Suave.dll"
#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"
#r @"../FsHttp.NUnit/bin/Debug/netstandard2.0/FsHttp.NUnit.dll"
#load @"./Server.fs"
#else
module ``Unit tests for FsHttp``
#endif

open FsUnit
open FsHttp
open FsHttp.DslCE
open FsHttp.Testing
open NUnit.Framework
open Server
open System


// [<TestCase>]
let ``httpLazy and invocation signatures are correct``() =
    let request = httpLazy {
        GET "http://www.google.de"
    }

    let (response:Response) = request |> Request.send
    let (asyncResponse:Async<Response>) = request |> Request.sendAsync

    ()
    
// Sync / Async operator signatures test in DSL and CompExp style

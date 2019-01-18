
#r @"../../packages/fsharp.data/lib/net45/FSharp.Data.dll"
#r @"../../packages/NUnit/lib/netstandard2.0/nunit.framework.dll"
#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp
open FsHttp.Dsl


get "http://www.google.de"

async {
    let! response =
        post "https://reqres.in/api/users"
        --cacheControl "no-cache"
        --body
        --json """
        {
            "name": "morpheus",
            "job": "leader"
        }
        """
        >. go
    let content = response |> toText
    printfn "Content is: %s" content
}
|> Async.RunSynchronously


httpLazy {
    GET "http://www.google.de"
}
.> go

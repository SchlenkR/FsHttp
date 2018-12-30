
#r @"../../packages/fsharp.data/lib/net45/FSharp.Data.dll"
#r @"../../packages/NUnit/lib/netstandard2.0/nunit.framework.dll"
#r @"../../packages/fsunit/lib/netstandard2.0/FsUnit.NUnit.dll"
#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp
open FsUnit
open FSharp.Data
open FSharp.Data.JsonExtensions


http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}


http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
||> (fun json -> json?data.AsArray() |> should haveLength 3)
|> jsonShouldLookLike IgnoreOrder Subset
    """
    {
        "data": [
            {
                "id": 4
            }
        ]
    }
    """

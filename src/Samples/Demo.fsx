
/// -------
/// From the repo root, run "fake build" before using this demo script.
/// -------

#r @"../../packages/fsharp.data/lib/net45/FSharp.Data.dll"
#r @"../../packages/NUnit/lib/netstandard2.0/nunit.framework.dll"
#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp
open FsHttp.Dsl


// evaluate until here and play with the samples.

//////////////////////////////////////////////////////
let req = get "http://www.google.de" .> go
let html = req |> toText


//////////////////////////////////////////////////////
// specify a timeout (should throw because it's very short)
get "http://www.google.de" --timeoutInSeconds 0.1 .> go


//////////////////////////////////////////////////////
// inspect a response's header
let r = get "https://reqres.in/api/users" .> go
r.content.Headers.ContentType.MediaType.Contains("application/json")


//////////////////////////////////////////////////////
// post some data
post "https://reqres.in/api/users"
--cacheControl "no-cache"
--body
--json """
{
    "name": "morpheus",
    "job": "leader"
}
"""
.> go


//////////////////////////////////////////////////////
// use in an async context (>. operator)
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

//////////////////////////////////////////////////////
// lazy evaluation is invoked synchronousely
let lazySyncBuilder =
    httpLazy {
        GET "http://www.google.de"
    }
    .> go

//////////////////////////////////////////////////////
let lazyAsyncBuilder =
    httpLazy {
        GET "http://www.google.de"
    }
    >. go

//////////////////////////////////////////////////////
// lazy evaluation is invoked synchronousely
let asyncBuilder =
    httpAsync {
        GET "http://www.google.de"
    }


//////////////////////////////////////////////////////
// change the global timeout for all requests
FsHttp.Config.setTimeout (System.TimeSpan.FromSeconds 15.0)


//////////////////////////////////////////////////////
// specify a timeout per request
http {
    GET "http://www.google.de"
    timeoutInSeconds 1.0
}

get "http://www.google.com"
--timeoutInSeconds 1.0
.> go


//////////////////////////////////////////////////////
// lazy + incocation functions
let request = httpLazy {
    GET "http://www.google.de"
}

let (response:Response) = request .> go
let (asyncResponse:Async<Response>) = request >. go


//////////////////////////////////////////////////////
// use %% operator
let google url = "http://www.google.de" </> url

get %% google "test"
--acceptLanguage "de-DE"
.> go


//////////////////////////////////////////////////////
// control the print style
get @"https://reqres.in/api/users?page=2&delay=3"
.> print (noRequest >> withResponseContentMaxLength 500)


//////////////////////////////////////////////////////
open FSharp.Data
open FSharp.Data.JsonExtensions

http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
|> fun json -> json?page.AsInteger()


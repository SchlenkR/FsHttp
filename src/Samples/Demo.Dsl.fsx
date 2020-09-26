
(* ----------------------------

    Here you find some asorted use cases using the operator-less syntax.
    For a more complete demo of FsHttp, please have a look at:
        ./Demo.DslCE.fsx

---------------------------- *)




#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp
open FsHttp.Dsl



post "https://reqres.in/api/users"
    |> cacheControl "no-cache"
    |> body
    |> json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
    |> go


post "https://reqres.in/api/users"
    |> H.cacheControl "no-cache"
    |> body
    |> B.json """ { } """
    |> go


get "https://reqres.in/api/users
        ?page=2
        //&skip=5
        &delay=3"
    |> go


let users =
    get "https://reqres.in/api/users?page=2"
    |> send
    |> toJson


// Default print options (don't print request; print response headers, a formatted preview of the content)
get @"https://reqres.in/api/users?page=2&delay=3" |> go

// Default print options (see above) + max. content length of 100
get @"https://reqres.in/api/users?page=2&delay=3" |> (show 100)

// Default print options (don't print request; print response headers, whole content formatted)
get @"https://reqres.in/api/users?page=2&delay=3" |> expand


post "https://reqres.in/api/users"
    |> cacheControl "no-cache"
    |> bearerAuth "lskdjfjseriuwu8u854"
    |> acceptLanguage "de"
    |> body
    |> json
    """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
    |> exp


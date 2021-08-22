
(* ----------------------------

    Here you find some asorted use cases using the operator-less syntax.
    For a more complete demo of FsHttp, please have a look at:
        ./Demo.DslCE.fsx

---------------------------- *)




#r @"../FsHttp/bin/Debug/netstandard2.1/FsHttp.dll"

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
|> Request.send


post "https://reqres.in/api/users"
|> Header.cacheControl "no-cache"
|> body
|> Body.json """ { } """
|> Request.send


get "https://reqres.in/api/users
        ?page=2
        //&skip=5
        &delay=3"
|> Request.send


let users =
    get "https://reqres.in/api/users?page=2"
    |> Request.send
    |> Response.toJson


// Default print options (don't print request; print response headers, a formatted preview of the content)
get @"https://reqres.in/api/users?page=2&delay=3" |> Request.send

// Default print options (see above) + max. content length of 100
get @"https://reqres.in/api/users?page=2&delay=3" |> (show 100)

// Default print options (don't print request; print response headers, whole content formatted)
get @"https://reqres.in/api/users?page=2&delay=3" |> expand


post "https://reqres.in/api/users"
|> cacheControl "no-cache"
|> authorizationBearer "lskdjfjseriuwu8u854"
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

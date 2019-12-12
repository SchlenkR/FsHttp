
#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp
open FsHttp.Dsl



post "https://reqres.in/api/users"
    cacheControl "no-cache"
    body json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
    send

post "https://reqres.in/api/users"
    H.cacheControl "no-cache"
    body
    B.json """ { } """
    send

get "https://reqres.in/api/users
        ?page=2
        //&skip=5
        &delay=3"
    send


let users =
    get "https://reqres.in/api/users?page=2"
        send
    |> toJson


(***************
    FSI examples
****************)


// Default print options (don't print request; print response headers, a formatted preview of the content)
get @"https://reqres.in/api/users?page=2&delay=3" go

// Default print options (see above) + max. content length of 100
get @"https://reqres.in/api/users?page=2&delay=3" (show 100)

// Default print options (don't print request; print response headers, whole content formatted)
get @"https://reqres.in/api/users?page=2&delay=3" expand



get "http://www.google.de" go




post "https://reqres.in/api/users"
    cacheControl "no-cache"
    bearerAuth "lskdjfjseriuwu8u854"
    acceptLanguage "de"
    body json
    """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
    exp

// TODO: fin


#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"

module DslDemo =

    open FsHttp.Fsi
    open FsHttp.Dsl
    open FsHttp.Dsl.Fsi

    get "http://www.google.de"
        bearerAuth "4354terjkljgdlfkj"
        run go

    post
        "https://reqres.in/api/users"
        cacheControl "no-cache"
        body json
        """
        {
            "name": "morpheus",
            "job": "leader"
        }
        """
        run exp

    // TODO: fin

module DslCEDemo =

    open FsHttp
    open FsHttp.DslCE

    http {
        GET "http://www.google.de"
        BearerAuth "4354terjkljgdlfkj"
    }

    http {
        POST "https://reqres.in/api/users"
        CacheControl "no-cache"
        body
        json """
        {
            "name": "morpheus",
            "job": "leader"
        }
        """
    }

    // TODO: fin

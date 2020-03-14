
// Inside F# Interactive, load the FsHttp script instead of referencing the dll.
// This will register pretty output printers for HTTP requests and responses.
#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp

// Choose your style (here: Computation Expression)
open FsHttp.DslCE


// build up a GET request.
// The request will be sent immediately and synchronous.
http {
    GET "https://reqres.in/api/users"
}

// add a header...
http {
    GET "https://reqres.in/api/users"
    CacheControl "no-cache"
}

// When you work in FSI, you can control the output
// formatting with special keywords. Some predefined //
// printers are defined in './src/FsHttp/DslCE.fs, module Fsi':
// 2 most common printers are:
//   * 'go' (alias: 'preview'): This will render a small part of the response content.
//   * 'exp' (alias: 'expand'): This will render the whole response content.
http {
    GET "https://reqres.in/api/users"
    CacheControl "no-cache"
    exp
}


// Alternatively, you can write the verb first.
// Note that computation expressions must not be empty, so you
// have to write at lease something, like 'id', 'go', 'exp', etc.
// (have a look at: './src/FsHttp/DslCE.fs, module Shortcuts'.)
get "https://reqres.in/api/users" { exp }

// Inside the { }, you can place headers as usual...
get "https://reqres.in/api/users" {
    CacheControl "no-cache"
    exp
}

// You can also insert line breaks and F#/C# style line commenting:
get "https://reqres.in/api/users
            ?page=2
            //&skip=5
            &delay=3"
            { go }

// Here is an example of a POST with JSON as body.
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

// There are several ways transforming the content of the returned response to
// something like text or JSON:
// (have a look at: ./src/FsHttp/ResponseHandling.fs)
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
|> toJson

// works of cource also like this:
post "https://reqres.in/api/users" {
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}
|> toJson


// Use FSharp.Data.JsonExtensions to do JSON stuff:
open FSharp.Data
open FSharp.Data.JsonExtensions

http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
|> fun json -> json?page.AsInteger()



// You can specify a timeout (should throw because it's very short)
http {
    GET "http://www.google.de"
    timeoutInSeconds 0.1
}

// You can also set config values globally (inherited when requests are created):
FsHttp.Config.setTimeout (System.TimeSpan.FromSeconds 15.0)



// Transform underlying http client and do whatever you feel you gave to do:
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
    transformHttpClient (fun httpClient ->
        // this will cause a timeout exception
        httpClient.Timeout <- System.TimeSpan.FromMilliseconds 1.0
        httpClient)
}


// Transform underlying http request message
http {
    GET @"https://reqres.in/api/users?page=2&delay=3"
    transformHttpRequestMessage (fun msg ->
        printfn "HTTP message: %A" msg
        msg)
}


// There is not only the immediate + synchronous way of specifying requests.
// Have a look at: './src/FsHttp/DslCE.fs, module Fsi'.

// chaining builders together: First, use a httpLazy to create a 'HeaderContext'
// (Hint: "httpLazy { ... }" is just a shortcut for "httpRequest StartingContext { ... }")."
let postOnly =
    httpLazy {
        POST "https://reqres.in/api/users"
    }

// add some HTTP headers to the context
let postWithCacheControlBut =
    httpRequest postOnly {
        CacheControl "no-cache"
    }

// transform the HeaderContext to a BodyContext and add JSON content
let finalPostWithBody =
    httpRequest postWithCacheControlBut {
        body
        json """
        {
            "name": "morpheus",
            "job": "leader"
        }
        """
    }

// finally, send the request (sync or async)
let finalPostResponse = finalPostWithBody |> send
let finalPostResponseAsync = finalPostWithBody |> sendAsync




// HTTP in an async context:
let pageAsync =
    async {
        let! response = 
            httpAsync {
                GET "https://reqres.in/api/users?page=2&delay=3"
            }
        let page =
            response
            |> toJson
            |> fun json -> json?page.AsInteger()
        return page
    }




// TODO:
// * There are different types of builders (`http`, `httpAsync`, `httpLazy`, and `httpMsg`)

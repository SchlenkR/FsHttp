# FsHttp

A lightweight F# HTTP library.

## Synopsis

This library provides a convenient way of interacting with HTTP endpoints.

The focus of FsHttp is:

* Exploring HTTP services interactively by sending HTTP requests and viewing the response in F# interactive.
* Test web APIs by sending requests and assert expectations.

Parts of the code is taken from the [HTTP utilities of FSharp.Data](http://fsharp.github.io/FSharp.Data/library/Http.html).

## Examples

### F# Interactive Usage

Using FsHttp in F# interactive, you should load the 'FsHttp.fsx' instead of referencing the dll directly. This will enable pretty printing of a response in the FSI output.

```fsharp
#load "./bin/debug/netstandard2.0/FsHttp.fsx"

open FsHttp
```

### Sending Requests

A simple GET request looks like this:

```fsharp
http {  GET "https://reqres.in/api/users?page=2&delay=3"
}
|> send
```

You can split query parameters like this:

```fsharp
http {  GET "https://reqres.in/api/users
                ?page=2
                &delay=3"
}
|> send
```

You can set header parameters like this:

```fsharp
http {  GET @"http://www.google.com"
        AcceptLanguage "de-DE"
}
|> send
```

...or you can post data like this:

```fsharp
http {  POST @"https://reqres.in/api/users"
        CacheControl "no-cache"

        body
        json """
        {
            "name": "morpheus",
            "job": "leader"
        }
        """
}
|> send
```

## Testing Responses

TODO

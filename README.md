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

For using the JSON and testing functions, reference the FSharp.Data, NUnit and FSUnit libraries. Have a look at the setup shown in the **FsHttp.DevTest** folder for an example.

```fsharp
#r @".\packages\fsharp.data\lib\net45\FSharp.Data.dll"
#r @".\packages\NUnit\lib\netstandard2.0\nunit.framework.dll"
#r @".\packages\fsunit\lib\netstandard2.0\FsUnit.NUnit.dll"
#load @"../FsHttp/bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp
open FsUnit
open FSharp.Data
open FSharp.Data.JsonExtensions
```

### Basics

A simple GET request looks like this:

```fsharp
http {  GET "https://reqres.in/api/users?page=2&delay=3"
}
```

You can split query parameters like this:

```fsharp
http {  GET "https://reqres.in/api/users
                ?page=2
                &delay=3"
}
```

You can set header parameters like this:

```fsharp
http {  GET @"http://www.google.com"
        AcceptLanguage "de-DE"
}
```

Post data like this:

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
```

### Response Handling and Testing

Convert a response to a JsonValue:

```fsharp
http {  GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
```

Testing response data by asserting JSON expectations:

```fsharp
http {  GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
|> test
>>= expect *> fun json -> json?data.AsArray() |> should haveLength 3
|> run
```

Testing response data by asserting JSON expectations and example:

```fsharp
http {  GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> toJson
|> test
>>= expect *> fun json -> json?data.AsArray() |> should haveLength 3
>>= expectJsonByExample IgnoreIndexes Subset
    """
    {
        "data": [
            {
                "id": 4
            }
        ]
    }
    """
|> run
```

## Hints

The examples shown here use the **http** builder, which evaluates requests immediately and is executed synchronousely. There are more builders that can be used to achieve a different behavior:

* **http** Immediately evaluated, synchronous
* **httpAsync** Immediately evaluated, asynchronous
* **httpLazy** Lazy evaluated, synchronous
* **httpLazyAsync** Lazy evaluated, asynchronous

The inner DSL is the same for all builders.

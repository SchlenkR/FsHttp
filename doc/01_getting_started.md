
## Getting Started

Have a look at a simple use case using a POST and json as data. Each style is handles more detailed in the upcoming sections. All flavours are equivalent in functionality; they only differ in syntax.

**Important: The general use cases described here use the operator-less syntax, but the concepts work for the other flavours as well. Differences are handled in the upcoming sections of each flavour.**

See `src\Samples` folder for use cases.

### CE Flavour

```fsharp
http {
    POST "https://reqres.in/api/users"
    CacheControl "no-cache"
    body json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}
```

The CE flavour uses F# Computation Expressions to describe requests. Have a look at the [Sample Script for CE DSL](Samples/Demo.DslCE.fsx) for detailed use cases.

### Pipe Flavour

```fsharp
post "https://reqres.in/api/users"
--cacheControl "no-cache"
--body
--json """
{
    "name": "morpheus",
    "job": "leader"
}
"""
|> send
```

Have a look at the [Sample Script for Pipe DSL](Samples/Demo.DslPipe.fsx) for detailed use cases.

### Op-Less Flavour

```fsharp
post "https://reqres.in/api/users"
    cacheControl "no-cache"
    body json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
    send
```

Have a look at the [Sample Script for OP-less DSL](Samples/Demo.Dsl.fsx) for detailed use cases.

## Explicit Headers

When you want to have intellisense for the header values, you can use `H` for header and `B` for body (not for CE flavour):

```fsharp
post "https://reqres.in/api/users"
    H.cacheControl "no-cache"
    body
    B.json """ { "name": "morpheus" } """
    send
```

## URL Formatting

You can split URL query parameters or comment lines out by using F# line-comment syntax:

```fsharp
// &skip=5 won't be a part of the final url.
// Line breaks and trailing / leading spaces will be removed.
get "https://reqres.in/api/users
        ?page=2
        //&skip=5
        &delay=3"
    send
```

## Response Handling

There are several convenience functions for transforming responses. They can be found in `FsHttp.ResponseHandling` module. The source can be found here: [Response Handling](src/FsHttp/ResponseHandling.fs)

```fsharp
let users =
    get "https://reqres.in/api/users?page=2" send
    |> toJson
```


* send / sendAsync





* TODO: Config
* 
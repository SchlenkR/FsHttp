(**
---
title: Sending JSON
category: Documentation
categoryindex: 2
index: 3
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp



(**
## JSON String
*)
http {
    POST "https://mysite"

    body
    json """
    {
        "name" = "morpheus",
        "job" = "leader",
        "age": %d
    }
    """
}
|> Request.send


(**
**Parametrized JSON:**

When the JSON content needs to be parametrized, `sprintf` function is a useful tool. Compared to interpolated strings, the curly braces - which are a key character in JSON - don't have to be escaped:
*)
let sendRequestWithSprintf age =
    http {
        POST "https://mysite"

        body
        json (sprintf """
        {
            "name" = "morpheus",
            "job" = "leader",
            "age": %d
        }
        """ age)
    }
    |> Request.send


(**
**Using an interpolated string:**
*)
let sendRequestWithInterpolatedString age =
    http {
        POST "https://mysite"

        body
        json $"""
        {{
            "name" = "morpheus",
            "job" = "leader",
            "age": {age}
        }}
        """
    }
    |> Request.send

(**
## Sending records or objects as JSON
*)
http {
    POST "https://mysite"

    body
    jsonSerialize
        {|
            name = "morpheus"
            job = "leader"
        |}
}
|> Request.send

(**
> It is also possible to pass serializer settings using the `jsonSerializeWith` operation.
*)
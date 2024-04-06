(**
---
title: Sending Requests
category: Documentation
categoryindex: 2
index: 7
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../src/FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp
open System.Threading



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


(**
## Plain text
*)
http {
    POST "https://mysite"
    body
    // Sets Content-Type: plain/text header
    text """
The last train is nearly due
The underground is closing soon
And in the dark deserted station
Restless in anticipation
A man waits in the shadows
"""
}
|> Request.send

(**
## Request Cancellation

It is possible to bind a cancellation token to a request definition,
that will be used for the underlying HTTP request:
*)

use cts = new CancellationTokenSource()

// ...

http {
    GET "https://mysite"
    config_cancellationToken cts.Token
}
|> Request.send


(**
See also: https://github.com/fsprojects/FsHttp/issues/105

Instead of binding a cancellation token directly to a request definition (like in the example above),
it is also possible to pass it on execution-timer, like so:
*)

http {
    GET "https://mysite"
}
|> Config.cancellationToken cts.Token
|> Request.send


- Integration packages - Newtonsoft, FsharpData


Making Requests
- Sending
- Transforming

Configuration
- Configuration

JSON
- System.Text.JSON
	- ? Operator: Working with Json
- FsHttp.FSharpData package

CSharp

FSI:
- % Operator

Async
---
Async.await / Async.map

File Upload

* Referenzen zu Tests in jeder Datei, wo es sich anbietet



(**
An alternative way: HTTP method-first functions
*)
get "https://mysite" {
    AcceptLanguage "en-US"
}
|> Request.sendAsync




(**
Working in F# Interactive or notebooks, a short form for sending requests can be used: The `%` operator.

> Note: Since the `%` operator send a synchronous request (blocking the caller thread),
> it is only recommended for using in an interactive environment.
*)
open FsHttp.Operators

% http {
    GET "https://mysite"
    AcceptLanguage "en-US"
}








expect / assert


Async: 
	diff betweet send and to
	task "...TAsync functions"
	pipeline style (await / map) und CE-style




(**
Process response content as JSON:

Hint:
* HOT:  'sendAsync' sends the request immediately.
* COLD: 'toAsync' builds an async request that needs to be started.

*)

let asyncResponse =
    // Assume this returns: { "name": "Paul"; "age": 54 }
    http {
        GET "https://mysite"
        AcceptLanguage "en-US"
    }
    |> Request.sendAsync
    |> Async.await Response.toJsonAsync
    |> Async.map (fun json -> json?name.GetString(), json?age.GetInt32())


DSL Syntax (Pipeline / mixing this)




* Per-defining buidlers


// A configuration per request
http {
    config_timeoutInSeconds 10.0
}


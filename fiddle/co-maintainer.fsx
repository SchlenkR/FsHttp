
#r "../src/FsHttp/bin/debug/net7.0/FsHttp.dll"
#r "nuget: FsHttp"

open System
open System.Net.Http
open FsHttp
open FsHttp.Operators

do Fsi.disableDebugLogs()

http {
    GET "https://www.wikipedia.de"
    Authorization c
}
|> Request.sendAsync



get "https://www.wikipedia.de"
|> Header.authorization "https://www.wikipedia.de"


get "https://www.wikipedia.de" {
    Authorization c
}
|> Header.acceptCharset "whatever"


// -----------------
// F# Interactive
// -----------------

open FsHttp.Operators

% get "https://www.wikipedia.de" {
    AcceptCharset ""
}

get "https://www.wikipedia.de" {
    AcceptCharset ""
}
|> Request.send



// -----------------
// Composability
// -----------------

let httpLongRunning = 
    http {
        config_timeoutInSeconds 100.0
    }

let waitLongForGoogle =
    httpLongRunning {
        GET "https://www.google.de"
    }

waitLongForGoogle |> Request.send

// -----------------
// Use Cases
// -----------------

let makeEnv serverName (method: string -> HeaderContext) urlSuffix =
    method (serverName </> urlSuffix)

let env1 = makeEnv "http://localhost/myService"
let env2 = makeEnv "https://www.google.de"


% env2 get "xxx" {
    AcceptCharset ""
}



http {
    // IStartingContext ...
    
    GET "http://www.google.de"
    // IHeaderContext ...

    AcceptCharset ""
    
    body
    // IBodyContext ...

    json """ [1] """
    // IFinalContext (has no operations defined at all)
}


http {
    GET "http://...."
}
|> Request.toHttpRequestMessage


|> Request.send
|> Response.toJsonDocument

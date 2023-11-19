(**
---
title: Request Headers
category: Documentation
categoryindex: 1
index: 4
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../src/FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp


(**
## Specifying Standard and Custom Headers

* The standard headers can be specified using the corresponding custom operation names.
    * Example: `AuthorizationBearer` (a shortcur for the "Authorization" header with the "Bearer" scheme)
    * Example: `Accept`
    * Example: `UserAgent`
* Custom headers can be specified using the `header` function.
    * Example: `header "X-GitHub-Api-Version" "2022-11-28"`

Here's an example that fetches the issues of the vide-collabo/vide repository on GitHub:
*)

http {
    GET "https://api.github.com/repos/vide-collabo/vide/issues"

    AuthorizationBearer "**************"
    Accept "application/vnd.github.v3+json"
    UserAgent "FsHttp"
    header "X-GitHub-Api-Version" "2022-11-28"
}
|> Request.send

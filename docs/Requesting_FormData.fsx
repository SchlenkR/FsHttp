(**
---
title: POST Form Data
category: Documentation
categoryindex: 2
index: 6
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../src/RestInPeace/bin/Release/net8.0/RestInPeace.dll"
open RestInPeace



(**
## Sending URL-Encoded Form
*)
http {
    POST "https://mysite"
    body
    formUrlEncoded [
        "key_1", "Data 1"
        "key_2", "Data 2"
    ]
}
|> Request.send

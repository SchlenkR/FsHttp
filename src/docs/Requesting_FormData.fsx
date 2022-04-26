(**
---
title: POST Form Data
category: Documentation
categoryindex: 2
index: 5
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp



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

(**
## Further Readings

> Have a look at the [https://github.com/fsprojects/FsHttp/blob/master/src/Tests/Multipart.fs](multipart tests) for more examples using multipart.
*)
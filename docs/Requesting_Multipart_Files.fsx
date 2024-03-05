(**
---
title: Multipart and File Upload
category: Documentation
categoryindex: 2
index: 5
---
*)

(*** condition: prepare ***)
#nowarn "211"
#r "../src/FsHttp/bin/Release/net6.0/FsHttp.dll"
open FsHttp



(**
## Sending Multipart Form-Data

**Performing a POST multipart request / uploading a file:**
*)
http {
    POST "https://mysite"

    // use "multipart" keyword (instead of 'body') to start specifying parts
    multipart
    textPart "the-actual-value_1" "the-part-name_1"
    textPart "the-actual-value_2" "the-part-name_2"
    filePart "super.txt" "F# rocks!"
}
|> Request.send

(**
## Further Readings

> Have a look at the [https://github.com/fsprojects/FsHttp/blob/master/src/Tests/Multipart.fs](multipart tests) for more examples using multipart.
*)
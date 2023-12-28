(*
shell> dotnet fsi .\issue-109-114-success.fsx
*)

#r "nuget: FsHttp, 10.0.0-preview2"

open FsHttp

let req = http {
    GET "http://google.com"
}

let res = Request.send req

printfn $"res = %A{res}"

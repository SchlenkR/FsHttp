(*
shell> dotnet fsi .\issue-109-114-success.fsx
*)

//#r "nuget: FsHttp, 10.0.0-preview1"
#r @"C:\Users\ronal\source\repos\github\FsHttp\src\FsHttp\bin\Debug\net6.0\FsHttp.dll"

open FsHttp

let req = http {
    GET "http://google.com"
}

let res = Request.send req

printfn $"res = %A{res}"

#r "../src/FsHttp/bin/debug/net7.0/FsHttp.dll"

open System
open FsHttp

type ArgA =
    | FileName of string
    | Path of string

type ArgB =
    | FileName of string
    | AnotherPath of string

type IRequestContext<'self> with
    [<CustomOperation("a")>]
    member _.A(context: IRequestContext<MultipartContext>, name, [<ParamArray>] argA: ArgA array) : MultipartContext =
        failwith "TODO: implement me"
    
    [<CustomOperation("b")>]
    member _.B(context: IRequestContext<MultipartContext>, name, [<ParamArray>] argA: ArgB array) : MultipartContext =
        failwith "TODO: implement me"

http {
    POST("")
    multipart

    a "Resources/uploadFile.txt" (FileName "xsycy") (Path "xsycy")
}

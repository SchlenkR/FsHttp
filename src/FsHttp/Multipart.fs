
[<AutoOpen>]
module FsHttp.Multipart

open System
open System.Net.Http

type MultipartContent =
    { files: string list }

type MultipartBuilder() =
    member this.Zero() = { files = [] }
    member this.Yield(x) = this.Zero()

    [<CustomOperation("file")>]
    member this.File(context: MultipartContent, fileName: string) =
        { files = context.files @ [fileName] }

let multipart = MultipartBuilder()


let res1 = multipart {
    file "a"
    file "b"
}

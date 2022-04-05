namespace FsHttp

[<AutoOpen>]
module Json =
    open System.Text.Json

    let (?) (json: JsonElement) (key: string) : JsonElement = json.GetProperty(key)

module Operators =
    let ( </> ) = Helper.Url.combine
    let ( ~% ) = Request.send

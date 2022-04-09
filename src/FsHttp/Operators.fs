namespace FsHttp

open System.Text.Json

[<AutoOpen>]
module JsonDynamic =
    let (?) (json: JsonElement) (key: string) : JsonElement = json.GetProperty(key)

module Operators =
    let ( </> ) = Helper.Url.combine
    let ( ~% ) = Request.send

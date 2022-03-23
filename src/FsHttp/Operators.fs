namespace FsHttp

[<AutoOpen>]
module Json =
    open System.Text.Json

    let (?) (json: JsonElement) (key: string) : JsonElement = json.GetProperty(key)

module Operators =
    open FsHttp.Helper

    let (</>) = Url.combine

    // TODO: Document this
    type Kickoff = Kickoff with
        static member inline ($) (Kickoff, x: FsHttp.Domain.IToRequest) =
            x |> Request.send
    let inline kickoff x = (($) Kickoff) x
    let inline (~%) x = kickoff x

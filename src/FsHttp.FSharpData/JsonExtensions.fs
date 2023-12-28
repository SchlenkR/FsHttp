namespace FsHttp.FSharpData

open FSharp.Data

[<AutoOpen>]
module JsonExtensions =
    type JsonValue with
        member this.HasProperty(propertyName: string) =
            let prop = this.TryGetProperty propertyName

            match prop with
            | Some _ -> true
            | None -> false

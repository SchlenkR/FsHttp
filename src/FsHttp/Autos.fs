namespace FsHttp

open System.Text.Json

[<AutoOpen>]
module SystemTextJsonExtensions =
    type JsonElement with
        member this.ObjValue: obj =
            let fromTry f =
                let succ, value = f ()
                if succ then Some(value :> obj) else None

            match this.ValueKind with
            | JsonValueKind.True -> true
            | JsonValueKind.False -> false
            | JsonValueKind.Number ->
                fromTry this.TryGetInt32
                |> Option.orElse (fromTry this.TryGetInt64)
                |> Option.orElse (fromTry this.TryGetDouble)
                |> Option.defaultValue ""
            | JsonValueKind.Array -> this.EnumerateArray()
            | JsonValueKind.Null -> null
            | _ -> this.ToString()

    type JsonProperty with
        member this.ObjValue = this.Value.ObjValue

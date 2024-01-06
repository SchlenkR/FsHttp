namespace FsHttp

open System.Text.Json
open System.Runtime.CompilerServices

[<AutoOpen>]
module SystemTextJsonExtensions =
    
    [<Extension>]
    type JsonElementExtensions =

        // Is that a good thing? I don't know... Maybe not.
        [<Extension>]
        static member ObjValue(this: JsonElement) : obj =
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
            | JsonValueKind.Array ->
                this.EnumerateArray()
                |> Seq.toList
                :> obj
            | JsonValueKind.Null -> null
            | _ -> this.ToString()
        
        [<Extension>]
        static member GetListOf<'a>(this: JsonElement) =
            this.EnumerateArray()
            |> Seq.map (fun x -> JsonElementExtensions.ObjValue(x) :?> 'a)
            |> Seq.toList
        
        [<Extension>]
        static member GetList(this: JsonElement) =
            this.EnumerateArray()
            |> Seq.toList

    [<Extension>]
    type JsonPropertyExtensions =
        
        [<Extension>]
        static member ObjValue(this: JsonProperty) =
            JsonElementExtensions.ObjValue(this.Value)

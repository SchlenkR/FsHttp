[<AutoOpen>]
module FsHttp.DomainExtensions

open System
open System.Net.Http.Headers
open FsHttp

type FsHttpTarget with
    member this.ToUriStringWithDefault(defaultValue) =
        let queryParamsString =
            this.additionalQueryParams
            |> Seq.map (fun (k, v) -> $"""{k}={Uri.EscapeDataString $"{v}"}""")
            |> String.concat "&"

        match this.address with
        | None -> defaultValue
        | Some address -> 
            let uri = UriBuilder(address)
            uri.Query <-
                match uri.Query, queryParamsString with
                | "", "" -> ""
                | s, "" -> s
                | "", q -> $"?{q}"
                | s, q -> $"{s}&{q}"
            uri.ToString()

type ContentType with
    member this.ToMediaHeaderValue() =
        let mhv = MediaTypeHeaderValue.Parse(this.value)
        do this.charset |> Option.iter (fun charset -> mhv.CharSet <- charset.WebName)
        mhv

[<AutoOpen>]
module FsHttp.DomainExtensions

open System.Net.Http.Headers
open FsHttp

type ContentType with
    member this.ToMediaHeaderValue() =
        let mhv = MediaTypeHeaderValue.Parse(this.value)
        do this.charset |> Option.iter (fun charset -> mhv.CharSet <- charset.WebName)
        mhv

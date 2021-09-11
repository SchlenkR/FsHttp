module FsHttp.Operators

open FsHttp.Helper

let (</>) = Url.combine

type Kickoff = Kickoff with
    static member inline ($) (Kickoff, x: FsHttp.DslCE.LazyHttpBuilder<'context>) =
        x |> Request.send
    static member inline ($) (Kickoff, x: FsHttp.Domain.IContext) =
        x |> Request.send
let inline kickoff x = (($) Kickoff) x
let inline (~%) x = kickoff x

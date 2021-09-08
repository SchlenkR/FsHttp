namespace FsHttp.Helper


module String =

    open System
    open System.Text

    let urlEncode (s: string) = System.Web.HttpUtility.UrlEncode(s)

    let private base64Encoding = Encoding.GetEncoding("ISO-8859-1")

    let toBase64 (s: string) =
        s
        |> base64Encoding.GetBytes
        |> Convert.ToBase64String

    let fromBase64 (s: string) =
        s
        |> Convert.FromBase64String
        |> base64Encoding.GetString

    let substring (s:string) maxLength = string(s.Substring(0, Math.Min(maxLength, s.Length)))

module Map =
    let union (m1: Map<'k, 'v>) (s: seq<'k * 'v>) =
        seq {
            yield! m1 |> Seq.map (fun kvp -> kvp.Key, kvp.Value)
            yield! s
        }
        |> Map.ofSeq


module Url =
    open System

    let internal urlCombine (url1:string) (url2:string) =
        (url1.TrimEnd [|'/'|]) + "/" + (url2.TrimStart [|'/'|])


// TODO: Maybe move this and merge with other top level ops
[<AutoOpen>]
module TopLevelOperators =
    let (</>) = Url.urlCombine

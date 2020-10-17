namespace FsHttp

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


module Uri =

    open System

    let encodeUrlParam (param: string) =
        (Uri.EscapeUriString param).Replace("&", "%26").Replace("#", "%23")
            
    let appendQueryToUrl queryParams (url: string) =
        match queryParams with
        | [] -> url
        | query ->
            url
            + if url.Contains "?" then "&" else "?"
            + String.concat "&" (query |> List.map (fun (k, v) -> encodeUrlParam k + "=" + encodeUrlParam v))

    // TODO: Test
    let urlCombine (url1:string) (url2:string) =
        (url1.TrimEnd [|'/'|]) + "/" + (url2.TrimStart [|'/'|])

[<AutoOpen>]
module TopLevelOperators =
    let (</>) = Uri.urlCombine

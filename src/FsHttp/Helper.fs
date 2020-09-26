namespace FsHttp

[<RequireQualifiedAccess>]
module String =

    open System
    open System.Text

    let urlEncode (s: string) = System.Web.HttpUtility.UrlEncode(s)

    let toBase64 (s: string) =
        let utf8Bytes = Encoding.UTF8.GetBytes(s)
        Convert.ToBase64String(utf8Bytes)

    let fromBase64 (s: string) =
        let base64Bytes = Convert.FromBase64String(s)
        Encoding.UTF8.GetString(base64Bytes)

    let substring (s:string) maxLength = string(s.Substring(0, Math.Min(maxLength, s.Length)))


[<RequireQualifiedAccess>]
module Helper =

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
    let (</>) = urlCombine

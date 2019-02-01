
namespace FsHttp

[<AutoOpen>]
module Helper =

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

    // TODO: Test
    let combine (url1:string) (url2:string) =
        (url1.TrimEnd [|'/'|]) + "/" + (url2.TrimStart [|'/'|])
    let (</>) = combine
    
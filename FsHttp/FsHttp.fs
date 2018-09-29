
namespace FsHttp

[<AutoOpen>]
module Core =

    open System
    open System.Net.Http
    open System.Text

    [<AutoOpen>]
    module Helper =

        let urlEncode (s: string) = System.Web.HttpUtility.UrlEncode(s)

        let toBase64 (s: string) =
            let utf8Bytes = Encoding.UTF8.GetBytes(s)
            Convert.ToBase64String(utf8Bytes)

        let fromBase64 (s: string) =
            let base64Bytes = Convert.FromBase64String(s)
            Encoding.UTF8.GetString(base64Bytes)

        let formatUrl (url: string) =
            let segments =
                url.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries)
                |> Seq.map (fun x -> x.Trim())
                |> Seq.filter (fun x -> not (x.StartsWith("//")))
                // TODO
                //|> Seq.map (fun x ->
                //    if x.StartsWith("?") || x.StartsWith("&")
                //    then x.Substring(1)
                //    else x
                //)
                |> Seq.reduce (+)
            segments

    [<AutoOpen>]
    module Domain =

        type Header = {
            url: string;
            method: HttpMethod;
            headers: (string*string) list;
        }

        type Content = {
            content: string;
            contentType: string;
            headers: (string*string) list;
        } 

        type StartingContext = StartingContext

        type FinalContext = {
            request: Header;
            content: Content option;
        }

        type HeaderContext = { request: Header } with
            static member header (this: HeaderContext, name: string, value: string) =
                { this with request = { this.request with headers = this.request.headers @ [name,value] } }
            static member finalize (this: HeaderContext) =
                let finalContext = { request=this.request; content=None }
                finalContext

        type BodyContext = {
            request: Header;
            content: Content;
        } with
            static member header (this: BodyContext, name: string, value: string) =
                { this with request = { this.request with headers = this.request.headers @ [name,value] } }
            static member finalize (this: BodyContext) =
                let finalContext:FinalContext = { request=this.request; content=Some this.content }
                finalContext

    type HttpBuilder() =
        member this.Bind(m, f) = f m
        member this.Return(x) = x
        member this.Yield(x) = StartingContext
        member this.For(m, f) = this.Bind m f

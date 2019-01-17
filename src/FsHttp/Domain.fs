
namespace FsHttp

open System.Net.Http

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
        static member Header (this: HeaderContext, name: string, value: string) =
            { this with request = { this.request with headers = this.request.headers @ [name,value] } }
        static member Finalize (this: HeaderContext) =
            let finalContext = { request=this.request; content=None }
            finalContext

    type BodyContext = { request: Header; content: Content; } with
        static member Header (this: BodyContext, name: string, value: string) =
            { this with request = { this.request with headers = this.request.headers @ [name,value] } }
        static member Finalize (this: BodyContext) =
            let finalContext:FinalContext = { request=this.request; content=Some this.content }
            finalContext

    type HttpBuilder() = class end


namespace FsHttp

open System
open System.Net.Http

[<AutoOpen>]
module Domain =

    type Config = {
        timeout: TimeSpan
        httpMessageTransformer: (HttpRequestMessage -> HttpRequestMessage) option
        httpClientTransformer: (HttpClient -> HttpClient) option
    }

    type Header = {
        url: string
        method: HttpMethod
        headers: (string*string) list
    }

    type Content = {
        content: string
        contentType: string
        headers: (string*string) list
    } 

    type StartingContext = StartingContext

    type FinalContext = {
        request: Header
        content: Content option
        config: Config
    }

    type HeaderContext =
        { request: Header
          config: Config } with
        static member Header (this: HeaderContext, name: string, value: string) =
            { this with request = { this.request with headers = this.request.headers @ [name,value] } }
        static member Config (this: HeaderContext, f: Config -> Config) =
            { this with config = f this.config }
        static member Finalize (this: HeaderContext) =
            let finalContext = { request=this.request; content=None; config=this.config }
            finalContext

    type BodyContext =
        { request: Header
          content: Content
          config: Config } with
        static member Header (this: BodyContext, name: string, value: string) =
            { this with request = { this.request with headers = this.request.headers @ [name,value] } }
        static member Config (this: BodyContext, f: Config -> Config) =
            { this with config = f this.config }
        static member Finalize (this: BodyContext) =
            let finalContext:FinalContext = { request=this.request; content=Some this.content; config=this.config }
            finalContext

    // TODO: Get rid of all the boolean switches and use options instead.
    type Response = {
        requestContext: FinalContext
        requestMessage: HttpRequestMessage
        content: HttpContent
        headers: Headers.HttpResponseHeaders
        reasonPhrase: string
        statusCode: System.Net.HttpStatusCode
        version: Version
        printHint: PrintHint
    }
    and PrintHint = {
        isEnabled: bool
        requestPrintHint: RequestPrintHint
        responsePrintHint: ResponsePrintHint
    }
    and RequestPrintHint = {
        enabled: bool
        printHeader: bool
    }
    and ResponsePrintHint = {
        enabled: bool
        printHeader: bool
        printContent: ContentPrintHint
    }
    and ContentPrintHint = {
        enabled: bool
        format: bool
        maxLength: int
    }

    type HttpBuilder() = class end

    let defaultPrintHint = 
        {
            isEnabled = true
            requestPrintHint = {
                enabled = true
                printHeader = true
            }
            responsePrintHint = {
                enabled = true
                printHeader = true
                printContent = {
                    enabled = false
                    format = true
                    // TODO: provide default in config
                    maxLength = 500
                }
            }
        }

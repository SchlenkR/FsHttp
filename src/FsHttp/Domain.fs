
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
        // We use a .Net type here, which we never do in other places.
        // Since Cookie is record style, I see no problem here.
        cookies: System.Net.Cookie list
    }

    type Content = {
        content: string
        contentType: string
        headers: (string*string) list
    } 

    type StartingContext = StartingContext

    type FinalContext =
        { header: Header
          content: Content option
          config: Config } with
        // important because we can use sendFinal with all context types
        static member Finalize (this: FinalContext) = this

    type HeaderContext =
        { header: Header
          config: Config } with
        static member Header (this: HeaderContext, name: string, value: string) =
            { this with header = { this.header with headers = this.header.headers @ [name,value] } }
        static member Config (this: HeaderContext, f: Config -> Config) =
            { this with config = f this.config }
        static member Finalize (this: HeaderContext) =
            let finalContext = { header=this.header; content=None; config=this.config }
            finalContext

    type BodyContext =
        { header: Header
          content: Content
          config: Config } with
        static member Header (this: BodyContext, name: string, value: string) =
            { this with header = { this.header with headers = this.header.headers @ [name,value] } }
        static member Config (this: BodyContext, f: Config -> Config) =
            { this with config = f this.config }
        static member Finalize (this: BodyContext) =
            let finalContext:FinalContext = { header=this.header; content=Some this.content; config=this.config }
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
                    maxLength = Config.defaultPreviewLength
                }
            }
        }

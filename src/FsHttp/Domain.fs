[<AutoOpen>]
module FsHttp.Domain

open System.Threading

type StatusCodeExpectation = {
    expected: System.Net.HttpStatusCode list
    actual: System.Net.HttpStatusCode
}

exception StatusCodeExpectedxception of StatusCodeExpectation

type BodyPrintMode = {
    format: bool
    maxLength: int option
}

type PrintMode<'bodyPrintMode> =
    | AsObject
    | HeadersOnly
    | HeadersAndBody of BodyPrintMode

type PrintHint = {
    requestPrintMode: PrintMode<unit>
    responsePrintMode: PrintMode<BodyPrintMode>
}

type CertErrorStrategy =
    | Default
    | AlwaysAccept

type Proxy = {
    url: string
    credentials: System.Net.ICredentials option
}

type Config = {
    timeout: System.TimeSpan option
    defaultDecompressionMethods: System.Net.DecompressionMethods list
    printHint: PrintHint
    headerTransformers: list<Header -> Header>
    httpMessageTransformers: list<System.Net.Http.HttpRequestMessage -> System.Net.Http.HttpRequestMessage>
    httpClientHandlerTransformers: list<System.Net.Http.SocketsHttpHandler -> System.Net.Http.SocketsHttpHandler>
    httpClientTransformers: list<System.Net.Http.HttpClient -> System.Net.Http.HttpClient>
    httpCompletionOption: System.Net.Http.HttpCompletionOption
    proxy: Proxy option
    certErrorStrategy: CertErrorStrategy
    httpClientFactory: Config -> System.Net.Http.HttpClient
    // Calls `LoadIntoBufferAsync` of the response's HttpContent immediately after receiving.
    bufferResponseContent: bool
    cancellationToken: CancellationToken
}

and ConfigTransformer = Config -> Config

and PrintHintTransformer = PrintHint -> PrintHint

and FsHttpTarget = {
    method: System.Net.Http.HttpMethod option
    address: string option
    additionalQueryParams: list<string * string>
}

and Header = {
    target: FsHttpTarget
    headers: Map<string, string>
    // We use a .Net type here, which we never do in other places.
    // Since Cookie is record style, I see no problem here.
    cookies: System.Net.Cookie list
}

type BodyContent =
    | Empty
    | Single of SinglepartContent
    | Multi of MultipartContent

and SinglepartContent = {
    contentElement: ContentElement
    headers: Map<string, string>
}

and MultipartContent = {
    partElements: MultipartElement list
    headers: Map<string, string>
}

and MultipartElement = {
    name: string
    content: ContentElement
    fileName: string option
}

and ContentData =
    | TextContent of string
    | BinaryContent of byte array
    | StreamContent of System.IO.Stream
    | FormUrlEncodedContent of Map<string, string>
    | FileContent of string

and ContentType = {
    value: string
    charset: System.Text.Encoding option
}

and ContentElement = {
    contentData: ContentData
    explicitContentType: ContentType option
}

type Request = {
    header: Header
    content: BodyContent
    config: Config
}

type IToRequest =
    abstract member ToRequest: unit -> Request

type IUpdateConfig<'t, 'self> =
    abstract member UpdateConfig: 't -> 'self

// It seems to impossible extending builder methods on the context type
// directly when they are not polymorph.
type IRequestContext<'self> =
    abstract member Self: 'self

let configPrinter (c: IUpdateConfig<ConfigTransformer, _>) transformPrintHint =
    c.UpdateConfig(fun conf -> { conf with printHint = transformPrintHint conf.printHint })

// Unifying IToBodyContext and IToMultipartContext doesn't work; see:
// https://github.com/dotnet/fsharp/issues/12814
type IToBodyContext =
    inherit IToRequest
    abstract member ToBodyContext: unit -> BodyContext

and IToMultipartContext =
    inherit IToRequest
    abstract member ToMultipartContext: unit -> MultipartContext

// TODO: Convert this to a class.
and HeaderContext = {
    header: Header
    config: Config
} with
    interface IRequestContext<HeaderContext> with
        member this.Self = this

    interface IUpdateConfig<ConfigTransformer, HeaderContext> with
        member this.UpdateConfig(transformConfig) = { this with config = transformConfig this.config }

    interface IUpdateConfig<PrintHintTransformer, HeaderContext> with
        member this.UpdateConfig(transformPrintHint) = configPrinter this transformPrintHint

    interface IToRequest with
        member this.ToRequest() = {
            header = this.header
            content = Empty
            config = this.config
        }

    interface IToBodyContext with
        member this.ToBodyContext() = {
            header = this.header
            bodyContent = {
                contentElement = {
                    contentData = BinaryContent [||]
                    explicitContentType = None
                }
                headers = Map.empty
            }
            config = this.config
        }

    interface IToMultipartContext with
        member this.ToMultipartContext() = {
            header = this.header
            config = this.config
            multipartContent = {
                partElements = []
                headers = Map.empty
            }
        }

// TODO: Convert this to a class.
and BodyContext = {
    header: Header
    bodyContent: SinglepartContent
    config: Config
} with
    interface IRequestContext<BodyContext> with
        member this.Self = this

    interface IUpdateConfig<ConfigTransformer, BodyContext> with
        member this.UpdateConfig(transformConfig) = { this with config = transformConfig this.config }

    interface IUpdateConfig<PrintHintTransformer, BodyContext> with
        member this.UpdateConfig(transformPrintHint) = configPrinter this transformPrintHint

    interface IToRequest with
        member this.ToRequest() = {
            header = this.header
            content = Single this.bodyContent
            config = this.config
        }

    interface IToBodyContext with
        member this.ToBodyContext() = this

// TODO: Convert this to a class.
and MultipartContext = {
    header: Header
    multipartContent: MultipartContent
    config: Config
} with
    interface IRequestContext<MultipartContext> with
        member this.Self = this

    interface IUpdateConfig<ConfigTransformer, MultipartContext> with
        member this.UpdateConfig(transformConfig) = { this with config = transformConfig this.config }

    interface IUpdateConfig<PrintHintTransformer, MultipartContext> with
        member this.UpdateConfig(transformPrintHint) = configPrinter this transformPrintHint

    interface IToRequest with
        member this.ToRequest() = {
            header = this.header
            content = Multi this.multipartContent
            config = this.config
        }

    interface IToMultipartContext with
        member this.ToMultipartContext() = this

// TODO: Convert this to a class.
and MultipartElementContext = {
    parent: MultipartContext
    part: MultipartElement
} with
    interface IRequestContext<MultipartElementContext> with
        member this.Self = this

    interface IUpdateConfig<ConfigTransformer, MultipartElementContext> with
        member this.UpdateConfig(transformConfig) =
            let updatedCfg = this.parent.config |> transformConfig
            { this with parent.config = updatedCfg }

    interface IUpdateConfig<PrintHintTransformer, MultipartElementContext> with
        member this.UpdateConfig(transformPrintHint) = configPrinter this transformPrintHint

    interface IToRequest with
        member this.ToRequest() =
            let parentWithSelf = (this :> IToMultipartContext).ToMultipartContext()
            (parentWithSelf :> IToRequest).ToRequest()

    interface IToMultipartContext with
        member this.ToMultipartContext() =
            let parentElementsAndSelf = this.parent.multipartContent.partElements @ [ this.part ]
            { this with parent.multipartContent.partElements = parentElementsAndSelf }.parent

type Response = {
    request: Request
    requestMessage: System.Net.Http.HttpRequestMessage
    content: System.Net.Http.HttpContent
    headers: System.Net.Http.Headers.HttpResponseHeaders
    reasonPhrase: string
    statusCode: System.Net.HttpStatusCode
    version: System.Version
    originalHttpRequestMessage: System.Net.Http.HttpRequestMessage
    originalHttpResponseMessage: System.Net.Http.HttpResponseMessage
    dispose: unit -> unit
} with
    interface IUpdateConfig<PrintHintTransformer, Response> with
        member this.UpdateConfig(transformPrintHint) = 
            { this with request.config.printHint = transformPrintHint this.request.config.printHint }

    interface System.IDisposable with
        member this.Dispose() = this.dispose ()

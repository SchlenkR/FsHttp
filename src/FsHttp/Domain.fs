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
    printHint: PrintHint
}

type IToRequest =
    abstract member ToRequest: unit -> Request

type IUpdateConfig<'self> =
    abstract member UpdateConfig: (Config -> Config) -> 'self

type IUpdatePrintHint<'self> =
    abstract member UpdatePrintHint: (PrintHint -> PrintHint) -> 'self

// It seems to impossible extending builder methods on the context type
// directly when they are not polymorph.
type IRequestContext<'self> =
    abstract member Self: 'self

// Unifying IToBodyContext and IToMultipartContext doesn't work; see:
// https://github.com/dotnet/fsharp/issues/12814
type IToBodyContext =
    inherit IToRequest
    abstract member ToBodyContext: unit -> BodyContext

and IToMultipartContext =
    inherit IToRequest
    abstract member ToMultipartContext: unit -> MultipartContext

and HeaderContext = {
    header: Header
    config: Config
    printHint: PrintHint
} with
    interface IRequestContext<HeaderContext> with
        member this.Self = this

    interface IUpdateConfig<HeaderContext> with
        member this.UpdateConfig(transformConfig) =
            { this with config = transformConfig this.config }

    interface IUpdatePrintHint<HeaderContext> with
        member this.UpdatePrintHint(transformPrintHint) =
            { this with printHint = transformPrintHint this.printHint }

    interface IToRequest with
        member this.ToRequest() = {
            header = this.header
            content = Empty
            config = this.config
            printHint = this.printHint
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
            printHint = this.printHint
        }

    interface IToMultipartContext with
        member this.ToMultipartContext() = {
            header = this.header
            config = this.config
            printHint = this.printHint
            multipartContent = {
                partElements = []
                headers = Map.empty
            }
        }

and BodyContext = {
    header: Header
    bodyContent: SinglepartContent
    config: Config
    printHint: PrintHint
} with
    interface IRequestContext<BodyContext> with
        member this.Self = this

    interface IUpdateConfig<BodyContext> with
        member this.UpdateConfig(transformConfig) =
            { this with config = transformConfig this.config }
    
    interface IUpdatePrintHint<BodyContext> with
        member this.UpdatePrintHint(transformPrintHint) =
            { this with printHint = transformPrintHint this.printHint }

    interface IToRequest with
        member this.ToRequest() = {
            header = this.header
            content = Single this.bodyContent
            config = this.config
            printHint = this.printHint
        }

    interface IToBodyContext with
        member this.ToBodyContext() = this

and MultipartContext = {
    header: Header
    multipartContent: MultipartContent
    config: Config
    printHint: PrintHint
} with
    interface IRequestContext<MultipartContext> with
        member this.Self = this

    interface IUpdateConfig<MultipartContext> with
        member this.UpdateConfig(transformConfig) =
            { this with config = transformConfig this.config }

    interface IUpdatePrintHint<MultipartContext> with
        member this.UpdatePrintHint(transformPrintHint) =
            { this with printHint = transformPrintHint this.printHint }

    interface IToRequest with
        member this.ToRequest() = {
            header = this.header
            content = Multi this.multipartContent
            config = this.config
            printHint = this.printHint
        }

    interface IToMultipartContext with
        member this.ToMultipartContext() = this

and MultipartElementContext = {
    parent: MultipartContext
    part: MultipartElement
} with
    interface IRequestContext<MultipartElementContext> with
        member this.Self = this

    interface IUpdateConfig<MultipartElementContext> with
        member this.UpdateConfig(transformConfig) =
            { this with parent.config = this.parent.config |> transformConfig }

    interface IUpdatePrintHint<MultipartElementContext> with
        member this.UpdatePrintHint(transformPrintHint) =
            { this with parent.printHint = transformPrintHint this.parent.printHint }

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
    printHint: PrintHint
    originalHttpRequestMessage: System.Net.Http.HttpRequestMessage
    originalHttpResponseMessage: System.Net.Http.HttpResponseMessage
    dispose: unit -> unit
} with
    interface IUpdatePrintHint<Response> with
        member this.UpdatePrintHint(transformPrintHint) =
            { this with request.printHint = transformPrintHint this.request.printHint }


    interface System.IDisposable with
        member this.Dispose() = this.dispose ()

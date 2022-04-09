[<AutoOpen>]
module FsHttp.Domain

open System

type StatusCodeExpectation =
    { expected: System.Net.HttpStatusCode list
      actual: System.Net.HttpStatusCode
    }

exception StatusCodeExpectedxception of StatusCodeExpectation

type BodyPrintMode = 
    { format: bool
      maxLength: int option
    }
type PrintMode<'bodyPrintMode> =
    | AsObject
    | HeadersOnly
    | HeadersAndBody of BodyPrintMode
type PrintHint = 
    { printDebugMessages: bool
      requestPrintMode: PrintMode<unit>
      responsePrintMode: PrintMode<BodyPrintMode>
    }

type CertErrorStrategy =
    | Default
    | AlwaysAccept

type Proxy =
    { url: string
      credentials: System.Net.ICredentials option
    }

type HttpClientHandlerTransformer =
#if NETSTANDARD2_0 || NETSTANDARD2_1
    (System.Net.Http.HttpClientHandler -> System.Net.Http.HttpClientHandler) option
#else
    (System.Net.Http.SocketsHttpHandler -> System.Net.Http.SocketsHttpHandler) option
#endif

type Config =
    { timeout: TimeSpan
      printHint: PrintHint
      httpMessageTransformer: (System.Net.Http.HttpRequestMessage -> System.Net.Http.HttpRequestMessage) option
      httpClientHandlerTransformer: HttpClientHandlerTransformer
      httpClientTransformer: (System.Net.Http.HttpClient -> System.Net.Http.HttpClient) option
      httpCompletionOption: System.Net.Http.HttpCompletionOption
      proxy: Proxy option
      certErrorStrategy: CertErrorStrategy
      httpClientFactory: (unit -> System.Net.Http.HttpClient) option
      
      /// Calls `LoadIntoBufferAsync` of the response's HttpContent immediately after receiving.
      bufferResponseContent: bool
    }

type ConfigTransformer = Config -> Config

type PrintHintTransformer = PrintHint -> PrintHint

type FsHttpUrl =
    { address: string
      additionalQueryParams: Map<string, obj>
    }
    member url.ToUriString() =
        let uri = UriBuilder(url.address)
        let queryParamsString = 
            url.additionalQueryParams 
            |> Seq.map (fun kvp -> $"{kvp.Key}={kvp.Value}") 
            |> String.concat "&"
        uri.Query <-
            match uri.Query, queryParamsString with
            | "", "" -> ""
            | s, "" -> s
            | "", q -> $"?{q}"
            | s, q -> $"{s}&{q}"
        uri.ToString()


type Header =
    { url: FsHttpUrl
      method: System.Net.Http.HttpMethod
      headers: Map<string, string>
      // We use a .Net type here, which we never do in other places.
      // Since Cookie is record style, I see no problem here.
      cookies: System.Net.Cookie list
    }

type ContentData =
    | StringContent of string
    | ByteArrayContent of byte array
    | StreamContent of System.IO.Stream
    | FormUrlEncodedContent of Map<string, string>
    | FileContent of string

type BodyContent =
    { contentData: ContentData
      headers: Map<string, string>
      contentType: string option
    }

type MultipartContent =
    { contentData:
        {| name: string
           contentType: string option
           content: ContentData |} list
      headers: Map<string, string>
      contentType: string
    }

type RequestContent =
    | Empty
    | Single of BodyContent
    | Multi of MultipartContent

type Request =
    { header: Header
      content: RequestContent
      config: Config
    }

type IToRequest =
    abstract member Transform : unit -> Request

type IConfigure<'t, 'self> =
    abstract member Configure : 't -> 'self

// It seems to impossible extending builder methods on the context type
// directly when they are not polymorph.
type IRequestContext<'self> =
    abstract member Self : 'self

let configPrinter (c: IConfigure<ConfigTransformer, _>) transformPrintHint =
    c.Configure (fun conf -> { conf with printHint = transformPrintHint conf.printHint })

// Unifying IToBodyContext and IToMultipartContext doesn't work; see:
// https://github.com/dotnet/fsharp/issues/12814
type IToBodyContext =
    inherit IToRequest
    abstract member Transform : unit -> BodyContext

and IToMultipartContext =
    inherit IToRequest
    abstract member Transform : unit -> MultipartContext

and HeaderContext =
    { header: Header
      config: Config
    }
    interface IRequestContext<HeaderContext> with
        member this.Self = this
    interface IConfigure<ConfigTransformer, HeaderContext> with
        member this.Configure(transformConfig) =
            { this with config = transformConfig this.config }
    interface IConfigure<PrintHintTransformer, HeaderContext> with
        member this.Configure(transformPrintHint) =
            configPrinter this transformPrintHint
    interface IToRequest with
        member this.Transform() =
            { Request.header = this.header
              content = Empty
              config = this.config
            }
    interface IToBodyContext with
        member this.Transform() =
            { header = this.header
              content = {
                  BodyContent.contentData = ByteArrayContent [| |]
                  headers = Map.empty
                  contentType = None
              }
              config = this.config
            }
    interface IToMultipartContext with
        member this.Transform() =
            let boundary = Guid.NewGuid().ToString("N")
            { MultipartContext.header = this.header
              content = { 
                  MultipartContent.contentData = []
                  headers = Map.empty
                  contentType = $"multipart/form-data; boundary={boundary}"
              }
              currentPartContentType = None
              config = this.config
            }

and BodyContext =
    { header: Header
      content: BodyContent
      config: Config
    }
    interface IRequestContext<BodyContext> with
        member this.Self = this
    interface IConfigure<ConfigTransformer, BodyContext> with
        member this.Configure(transformConfig) =
            { this with config = transformConfig this.config }
    interface IConfigure<PrintHintTransformer, BodyContext> with
        member this.Configure(transformPrintHint) =
            configPrinter this transformPrintHint
    interface IToRequest with
        member this.Transform() =
            { Request.header = this.header
              content = Single this.content
              config = this.config
            }
    interface IToBodyContext with
        member this.Transform() = this
        
and MultipartContext =
    { header: Header
      content: MultipartContent
      currentPartContentType : string option
      config: Config
    }
    interface IRequestContext<MultipartContext> with
        member this.Self = this
    interface IConfigure<ConfigTransformer, MultipartContext> with
        member this.Configure(transformConfig) =
            { this with config = transformConfig this.config }
    interface IConfigure<PrintHintTransformer, MultipartContext> with
        member this.Configure(transformPrintHint) =
            configPrinter this transformPrintHint
    interface IToRequest with
        member this.Transform() =
            { Request.header = this.header
              content = Multi this.content
              config = this.config
            }
    interface IToMultipartContext with
        member this.Transform() = this

type Response = 
    { request: Request
      requestMessage: System.Net.Http.HttpRequestMessage
      content: System.Net.Http.HttpContent
      headers: System.Net.Http.Headers.HttpResponseHeaders
      reasonPhrase: string
      statusCode: System.Net.HttpStatusCode
      version: Version
      originalHttpRequestMessage: System.Net.Http.HttpRequestMessage
      originalHttpResponseMessage: System.Net.Http.HttpResponseMessage
      dispose: unit -> unit
    }
    interface IConfigure<PrintHintTransformer, Response> with
        member this.Configure(transformPrintHint) =
            { this with
                request =
                    { this.request with
                        config = { this.request.config with
                                    printHint = transformPrintHint this.request.config.printHint
                        }
                }
            }
    interface IDisposable with
        member this.Dispose() = this.dispose()

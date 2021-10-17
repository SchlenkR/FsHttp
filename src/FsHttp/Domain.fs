[<AutoOpen>]
module FsHttp.Domain

open System
open System.Net.Http

type ContentPrintHint =
    { isEnabled: bool
      format: bool
      maxLength: int }

type RequestPrintHint = 
    { printHeader: bool
      printBody: bool }

type ResponsePrintHint =
    { printHeader: bool
      printContent: ContentPrintHint }

type PrintHint = 
    { isEnabled: bool
      requestPrintHint: RequestPrintHint
      responsePrintHint: ResponsePrintHint }

type CertErrorStrategy =
    | Default
    | AlwaysAccept

type Proxy =
    { url: string
      credentials: System.Net.ICredentials option }

// TODO: Get rid of all the boolean switches and use options instead.
type Config =
    { timeout: TimeSpan
      printHint: PrintHint
      printDebugMessages: bool
      httpMessageTransformer: (HttpRequestMessage -> HttpRequestMessage) option
#if NETSTANDARD_2
      httpClientHandlerTransformer: (HttpClientHandler -> HttpClientHandler) option
#else
      httpClientHandlerTransformer: (SocketsHttpHandler -> SocketsHttpHandler) option
#endif
      httpClientTransformer: (HttpClient -> HttpClient) option
      httpCompletionOption: HttpCompletionOption
      proxy: Proxy option
      certErrorStrategy: CertErrorStrategy
      httpClientFactory: (unit -> HttpClient) option }

type ConfigTransformer = Config -> Config


type FsHttpUrl =
    { address: string
      additionalQueryParams: Map<string, string> }

type Header =
    { url: FsHttpUrl
      method: HttpMethod
      headers: Map<string, string>
      // We use a .Net type here, which we never do in other places.
      // Since Cookie is record style, I see no problem here.
      cookies: System.Net.Cookie list }

type ContentData =
    | StringContent of string
    | ByteArrayContent of byte array
    | StreamContent of System.IO.Stream
    | FormUrlEncodedContent of Map<string, string>
    | FileContent of string

type BodyContent =
    { contentData: ContentData
      headers: Map<string, string>
      // TODO: remove this
      contentType: string option }

type MultipartContent =
    { contentData:
        {| name: string
           contentType: string option
           content: ContentData |} list
      contentType: string }

type Request =
    { header: Header
      content: RequestContent
      config: Config }

and RequestContent =
| Empty
| Single of BodyContent
| Multi of MultipartContent
        


type StartingContext =
    | StartingContext
    // TODO: Get rid of this
    interface IToRequest with
        member _.ToRequest() = failwith "StartingContext can't be transformed."
    interface IToBodyContext with
        member _.ToBodyContext() = failwith "StartingContext can't be transformed."
    interface IToMultipartContext with
        member _.ToMultipartContext() = failwith "StartingContext can't be transformed."

and HeaderContext =
    { header: Header
      config: Config } with
    interface IToRequest with
        member this.ToRequest() =
            { Request.header = this.header
              content = Empty
              config = this.config }
    interface IToBodyContext with
        member this.ToBodyContext() =
            { header = this.header
              content =
                { BodyContent.contentData = ByteArrayContent [| |]
                  headers = Map.empty
                  contentType = None }
              config = this.config }
    interface IToMultipartContext with
        member this.ToMultipartContext() =
            { MultipartContext.header = this.header
              content =
                { MultipartContent.contentData = []
                  contentType = sprintf "multipart/form-data; boundary=%s" (Guid.NewGuid().ToString("N")) }
              currentPartContentType = None
              config = this.config }
    member this.Configure(transformConfig: ConfigTransformer) =
        { this with config = transformConfig this.config }

and BodyContext =
    { header: Header
      content: BodyContent
      config: Config } with
    interface IToRequest with
        member this.ToRequest() =
            { Request.header = this.header
              content = Single this.content
              config = this.config }
    interface IToBodyContext with
        member this.ToBodyContext() = this
    interface IToMultipartContext with
        member _.ToMultipartContext() = failwith "BodyContext can't be transformed."
    member this.Configure(transformConfig: ConfigTransformer) =
        { this with config = transformConfig this.config }
        
and MultipartContext =
    { header: Header
      content: MultipartContent
      currentPartContentType : string option
      config: Config } with
    interface IToRequest with
        member this.ToRequest() =
            { Request.header = this.header
              content = Multi this.content
              config = this.config }
    interface IToBodyContext with
        member _.ToBodyContext() = failwith "MultipartContext can't be transformed."
    interface IToMultipartContext with
        member this.ToMultipartContext() = this
    member this.Configure(transformConfig: ConfigTransformer) =
        { this with config = transformConfig this.config }

and IToRequest =
    abstract member ToRequest : unit -> Request

and IToBodyContext =
    abstract member ToBodyContext : unit -> BodyContext

and IToMultipartContext =
    abstract member ToMultipartContext : unit -> MultipartContext


type Response = 
    { requestContext: Request
      requestMessage: System.Net.Http.HttpRequestMessage
      content: System.Net.Http.HttpContent
      headers: System.Net.Http.Headers.HttpResponseHeaders
      reasonPhrase: string
      statusCode: System.Net.HttpStatusCode
      version: Version
      printHint: PrintHint
      originalHttpRequestMessage: System.Net.Http.HttpRequestMessage
      originalHttpResponseMessage: System.Net.Http.HttpResponseMessage }


module FsHttpUrl =
    let toUriString (url: FsHttpUrl) =
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

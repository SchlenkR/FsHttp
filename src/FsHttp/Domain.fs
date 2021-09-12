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
        

type IRequestBuilderContext =
    abstract member ToRequest : unit -> Request
    
    // We cannot use an OOP interface for Configure because no HKTs here
    // abstract member Configure : (Config -> Config) -> ? 


type StartingContext =
    | StartingContext
    
    interface IRequestBuilderContext with
        member this.ToRequest () =
            failwith "Loophole! Even though a StartingContext implements IContext, it somehow doesn't."
    
    // TODO: There seems to be something wrong here.
    member this.Configure (transformConfig: ConfigTransformer) : StartingContext =
        failwith "Loophole! Even though a StartingContext implements Configure, it somehow doesn't."


type HeaderContext =
    { header: Header
      config: Config } with

    interface IRequestBuilderContext with
        member this.ToRequest () =
            { Request.header = this.header
              content = Empty
              config = this.config }

    member this.Configure (transformConfig: ConfigTransformer) =
        { this with config = transformConfig this.config }

and BodyContext =
    { header: Header
      content: BodyContent
      config: Config } with

    interface IRequestBuilderContext with
        member this.ToRequest () =
            { Request.header = this.header
              content = Single this.content
              config = this.config }

    member this.Configure (transformConfig: ConfigTransformer) =
        { this with config = transformConfig this.config }
        
and MultipartContext =
    { header: Header
      content: MultipartContent
      currentPartContentType : string option
      config: Config } with
    
    interface IRequestBuilderContext with
        member this.ToRequest() =
            { Request.header = this.header
              content = Multi this.content
              config = this.config }

    member this.Configure (transformConfig: ConfigTransformer) =
        { this with config = transformConfig this.config }


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

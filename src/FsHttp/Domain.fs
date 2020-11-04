
[<AutoOpen>]
module FsHttp.Domain

open System


// TODO: Get rid of all the boolean switches and use options instead.
type Config =
    { timeout: TimeSpan
      printHint: PrintHint
      printDebugMessages: bool
      httpMessageTransformer: (System.Net.Http.HttpRequestMessage -> System.Net.Http.HttpRequestMessage) option
#if NETSTANDARD_2
      httpClientHandlerTransformer: (System.Net.Http.HttpClientHandler -> System.Net.Http.HttpClientHandler) option
#else
      httpClientHandlerTransformer: (System.Net.Http.SocketsHttpHandler -> System.Net.Http.SocketsHttpHandler) option
#endif
      httpClientTransformer: (System.Net.Http.HttpClient -> System.Net.Http.HttpClient) option
      httpCompletionOption: System.Net.Http.HttpCompletionOption
      proxy: Proxy option
      certErrorStrategy: CertErrorStrategy
      httpClientFactory: (unit -> System.Net.Http.HttpClient) option }

and Proxy =
    { url: string
      credentials: System.Net.ICredentials option }

and CertErrorStrategy =
    | Default
    | AlwaysAccept

and PrintHint = 
    { isEnabled: bool
      requestPrintHint: RequestPrintHint
      responsePrintHint: ResponsePrintHint }

and RequestPrintHint = 
    { printHeader: bool
      printBody: bool }

and ResponsePrintHint =
    { printHeader: bool
      printContent: ContentPrintHint }

and ContentPrintHint =
    { isEnabled: bool
      format: bool
      maxLength: int }

type ConfigTransformer = Config -> Config



type Header =
    { url: string
      method: System.Net.Http.HttpMethod
      headers: (string * string) list
      // We use a .Net type here, which we never do in other places.
      // Since Cookie is record style, I see no problem here.
      cookies: System.Net.Cookie list }

type ContentData =
    | StringContent of string
    | ByteArrayContent of byte array
    | StreamContent of System.IO.Stream
    | FormUrlEncodedContent of (string * string) list
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
        

type IContext =
    abstract member ToRequest : unit -> Request
    
    // We cannot use an OOP interface for Configure because no HKTs here
    // abstract member Configure : (Config -> Config) -> ? 


type StartingContext =
    | StartingContext
    
    interface IContext with
        member this.ToRequest () =
            failwith "Loophole! Even though a StartingContext implements IContext, it somehow doesn't."

    member this.Configure (transformConfig: ConfigTransformer) : StartingContext =
        failwith "Loophole! Even though a StartingContext implements Configure, it somehow doesn't."


type HeaderContext =
    { header: Header
      config: Config } with

    interface IContext with
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

    interface IContext with
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
    
    interface IContext with
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
      printHint: PrintHint }

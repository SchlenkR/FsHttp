
[<AutoOpen>]
module FsHttp.Domain

open System
open System.Net.Http


type PrintHint = 
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

type Proxy =
    { url: string
      credentials: System.Net.ICredentials option }

type Config =
    { timeout: TimeSpan
      printHint: PrintHint
      httpMessageTransformer: (HttpRequestMessage -> HttpRequestMessage) option
#if NETSTANDARD_2
      httpClientHandlerTransformer: (HttpClientHandler -> HttpClientHandler) option
#else
      httpClientHandlerTransformer: (SocketsHttpHandler -> SocketsHttpHandler) option
#endif
      httpClientTransformer: (HttpClient -> HttpClient) option
      proxy: Proxy option
      httpClient: HttpClient option }

type Header =
    { url: string
      method: HttpMethod
      headers: (string * string) list
      // We use a .Net type here, which we never do in other places.
      // Since Cookie is record style, I see no problem here.
      cookies: System.Net.Cookie list }


and ContentData =
    | StringContent of string
    | ByteArrayContent of byte array
    | StreamContent of System.IO.Stream
    | FormUrlEncodedContent of (string * string) list
    | FileContent of string

type StartingContext = StartingContext


type HeaderContext =
    { header: Header
      config: Config } with

    interface IContext with
        member this.ToRequest () =
            { Request.header = this.header
              content = Empty
              config = this.config }

    member this.Configure transformConfig =
        { this with config = transformConfig this.config }

and BodyContent =
    { contentData: ContentData
      contentType: string option }
        
and BodyContext =
    { header: Header
      content: BodyContent
      config: Config } with

    interface IContext with
        member this.ToRequest () =
            { Request.header = this.header
              content = Single this.content
              config = this.config }

    member this.Configure transformConfig =
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

    member this.Configure transformConfig =
        { this with config = transformConfig this.config }

and MultipartContent =
    { contentData:
        {| name: string
           contentType: string option
           content: ContentData |} list
      contentType: string }



and Request =
    { header: Header
      content: RequestContent
      config: Config }

and RequestContent =
| Empty
| Single of BodyContent
| Multi of MultipartContent


and IContext =
    abstract member ToRequest : unit -> Request
    
    // We cannot use an OOP interface for Configure because no HKTs here
    // abstract member Configure : (Config -> Config) -> ? 


// TODO: Get rid of all the boolean switches and use options instead.
type Response = 
    { requestContext: Request
      requestMessage: HttpRequestMessage
      content: HttpContent
      headers: Headers.HttpResponseHeaders
      reasonPhrase: string
      statusCode: System.Net.HttpStatusCode
      version: Version
      printHint: PrintHint }

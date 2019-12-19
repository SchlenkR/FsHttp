
[<AutoOpen>]
module FsHttp.Domain

open System
open System.Net.Http


type PrintHint = 
    { isEnabled: bool
      requestPrintHint: RequestPrintHint
      responsePrintHint: ResponsePrintHint }

and RequestPrintHint = 
    { isEnabled: bool
      printHeader: bool }

and ResponsePrintHint =
    { isEnabled: bool
      printHeader: bool
      printContent: ContentPrintHint }

and ContentPrintHint =
    { isEnabled: bool
      format: bool
      maxLength: int }

type Config =
    { timeout: TimeSpan
      printHint: PrintHint
      httpMessageTransformer: (HttpRequestMessage -> HttpRequestMessage) option
      httpClientTransformer: (HttpClient -> HttpClient) option }

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

    member this.Finalize () =
        { FinalContext.header = this.header
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

    member this.Finalize () =
        { FinalContext.header = this.header
          content = Single this.content
          config = this.config }

    member this.Configure transformConfig =
        { this with config = transformConfig this.config }

        
and MultipartContext =
    { header: Header
      content: MultipartContent
      currentPartContentType : string option
      config: Config } with

    member this.Finalize () =
        { FinalContext.header = this.header
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

and FinalContentData =
    | Empty
    | Single of BodyContent
    | Multi of MultipartContent

and FinalContext =
    { header: Header
      content: FinalContentData
      config: Config } with

    // important because we can use sendFinal with all context types
    member this.Finalize () = this


// TODO: Get rid of all the boolean switches and use options instead.
type Response = 
    { requestContext: FinalContext
      requestMessage: HttpRequestMessage
      content: HttpContent
      headers: Headers.HttpResponseHeaders
      reasonPhrase: string
      statusCode: System.Net.HttpStatusCode
      version: Version
      printHint: PrintHint }

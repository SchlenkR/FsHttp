
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

type Content =
    { contentData: ContentData
      contentType: string
      name: string option }

and ContentType =
    | Single of ContentData
    | Multi of ContentData list

and ContentData =
    | StringContent of string
    | ByteArrayContent of byte array
    | StreamContent of System.IO.Stream
    | FormUrlEncodedContent of (string * string) list
    | FileContent of string


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
    static member Finalize (this: HeaderContext) =
        let finalContext = { header = this.header; content = None; config = this.config }
        finalContext

type BodyContext =
    { header: Header
      content: Content
      config: Config } with
    static member Finalize (this: BodyContext) =
        let finalContext:FinalContext =
            { header = this.header
              content = Some this.content
              config = this.config }
        finalContext

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

module FsHttp.Dsl

open System
open System.Net
open System.Net.Http
open System.Text
open System.Globalization

open Domain
open Config

[<AutoOpen>]
module R =

    let request (method: string) (url: string) =

        let formattedUrl =
            url.Split([| '\n' |], StringSplitOptions.RemoveEmptyEntries)
            |> Seq.map (fun x -> x.Trim())
            |> Seq.filter (fun x -> not (x.StartsWith("//")))
            // TODO
            //|> Seq.map (fun x ->
            //    if x.StartsWith("?") || x.StartsWith("&")
            //    then x.Substring(1)
            //    else x
            //)
            |> Seq.reduce (+)

        { header =
            { url = formattedUrl
              method = HttpMethod(method)
              headers = []
              cookies = [] }
          config =
            { timeout = defaultTimeout
              printHint = defaultPrintHint
              httpMessageTransformer = None
              httpClientTransformer = None
              proxy = None
              httpClient = None } }

    // RFC 2626 specifies 8 methods + PATCH

    let get (url: string) = request "GET" url

    let put (url: string) = request "PUT" url

    let post (url: string) = request "POST" url

    let delete (url: string) = request "DELETE" url

    let options (url: string) = request "OPTIONS" url

    let head (url: string) = request "HEAD" url

    let trace (url: string) = request "TRACE" url

    let connect (url: string) = request "CONNECT" url

    let patch (url: string) = request "PATCH" url

// RFC 4918 (WebDAV) adds 7 methods
// TODO

[<AutoOpen>]
module H =

    /// Adds a custom header
    let header name value (context: HeaderContext) =
        { context with header = { context.header with headers = context.header.headers @ [ name, value ] } }

    /// Content-Types that are acceptable for the response
    let accept (contentType: string) (context: HeaderContext) =
        header "Accept" contentType context

    /// Character sets that are acceptable
    let acceptCharset (characterSets: string) (context: HeaderContext) =
        header "Accept-Charset" characterSets context

    /// Acceptable version in time
    let acceptDatetime (dateTime: DateTime) (context: HeaderContext) =
        header "Accept-Datetime" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context

    /// List of acceptable encodings. See HTTP compression.
    let acceptEncoding (encoding: string) (context: HeaderContext) =
        header "Accept-Encoding" encoding context

    /// List of acceptable human languages for response
    let acceptLanguage (language: string) (context: HeaderContext) =
        header "Accept-Language" language context

    // response field
    /////// The Allow header, which specifies the set of HTTP methods supported.
    ////let allow (methods: string) (context:HeaderContext) =
    ////    headerField "Allow" methods context 

    let query (context: HeaderContext) (queryParams: (string * string) list) (next: Next<_, _>) =
        { context with header = { context.header with url = context.header.url |> appendQueryToUrl queryParams } } |> next
    
    /// Authentication credentials for HTTP authentication
    let authorization (credentials: string) (context: HeaderContext) =
        header "Authorization" credentials context

    /// Authentication header using Bearer Auth token
    let bearerAuth (token: string) (context: HeaderContext) =
        let s = token |> sprintf "Bearer %s"
        authorization s context

    /// Authentication header using Basic Auth encoding
    let basicAuth (username: string) (password: string) (context: HeaderContext) =
        let s =
            sprintf "%s:%s" username password
            |> String.toBase64
            |> sprintf "Basic %s"
        authorization s context

    /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
    let cacheControl (control: string) (context: HeaderContext) =
        header "Cache-Control" control context

    /// What type of connection the user-agent would prefer
    let connection (connection: string) (context: HeaderContext) =
        header "Connection" connection context

    let private addCookie (cookie: Cookie) (context: HeaderContext) =
        { context with header = { context.header with cookies = context.header.cookies @ [ cookie ] } }

    /// An HTTP cookie previously sent by the server with 'Set-Cookie'.
    let cookie (name: string) (value: string) (context: HeaderContext) =
        addCookie (Cookie(name, value)) context

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies.
    let cookieForPath (name: string) (value: string) (path: string) (context: HeaderContext) =
        addCookie (Cookie(name, value, path)) context

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies
    /// and the internet domain for which this Cookie is valid.
    let cookieForDomain
        (name: string)
        (value: string)
        (path: string)
        (domain: string)
        (context: HeaderContext)
        =
        addCookie (Cookie(name, value, path, domain)) context

    /// The date and time that the message was sent
    let date (date: DateTime) (context: HeaderContext) =
        header "Date" (date.ToString("R", CultureInfo.InvariantCulture)) context

    /// Indicates that particular server behaviors are required by the client
    let expect (behaviors: string) (context: HeaderContext) =
        header "Expect" behaviors context

    /// Gives the date/time after which the response is considered stale
    let expires (dateTime: DateTime) (context: HeaderContext) =
        header "Expires" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context

    // TODO: Forwarded ?

    /// The email address of the user making the request
    let from (email: string) (context: HeaderContext) = header "From" email context

    /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
    /// The port number may be omitted if the port is the standard port for the service requested.
    let host (host: string) (context: HeaderContext) = header "Host" host context

    /// Only perform the action if the client supplied entity matches the same entity on the server.
    /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
    let ifMatch (entity: string) (context: HeaderContext) = header "If-Match" entity context

    /// Allows a 304 Not Modified to be returned if content is unchanged
    let ifModifiedSince (dateTime: DateTime) (context: HeaderContext) =
        header "If-Modified-Since" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context

    /// Allows a 304 Not Modified to be returned if content is unchanged
    let ifNoneMatch (etag: string) (context: HeaderContext) =
        header "If-None-Match" etag context

    /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
    let ifRange (range: string) (context: HeaderContext) = header "If-Range" range context

    /// Only send the response if the entity has not been modified since a specific time
    let ifUnmodifiedSince (dateTime: DateTime) (context: HeaderContext) =
        header "If-Unmodified-Since" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context

    /// Specifies a parameter used into order to maintain a persistent connection
    let keepAlive (keepAlive: string) (context: HeaderContext) =
        header "Keep-Alive" keepAlive context

    /// Specifies the date and time at which the accompanying body data was last modified
    let lastModified (dateTime: DateTime) (context: HeaderContext) =
        header "Last-Modified" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context

    /// Limit the number of times the message can be forwarded through proxies or gateways
    let maxForwards (count: int) (context: HeaderContext) =
        header "Max-Forwards" (count.ToString()) context

    /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
    let origin (origin: string) (context: HeaderContext) = header "Origin" origin context

    /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
    let pragma (pragma: string) (context: HeaderContext) = header "Pragma" pragma context

    /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
    let prefer (prefer: string) (context: HeaderContext) = header "Prefer" prefer context

    /// Authorization credentials for connecting to a proxy.
    let proxyAuthorization (credentials: string) (context: HeaderContext) =
        header "Proxy-Authorization" credentials context

    /// Request only part of an entity. Bytes are numbered from 0
    let range (start: int64) (finish: int64) (context: HeaderContext) =
        header "Range" (sprintf "bytes=%d-%d" start finish) context

    /// This is the address of the previous web page from which a link to the currently requested page was followed. (The word "referrer" is misspelled in the RFC as well as in most implementations.)
    let referer (referer: string) (context: HeaderContext) =
        header "Referer" referer context

    /// The transfer encodings the user agent is willing to accept: the same values as for the response header
    /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
    /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
    let te (te: string) (context: HeaderContext) = header "TE" te context

    /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
    let trailer (trailer: string) (context: HeaderContext) =
        header "Trailer" trailer context

    /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.  The valid directives are one of: chunked, compress, deflate, gzip, or identity.
    let transferEncoding (directive: string) (context: HeaderContext) =
        header "Transfer-Encoding" directive context

    /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
    let translate (translate: string) (context: HeaderContext) =
        header "Translate" translate context

    /// Specifies additional communications protocols that the client supports.
    let upgrade (upgrade: string) (context: HeaderContext) =
        header "Upgrade" upgrade context

    /// The user agent string of the user agent
    let userAgent (userAgent: string) (context: HeaderContext) =
        header "User-Agent" userAgent context

    /// Informs the server of proxies through which the request was sent
    let via (server: string) (context: HeaderContext) = header "Via" server context

    /// A general warning about possible problems with the entity body
    let warning (message: string) (context: HeaderContext) =
        header "Warning" message context

    /// Override HTTP method.
    let xhttpMethodOverride (httpMethod: string) (context: HeaderContext) =
        header "X-HTTP-Method-Override" httpMethod context

[<AutoOpen>]
module B =

    let private emptyContentData =
        { BodyContent.contentData = ContentData.ByteArrayContent [||]
          contentType = None }

    let body (headerContext: HeaderContext) =
        { BodyContext.header = headerContext.header
          content = emptyContentData
          config = headerContext.config }
       

    /// The type of encoding used on the data
    let contentEncoding (encoding: string) (context: HeaderContext) =
        header "Content-Encoding" encoding context

    /// The MIME type of the body of the request (used with POST and PUT requests)
    let contentType (contentType: string) (context: BodyContext) =
        { context with content = { context.content with contentType = Some contentType } }

    /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
    let contentTypeWithEncoding (contentTypeString) (charset: Encoding) (context: BodyContext) =
        contentType (sprintf "%s; charset=%s" contentTypeString (charset.WebName)) context

    // a) MD5 is obsolete. See https://tools.ietf.org/html/rfc7231#appendix-B
    // b) the others are response fields

    /////// The language the content is in
    ////let contentLanguage (context:HeaderContext) (language: string) (next:<_,_>) =
    ////    header "Content-Language" language context 

    /////// An alternate location for the returned data
    ////let contentLocation (context:HeaderContext) (location: string) (next:<_,_>) =
    ////    header "Content-Location" location context 

    /////// A Base64-encoded binary MD5 sum of the content of the request body
    ////let contentMD5 (context:HeaderContext) (md5sum: string) (next:<_,_>) =
    ////    header "Content-MD5" md5sum context 

    /////// Where in a full body message this partial message belongs
    ////let contentRange (context:HeaderContext) (range: string) (next:<_,_>) =
    ////    header "Content-Range" range context 

    let private content defaultContentType data (context: BodyContext) =
        let content = context.content
        let contentType = content.contentType |> Option.defaultValue defaultContentType

        { context with
              content =
                  { content with
                        contentData = data
                        contentType = Some contentType } }
       

    let binary (data: byte array) (context: BodyContext) =
        content MimeTypes.octetStream (ContentData.ByteArrayContent data) context

    let stream (stream: System.IO.Stream) (context: BodyContext) =
        content MimeTypes.octetStream (ContentData.StreamContent stream) context

    let text (text: string) (context: BodyContext) =
        content MimeTypes.textPlain (ContentData.StringContent text) context

    let json (json: string) (context: BodyContext) =
        content MimeTypes.applicationJson (ContentData.StringContent json) context

    let formUrlEncoded (data: (string * string) list) (context: BodyContext) =
        content "application/x-www-form-urlencoded" (ContentData.FormUrlEncodedContent data) context

    let file (path: string) (context: BodyContext) =
        content MimeTypes.octetStream (ContentData.FileContent path) context

// TODO: Base64

[<AutoOpen>]
module M =

    let private emptyContentData =
        { MultipartContent.contentData = []
          contentType = sprintf "multipart/form-data; boundary=%s" (Guid.NewGuid().ToString("N")) }

    let multipart (headerContext: HeaderContext) =
        { MultipartContext.header = headerContext.header
          content = emptyContentData
          currentPartContentType = None
          config = headerContext.config }
       

    let part
        (content: ContentData)
        (defaultContentType: string option)
        (name: string)
        (context: MultipartContext)
        =

        let contentType =
            match context.currentPartContentType with
            | None -> defaultContentType
            | Some v -> Some v

        let newContentData =
            {| name = name
               contentType = contentType
               content = content |}

        { context with content = { context.content with contentData = context.content.contentData @ [ newContentData ] } }
       

    let valuePart name (value: string) (context: MultipartContext) =
        part (ContentData.StringContent value) None name context

    let filePartWithName name (path: string) (context: MultipartContext) =
        let contentType = MimeTypes.guessMineTypeFromPath path MimeTypes.defaultMimeType
        part (ContentData.FileContent path) (Some contentType) name context

    let filePart (path: string) (context: MultipartContext) =
        filePartWithName (System.IO.Path.GetFileNameWithoutExtension path) path context

    /// The MIME type of the body of the request (used with POST and PUT requests)
    let contentType (contentType: string) (context: MultipartContext) =
        { context with currentPartContentType = Some contentType }

// TODO: Config should work on any context, not just header context
[<AutoOpen>]
module Config =

    let config (f: Config -> Config) (context: HeaderContext) =
        { context with config = f context.config }

    let timeout value (context: HeaderContext) =
        config (fun config -> { config with timeout = value }) context

    let timeoutInSeconds value (context: HeaderContext) =
        config (fun config -> { config with timeout = TimeSpan.FromSeconds value }) context

    let transformHttpRequestMessage map (context: HeaderContext) =
        config (fun config -> { config with httpMessageTransformer = Some map }) context

    let transformHttpClient map (context: HeaderContext) =
        config (fun config -> { config with httpClientTransformer = Some map }) context

    let proxy url (context: HeaderContext) =
        config (fun config -> { config with proxy = Some { url = url; credentials = None } }) context

    let proxyWithCredentials url credentials (context: HeaderContext) =
        config (fun config -> { config with proxy = Some { url = url; credentials = Some credentials } }) context 

    /// Inject a HttpClient that will be used directly (most config parameters specified here will be ignored). 
    let useHttpClient (client: HttpClient) (context: HeaderContext) =
        config (fun config -> { config with httpClient = Some client }) context

[<AutoOpen>]
module Fsi =

    open FsHttp.Fsi

    ////// run is not needed anymore. disadvantage: no easy custom-printmodifiers
    ////let inline run context (printMod: Response -> 'a) =
    ////    send context |> printMod

    // overrides for print modifier in DSL
    let inline raw context = Request.send context |> modifyPrinter rawPrinterTransformer
    let inline headerOnly context = Request.send context |> modifyPrinter headerOnlyPrinterTransformer
    let inline show maxLength context = Request.send context |> modifyPrinter (showPrinterTransformer maxLength)
    let inline preview context = Request.send context |> modifyPrinter previewPrinterTransformer
    let inline prv context = preview context
    let inline go context = preview context
    let inline expand context = Request.send context |> modifyPrinter expandPrinterTransformer
    let inline exp context = expand context

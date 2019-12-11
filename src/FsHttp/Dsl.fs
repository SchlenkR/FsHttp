
module FsHttp.Dsl

open System
open System.Net
open System.Net.Http
open System.Text
open System.Globalization

open Domain
open Config
open RequestHandling


type Next<'a, 'b> = 'a -> 'b

/// Finalizes a Request build pipeline.
let fin = id

let headerField name value (context: HeaderContext)  (next: Next<_,_>) =
    { context with header = { context.header with headers = context.header.headers @ [name,value] } } |> next

[<AutoOpen>]
module R =
    
    let request (method: string) (url: string) (next: Next<_,_>) =

        let formattedUrl =
            url.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries)
            |> Seq.map (fun x -> x.Trim())
            |> Seq.filter (fun x -> not (x.StartsWith("//")))
            // TODO
            //|> Seq.map (fun x ->
            //    if x.StartsWith("?") || x.StartsWith("&")
            //    then x.Substring(1)
            //    else x
            //)
            |> Seq.reduce (+)

        let headerContext =
            { header = { url=formattedUrl
                         method=HttpMethod(method)
                         headers=[]
                         cookies=[] }
              config = { timeout=defaultTimeout
                         printHint = defaultPrintHint
                         httpMessageTransformer=None
                         httpClientTransformer=None } }
        next headerContext

    // RFC 2626 specifies 8 methods + PATCH
    
    let get (url:string) (next: Next<_,_>) =
        request "GET" url next
    
    let put (url:string) (next: Next<_,_>) =
        request "PUT" url next
    
    let post (url:string) (next: Next<_,_>) =
        request "POST" url next
    
    let delete (url:string) (next: Next<_,_>) =
        request "DELETE" url next
    
    let options (url:string) (next: Next<_,_>) =
        request "OPTIONS" url next
    
    let head (url:string) (next: Next<_,_>) =
        request "HEAD" url next
    
    let trace (url:string) (next: Next<_,_>) =
        request "TRACE" url next
    
    let connect (url:string) (next: Next<_,_>) =
        request "CONNECT" url next
    
    let patch (url:string) (next: Next<_,_>) =
        request "PATCH" url next

    // RFC 4918 (WebDAV) adds 7 methods
    // TODO

[<AutoOpen>]
module H =

    /// Content-Types that are acceptable for the response
    let accept (context:HeaderContext) (contentType:string) (next: Next<_,_>) =
        headerField "Accept" contentType context next

    /// Character sets that are acceptable
    let acceptCharset (context:HeaderContext) (characterSets:string) (next: Next<_,_>) =
        headerField "Accept-Charset" characterSets context  next

    /// Acceptable version in time
    let acceptDatetime (context:HeaderContext) (dateTime:DateTime) (next: Next<_,_>) =
        headerField "Accept-Datetime" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context  next
    
    /// List of acceptable encodings. See HTTP compression.
    let acceptEncoding (context:HeaderContext) (encoding:string) (next: Next<_,_>) =
        headerField "Accept-Encoding" encoding context  next
    
    /// List of acceptable human languages for response
    let acceptLanguage (context:HeaderContext) (language:string) (next: Next<_,_>) =
        headerField "Accept-Language" language context  next
    
    // response field
    /////// The Allow header, which specifies the set of HTTP methods supported.
    ////let allow (context:HeaderContext) (methods: string) (next: Next<_,_>) =
    ////    headerField "Allow" methods context  next
    
    /// Authentication credentials for HTTP authentication
    let authorization (context:HeaderContext) (credentials: string) (next: Next<_,_>) =
        headerField "Authorization" credentials context  next
    
    /// Authentication header using Bearer Auth token
    let bearerAuth (context:HeaderContext) (token: string) (next: Next<_,_>) =
        let s = token |> sprintf "Bearer %s"
        authorization context s next
    
    /// Authentication header using Basic Auth encoding
    let basicAuth (context:HeaderContext) (username: string) (password: string) (next: Next<_,_>) =
        let s = sprintf "%s:%s" username password |> Helper.toBase64 |> sprintf "Basic %s"
        authorization context s next
    
    /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
    let cacheControl (context:HeaderContext) (control: string) (next: Next<_,_>) =
        headerField "Cache-Control" control context  next
    
    /// What type of connection the user-agent would prefer
    let connection (context:HeaderContext) (connection: string) (next: Next<_,_>) =
        headerField "Connection" connection context  next
    
    let private addCookie (context:HeaderContext) (cookie:Cookie) (next: Next<_,_>) =
        { context with header = { context.header with cookies = context.header.cookies @ [cookie] } } |> next

    /// An HTTP cookie previously sent by the server with 'Set-Cookie'.
    let cookie (context:HeaderContext) (name: string) (value: string) (next: Next<_,_>) =
        addCookie context (Cookie(name, value)) next

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies.
    let cookieForPath (context:HeaderContext) (name: string) (value: string) (path: string) (next: Next<_,_>) =
        addCookie context (Cookie(name, value, path)) next

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies
    /// and the internet domain for which this Cookie is valid.
    let cookieForDomain (context:HeaderContext) (name: string) (value: string) (path: string) (domain: string) (next: Next<_,_>) =
        addCookie context (Cookie(name, value, path, domain)) next

    /// The date and time that the message was sent
    let date (context:HeaderContext) (date:DateTime) (next: Next<_,_>) =
        headerField "Date" (date.ToString("R", CultureInfo.InvariantCulture)) context  next
    
    /// Indicates that particular server behaviors are required by the client
    let expect (context:HeaderContext) (behaviors: string) (next: Next<_,_>) =
        headerField "Expect" behaviors context  next
    
    /// Gives the date/time after which the response is considered stale
    let expires (context:HeaderContext) (dateTime:DateTime) (next: Next<_,_>) =
        headerField "Expires" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context  next
    
    // TODO: Forwarded ?

    /// The email address of the user making the request
    let from (context:HeaderContext) (email: string) (next: Next<_,_>) =
        headerField "From" email context  next
    
    /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
    /// The port number may be omitted if the port is the standard port for the service requested.
    let host (context:HeaderContext) (host: string) (next: Next<_,_>) =
        headerField "Host" host context  next
    
    /// Only perform the action if the client supplied entity matches the same entity on the server.
    /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
    let ifMatch (context:HeaderContext) (entity: string) (next: Next<_,_>) =
        headerField "If-Match" entity context  next
    
    /// Allows a 304 Not Modified to be returned if content is unchanged
    let ifModifiedSince (context:HeaderContext) (dateTime:DateTime) (next: Next<_,_>) =
        headerField "If-Modified-Since" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context  next
    
    /// Allows a 304 Not Modified to be returned if content is unchanged
    let ifNoneMatch (context:HeaderContext) (etag: string) (next: Next<_,_>) =
        headerField "If-None-Match" etag context  next
    
    /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
    let ifRange (context:HeaderContext) (range: string) (next: Next<_,_>) =
        headerField "If-Range" range context  next
    
    /// Only send the response if the entity has not been modified since a specific time
    let ifUnmodifiedSince (context:HeaderContext) (dateTime:DateTime) (next: Next<_,_>) =
        headerField "If-Unmodified-Since" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context  next
    
    /// Specifies a parameter used into order to maintain a persistent connection
    let keepAlive (context:HeaderContext) (keepAlive: string) (next: Next<_,_>) =
        headerField "Keep-Alive" keepAlive context  next
    
    /// Specifies the date and time at which the accompanying body data was last modified
    let lastModified (context:HeaderContext) (dateTime:DateTime) (next: Next<_,_>) =
        headerField "Last-Modified" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context  next
    
    /// Limit the number of times the message can be forwarded through proxies or gateways
    let maxForwards (context:HeaderContext) (count:int) (next: Next<_,_>) =
        headerField "Max-Forwards" (count.ToString()) context  next
    
    /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
    let origin (context:HeaderContext) (origin: string) (next: Next<_,_>) =
        headerField "Origin" origin context  next
    
    /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
    let pragma (context:HeaderContext) (pragma: string) (next: Next<_,_>) =
        headerField "Pragma" pragma context  next
    
    /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
    let prefer (context:HeaderContext) (prefer: string) (next: Next<_,_>) =
        headerField "Prefer" prefer context  next
    
    /// Authorization credentials for connecting to a proxy.
    let proxyAuthorization (context:HeaderContext) (credentials: string) (next: Next<_,_>) =
        headerField "Proxy-Authorization" credentials context  next

    /// Request only part of an entity. Bytes are numbered from 0
    let range (context:HeaderContext) (start:int64) (finish:int64) (next: Next<_,_>) =
        headerField "Range" (sprintf "bytes=%d-%d" start finish) context  next
    
    /// This is the address of the previous web page from which a link to the currently requested page was followed. (The word "referrer" is misspelled in the RFC as well as in most implementations.)
    let referer (context:HeaderContext) (referer: string) (next: Next<_,_>) =
        headerField "Referer" referer context  next
    
    /// The transfer encodings the user agent is willing to accept: the same values as for the response header
    /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
    /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
    let te (context:HeaderContext) (te: string) (next: Next<_,_>) =
        headerField "TE" te context  next
    
    /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
    let trailer (context:HeaderContext) (trailer: string) (next: Next<_,_>) =
        headerField "Trailer" trailer context  next
    
    /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.  The valid directives are one of: chunked, compress, deflate, gzip, or identity.
    let transferEncoding (context:HeaderContext) (directive: string) (next: Next<_,_>) =
        headerField "Transfer-Encoding" directive context  next
    
    /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
    let translate (context:HeaderContext) (translate: string) (next: Next<_,_>) =
        headerField "Translate" translate context  next
    
    /// Specifies additional communications protocols that the client supports.
    let upgrade (context:HeaderContext) (upgrade: string) (next: Next<_,_>) =
        headerField "Upgrade" upgrade context  next
    
    /// The user agent string of the user agent
    let userAgent (context:HeaderContext) (userAgent: string) (next: Next<_,_>) =
        headerField "User-Agent" userAgent context  next
    
    /// Informs the server of proxies through which the request was sent
    let via (context:HeaderContext) (server: string) (next: Next<_,_>) =
        headerField "Via" server context  next
    
    /// A general warning about possible problems with the entity body
    let warning (context:HeaderContext) (message: string) (next: Next<_,_>) =
        headerField "Warning" message context  next
    
    /// Override HTTP method.
    let xhttpMethodOverride (context:HeaderContext) (httpMethod: string) (next: Next<_,_>) =
        headerField "X-HTTP-Method-Override" httpMethod context  next

[<AutoOpen>]
module B =

    let private emptyContentData =
        { contentData = ContentData.ByteArrayContent [||]
          contentType = ""
          name = None }

    let private add cd cds = cd :: cds
    let private replaceCurrent cds cd = (cds |> List.tail) |> add cd
    let private currentContentOf cds = cds |> List.head 

    let body (headerContext: HeaderContext) (next: Next<_,_>) =
        { header = headerContext.header
          contentParts = [ emptyContentData ]
          config = headerContext.config }
        |> next

    let part (bodyContext: BodyContext) (next: Next<_,_>) =
        { bodyContext with
              contentParts = bodyContext.contentParts |> add emptyContentData }
        |> next

    /// Describes the placement of the content. Valid dispositions are: inline, attachment, form-data
    let contentDisposition (context:HeaderContext) (placement: string) (name: string option) (fileName: string option) (next: Next<_,_>) =
        let namePart = match name with Some n -> sprintf "; name=\"%s\"" n | None -> ""
        let fileNamePart = match fileName with Some n -> sprintf "; filename=\"%s\"" n | None -> ""
        headerField "Content-Disposition" (sprintf "%s%s%s" placement namePart fileNamePart) context  next
    
    /// The type of encoding used on the data
    let contentEncoding (context:HeaderContext) (encoding: string) (next: Next<_,_>) =
        headerField "Content-Encoding" encoding context  next
    
    // a) MD5 is obsolete. See https://tools.ietf.org/html/rfc7231#appendix-B
    // b) the others are response fields

    /////// The language the content is in
    ////let contentLanguage (context:HeaderContext) (language: string) (next: Next<_,_>) =
    ////    header "Content-Language" language context  next
    
    /////// An alternate location for the returned data
    ////let contentLocation (context:HeaderContext) (location: string) (next: Next<_,_>) =
    ////    header "Content-Location" location context  next
    
    /////// A Base64-encoded binary MD5 sum of the content of the request body
    ////let contentMD5 (context:HeaderContext) (md5sum: string) (next: Next<_,_>) =
    ////    header "Content-MD5" md5sum context  next
    
    /////// Where in a full body message this partial message belongs
    ////let contentRange (context:HeaderContext) (range: string) (next: Next<_,_>) =
    ////    header "Content-Range" range context  next


    let private getContentTypeOrDefault (defaultValue:string) (contentDef: ContentPart) =
        if String.IsNullOrEmpty(contentDef.contentType) then defaultValue
        else contentDef.contentType

    let private content (context: BodyContext) defaultContentType data (next: Next<_,_>) =
        let content = currentContentOf context.contentParts
        let contentType = getContentTypeOrDefault defaultContentType content
        
        { context with
            contentParts =
                { content with contentData = data; contentType = contentType;  }
                |> replaceCurrent context.contentParts
        }
        |> next
    
    let binary (context: BodyContext) (data: byte array) (next: Next<_,_>) =
        content context "application/octet-stream" (ContentData.ByteArrayContent data) next
    
    let stream (context: BodyContext) (stream: System.IO.Stream) (next: Next<_,_>) =
        content context "application/octet-stream" (ContentData.StreamContent stream) next
    
    let text (context: BodyContext) (text: string) (next: Next<_,_>) =
        content context "text/plain" (ContentData.StringContent text) next

    let json (context: BodyContext) (json: string) (next: Next<_,_>) =
        content context "application/json" (ContentData.StringContent json) next

    let formUrlEncoded (context: BodyContext) (data: (string * string) list) (next: Next<_,_>) =
        content context "application/x-www-form-urlencoded" (ContentData.FormUrlEncodedContent data) next

    let file (context: BodyContext) (path: string) (next: Next<_,_>) =
        content context "application/octet-stream" (ContentData.FileContent path) next

    // TODO: Base64

    /// The MIME type of the body of the request (used with POST and PUT requests)
    let contentType (context: BodyContext) (contentType: string) (next: Next<_,_>) =
        let content = currentContentOf context.contentParts
        
        { context with
            contentParts =
                { content with contentType=contentType }
                |> replaceCurrent context.contentParts
        }
        |> next

    /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
    let contentTypeWithEncoding (context: BodyContext) (contentTypeString) (charset:Encoding) (next: Next<_,_>) =
        contentType context (sprintf "%s; charset=%s" contentTypeString (charset.WebName)) next

[<AutoOpen>]
module Config =
    
    let config (context: HeaderContext) (f: Config -> Config) (next: Next<_,_>) =
        { context with config = f context.config } |> next

    let timeout context value (next: Next<_,_>) =
        config context (fun config -> { config with timeout = value }) next

    let timeoutInSeconds context value (next: Next<_,_>) =
        config context (fun config -> { config with timeout = TimeSpan.FromSeconds value }) next
    
    let transformHttpRequestMessage context map (next: Next<_,_>) =
        config context (fun config -> { config with httpMessageTransformer = Some map }) next
    
    let transformHttpClient context map (next: Next<_,_>) =
        config context (fun config -> { config with httpClientTransformer = Some map }) next

[<AutoOpen>]
module Fsi =

    // run is not needed anymore. disadvantage: no easy custom-printmodifiers
    let inline run context (printMod: Response -> 'a) =
        send context |> printMod

    // overrides for print modifier in DSL
    // let inline raw context = send context |> raw
    // let inline header context = send context |> Fsi.header
    // let inline show context maxLength = send context |> (show maxLength)
    // let inline preview context = send context |> preview
    // let inline prv context = preview context
    // let inline go context = preview context
    // let inline expand context = send context |>  expand
    // let inline exp context = expand context

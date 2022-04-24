[<AutoOpen>]
module FsHttp.Dsl

open System
open System.Net
open System.Net.Http
open System.Text
open System.Globalization

open FsHttp
open FsHttp.HelperInternal
open FsHttp.Helper

/// Request constructors for RFC 2626 HTTP methods
[<AutoOpen>]
module Http =

    let methodWithConfig config (method: string) (url: string) =

        // FSI init HACK
        FsiInit.init()

        let formattedUrl =
            url.Split([| '\n' |], StringSplitOptions.RemoveEmptyEntries)
            |> Seq.map (fun x -> x.Trim().Replace("\r", ""))
            |> Seq.filter (fun x -> not (x.StartsWith("//")))
            |> Seq.reduce (+)

        { header =
            { url =
                { address = formattedUrl
                  additionalQueryParams = Map.empty }
              method = HttpMethod(method)
              headers = Map.empty
              cookies = [] }
          config = config }

    let method (method: string) (url: string) =
        methodWithConfig GlobalConfig.mutableDefaults method url

    let internal getWithConfig config (url: string) = methodWithConfig config "GET" url
    let internal putWithConfig config (url: string) = methodWithConfig config "PUT" url
    let internal postWithConfig config (url: string) = methodWithConfig config "POST" url
    let internal deleteWithConfig config (url: string) = methodWithConfig config "DELETE" url
    let internal optionsWithConfig config (url: string) = methodWithConfig config "OPTIONS" url
    let internal headWithConfig config (url: string) = methodWithConfig config "HEAD" url
    let internal traceWithConfig config (url: string) = methodWithConfig config "TRACE" url
    let internal connectWithConfig config (url: string) = methodWithConfig config "CONNECT" url
    let internal patchWithConfig config (url: string) = methodWithConfig config "PATCH" url

    let internal defaultConfig = GlobalConfig.mutableDefaults

    let get (url: string) = methodWithConfig defaultConfig "GET" url
    let put (url: string) = methodWithConfig defaultConfig "PUT" url
    let post (url: string) = methodWithConfig defaultConfig "POST" url
    let delete (url: string) = methodWithConfig defaultConfig "DELETE" url
    let options (url: string) = methodWithConfig defaultConfig "OPTIONS" url
    let head (url: string) = methodWithConfig defaultConfig "HEAD" url
    let trace (url: string) = methodWithConfig defaultConfig "TRACE" url
    let connect (url: string) = methodWithConfig defaultConfig "CONNECT" url
    let patch (url: string) = methodWithConfig defaultConfig "PATCH" url

    // TODO: RFC 4918 (WebDAV) adds 7 methods


module Header =

    /// Adds a header
    let header name value (context: HeaderContext) =
        { context with 
            header = { context.header with 
                         headers = Map.union context.header.headers [ name, value ] } }

    /// Adds a set of query parameters to the URL    
    let query (queryParams: (string * obj) list) (context: HeaderContext) =
        { context with
            header = { context.header with
                         url = { context.header.url with 
                                   additionalQueryParams = queryParams |> Map.ofList } } }
                         

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
    
    /// Authorization credentials for HTTP authorization
    let authorization (credentials: string) (context: HeaderContext) =
        header "Authorization" credentials context

    /// Authorization header using Bearer authorization token
    let authorizationBearer (token: string) (context: HeaderContext) =
        let s = token |> sprintf "Bearer %s"
        authorization s context

    /// Authorization header using Basic (User/Password) authorization
    let authorizationUserPw (username: string) (password: string) (context: HeaderContext) =
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

    let private cookieDotnet (cookie: Cookie) (context: HeaderContext) =
        { context with header = { context.header with cookies = context.header.cookies @ [ cookie ] } }

    /// An HTTP cookie previously sent by the server with 'Set-Cookie'.
    let cookie (name: string) (value: string) (context: HeaderContext) =
        cookieDotnet (Cookie(name, value)) context

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies.
    let cookieForPath (name: string) (value: string) (path: string) (context: HeaderContext) =
        cookieDotnet (Cookie(name, value, path)) context

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
        cookieDotnet (Cookie(name, value, path, domain)) context

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


module Body =
    open System.Text.Json

    /// Adds a header
    let header name value (context: IToBodyContext) =
        let context = context.Transform()
        { context with 
            content = { context.content with
                          headers = Map.union context.header.headers [ name, value ] } }

    /// The type of encoding used on the data
    let contentEncoding (encoding: string) (context: IToBodyContext) =
        header "Content-Encoding" encoding context

    /// The MIME type of the body of the request (used with POST and PUT requests)
    let contentType (contentType: string) (context: IToBodyContext) =
        let context = context.Transform()
        { context with content = { context.content with contentType = Some contentType } }

    /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
    let contentTypeWithEncoding (contentTypeString) (charset: Encoding) (context: IToBodyContext) =
        contentType (sprintf "%s; charset=%s" contentTypeString (charset.WebName)) context

    // a) MD5 is obsolete. See https://tools.ietf.org/html/rfc7231#appendix-B
    // b) the others are response fields

    /// The language the content is in
    let contentLanguage (language: string) (context: IToBodyContext)  =
        header "Content-Language" language context 

    /// An alternate location for the returned data
    let contentLocation (location: string) (context: IToBodyContext)  =
        header "Content-Location" location context 

    /// A Base64-encoded binary MD5 sum of the content of the request body
    let contentMD5 (md5sum: string) (context: IToBodyContext)  =
        header "Content-MD5" md5sum context 

    /// Where in a full body message this partial message belongs
    let contentRange (range: string) (context: IToBodyContext) =
        header "Content-Range" range context 

    let content defaultContentType (data: ContentData) (context: IToBodyContext) =
        let context = context.Transform()
        let content = context.content
        let contentType = content.contentType |> Option.defaultValue defaultContentType
        { context with
              content = { content with
                            contentData = data
                            contentType = Some contentType } }

    let binary (data: byte array) (context: IToBodyContext) =
        content MimeTypes.octetStream (ByteArrayContent data) context

    let stream (stream: System.IO.Stream) (context: IToBodyContext) =
        content MimeTypes.octetStream (StreamContent stream) context

    let text (text: string) (context: IToBodyContext) =
        content MimeTypes.textPlain (StringContent text) context

    let base64 (base64: byte []) (context: IToBodyContext) =
        content MimeTypes.octetStream (StringContent (Convert.ToBase64String base64)) context

    let json (json: string) (context: IToBodyContext) =
        content MimeTypes.applicationJson (StringContent json) context

    let jsonSerializeWith options (instance: 'a) (context: IToBodyContext) =
        // TODO: Use async / stream
        let jsonString = JsonSerializer.Serialize(instance, options = options)
        json jsonString context

    let jsonSerialize (instance: 'a) (context: IToBodyContext) =
        jsonSerializeWith (GlobalConfig.Json.defaultJsonSerializerOptions) instance context

    let formUrlEncoded (data: (string * string) list) (context: IToBodyContext) =
        content "application/x-www-form-urlencoded" (FormUrlEncodedContent (Map.ofList data)) context

    let file (path: string) (context: IToBodyContext) =
        content MimeTypes.octetStream (FileContent path) context


module Multipart =
    
    let part
            (content: ContentData)
            (defaultContentType: string option)
            (name: string)
            (context: IToMultipartContext)
        =
        let context = context.Transform()
        let contentType =
            match context.currentPartContentType with
            | None -> defaultContentType
            | Some v -> Some v
        let newContentData =
            {| name = name
               contentType = contentType
               content = content |}
        { context with
            content = { context.content with 
                          contentData = context.content.contentData @ [ newContentData ] } }

    /// The MIME type of the body of the request (used with POST and PUT requests)
    let contentType (contentType: string) (context: IToMultipartContext) =
        let context = context.Transform()
        { context with currentPartContentType = Some contentType }

    // -----
    // PARTS
    // -----

    let stringPart name (value: string) (context: IToMultipartContext) =
        part (StringContent value) None name context

    let filePartWithName name (path: string) (context: IToMultipartContext) =
        let contentType = MimeTypes.guessMineTypeFromPath path MimeTypes.defaultMimeType
        part (FileContent path) (Some contentType) name context

    let filePart (path: string) (context: IToMultipartContext) =
        filePartWithName (System.IO.Path.GetFileNameWithoutExtension path) path context

    let byteArrayPart name (value: byte[]) (context: IToMultipartContext) =
        part (ByteArrayContent value) None name context

    let streamPart name (value: System.IO.Stream) (context: IToMultipartContext) =
        part (StreamContent value) None name context


module Config =
    module With =
        let inline ignoreCertIssues config =
            { config with certErrorStrategy = AlwaysAccept }

        let inline timeout value config =
            { config with timeout = value }

        let inline timeoutInSeconds value config =
            { config with timeout = TimeSpan.FromSeconds value }

        let inline setHttpClient (client: HttpClient) config =
            { config with httpClientFactory = Some (fun () -> client) }

        let inline setHttpClientFactory (clientFactory: unit -> HttpClient) config =
            { config with httpClientFactory = Some clientFactory }

        let inline transformHttpClient transformer config =
            { config with httpClientTransformer = Some transformer }

        let inline transformHttpRequestMessage transformer config =
            { config with httpMessageTransformer = Some transformer }

        let inline transformHttpClientHandler transformer config =
            { config with httpClientHandlerTransformer = Some transformer }

        let inline proxy url config =
            { config with proxy = Some { url = url; credentials = None } }

        let inline proxyWithCredentials url credentials config =
            { config with proxy = Some { url = url; credentials = Some credentials } }

    let inline update transformer (context: IConfigure<ConfigTransformer, _>)=
        context.Configure transformer

    let inline set (config: Config) context =
        context |> update (fun _ -> config)

    let inline ignoreCertIssues context =
        context |> update (fun config -> config |> With.ignoreCertIssues)

    let inline timeout value context =
        context |> update (fun config -> config |> With.timeout value)

    let inline timeoutInSeconds value context =
        context |> update (fun config -> config |> With.timeoutInSeconds value)

    let inline setHttpClient (client: HttpClient) context =
        context |> update (fun config -> config |> With.setHttpClient client)

    let inline setHttpClientFactory (clientFactory: unit -> HttpClient) context =
        context |> update (fun config -> config |> With.setHttpClientFactory clientFactory)

    let inline transformHttpClient transformer context =
        context |> update (fun config -> config |> With.transformHttpClient transformer)

    let inline transformHttpRequestMessage transformer context =
        context |> update (fun config -> config |> With.transformHttpRequestMessage transformer)

    let inline transformHttpClientHandler transformer context =
        context |> update (fun config -> config |> With.transformHttpClientHandler transformer)

    let inline proxy url context =
        context |> update (fun config -> config |> With.proxy url)

    let inline proxyWithCredentials url credentials context =
        context |> update (fun config -> config |> With.proxyWithCredentials url credentials)


module Print =

    let inline withConfig transformer (context: IConfigure<PrintHintTransformer, _>) =
        context.Configure transformer
    
    let inline withRequestPrintMode updatePrintMode context =
        context |> withConfig (fun printHint ->
            { printHint with requestPrintMode = updatePrintMode printHint.requestPrintMode })
    
    let inline withResponsePrintMode updatePrintMode context =
        context |> withConfig (fun printHint ->
            { printHint with responsePrintMode = updatePrintMode printHint.responsePrintMode })
    
    let inline withResponseBody updateBodyPrintMode context =
        context |> withResponsePrintMode (fun printMode ->
            match printMode with
            | AsObject | HeadersOnly -> updateBodyPrintMode (GlobalConfig.defaultHeadersAndBodyPrintMode())
            | HeadersAndBody x -> updateBodyPrintMode x
            |> HeadersAndBody)

    let inline useObjectFormatting context =
        context 
        |> withRequestPrintMode (fun _ -> AsObject)
        |> withResponsePrintMode (fun _ -> AsObject)
    
    let inline headerOnly context =
        context 
        |> withResponsePrintMode (fun _ -> HeadersOnly)
    
    let inline withResponseBodyLength maxLength context =
        context 
        |> withResponseBody (fun bodyPrintMode -> { bodyPrintMode with maxLength = Some maxLength })
    
    let inline withResponseBodyFormat format context = 
        context 
        |> withResponseBody (fun bodyPrintMode -> { bodyPrintMode with format = format })
    
    let inline withResponseBodyExpanded context =
        context 
        |> withResponseBody (fun bodyPrintMode -> { bodyPrintMode with maxLength = None })

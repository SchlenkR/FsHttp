module FsHttp.DslCE

open FsHttp.Domain


// ---------
// Builders
// ---------

// Whatch out: Config.defaultConfig is mutable, so access must be delayed.
let private defaultStartingContext () = { config = Config.defaultConfig }

type HttpBuilder<'context when 'context :> IToRequest>(context: 'context) =
    member this.Context = context
    member this.Yield(_) = HttpBuilder context
    interface IToRequest with
        member this.Transform() = context.Transform()
let http = HttpBuilder(defaultStartingContext())


/// Provides builder construction shortcuts for RFC 2626 HTTP methods
[<AutoOpen>]
module Http =
    let method (method: string) (url: string) = Dsl.Http.method method url |> HttpBuilder

    let get (url: string) = Dsl.Http.get url |> HttpBuilder
    let put (url: string) = Dsl.Http.put url |> HttpBuilder
    let post (url: string) = Dsl.Http.post url |> HttpBuilder
    let delete (url: string) = Dsl.Http.delete url |> HttpBuilder
    let options (url: string) = Dsl.Http.options url |> HttpBuilder
    let head (url: string) = Dsl.Http.head url |> HttpBuilder
    let trace (url: string) = Dsl.Http.trace url |> HttpBuilder
    let connect (url: string) = Dsl.Http.connect url |> HttpBuilder
    let patch (url: string) = Dsl.Http.patch url |> HttpBuilder

    // TODO: RFC 4918 (WebDAV) adds 7 methods


// ---------
// Methods
// ---------

let private build (builder: HttpBuilder<StartingContext>) method (url: string) =
    method url |> Dsl.Config.set builder.Context.config |> HttpBuilder

type HttpBuilder<'context when 'context :> IToRequest> with

    [<CustomOperation("Method")>]
    member this.Method(builder: HttpBuilder<StartingContext>, method, url) = 
        build builder (fun url -> Dsl.Http.method method url) url

    // RFC 2626 specifies 8 methods
    [<CustomOperation("GET")>]
    member this.Get(builder: HttpBuilder<StartingContext>, url) =
        build builder Dsl.Http.get url

    [<CustomOperation("PUT")>]
    member this.Put(builder: HttpBuilder<StartingContext>, url) =
        build builder Dsl.Http.put url

    [<CustomOperation("POST")>]
    member this.Post(builder: HttpBuilder<StartingContext>, url) = 
        build builder Dsl.Http.post url

    [<CustomOperation("DELETE")>]
    member this.Delete(builder: HttpBuilder<StartingContext>, url) = 
        build builder Dsl.Http.delete url

    [<CustomOperation("OPTIONS")>]
    member this.Options(builder: HttpBuilder<StartingContext>, url) = 
        build builder Dsl.Http.options url

    [<CustomOperation("HEAD")>]
    member this.Head(builder: HttpBuilder<StartingContext>, url) = 
        build builder Dsl.Http.head url

    [<CustomOperation("TRACE")>]
    member this.Trace(builder: HttpBuilder<StartingContext>, url) = 
        build builder Dsl.Http.trace url

    [<CustomOperation("CONNECT")>]
    member this.Connect(builder: HttpBuilder<StartingContext>, url) = 
        build builder Dsl.Http.connect url

    [<CustomOperation("PATCH")>]
    member this.Patch(builder: HttpBuilder<StartingContext>, url) = 
        build builder Dsl.Http.patch url


// ---------
// Headers
// ---------

type HttpBuilder<'context when 'context :> IToRequest> with

    /// Append query params
    [<CustomOperation("query")>]
    member this.Query(builder: HttpBuilder<_>, queryParams) =
        Dsl.Header.query queryParams builder.Context |> HttpBuilder

    /// Custom header
    [<CustomOperation("header")>]
    member this.Header(builder: HttpBuilder<_>, key, value) =
        Dsl.Header.header key value builder.Context |> HttpBuilder

    /// Content-Types that are acceptable for the response
    [<CustomOperation("Accept")>]
    member this.Accept(builder: HttpBuilder<_>, contentType) =
        Dsl.Header.accept contentType builder.Context |> HttpBuilder

    /// Character sets that are acceptable
    [<CustomOperation("AcceptCharset")>]
    member this.AcceptCharset(builder: HttpBuilder<_>, characterSets) =
        Dsl.Header.acceptCharset characterSets builder.Context |> HttpBuilder

    /// Acceptable version in time
    [<CustomOperation("AcceptDatetime")>]
    member this.AcceptDatetime(builder: HttpBuilder<_>, dateTime) =
        Dsl.Header.acceptDatetime dateTime builder.Context |> HttpBuilder

    /// List of acceptable encodings. See HTTP compression.
    [<CustomOperation("AcceptEncoding")>]
    member this.AcceptEncoding(builder: HttpBuilder<_>, encoding) =
        Dsl.Header.acceptEncoding encoding builder.Context |> HttpBuilder

    /// List of acceptable human languages for response
    [<CustomOperation("AcceptLanguage")>]
    member this.AcceptLanguage(builder: HttpBuilder<_>, language) =
        Dsl.Header.acceptLanguage language builder.Context |> HttpBuilder
        
    /// Authorization credentials for HTTP authorization
    [<CustomOperation("Authorization")>]
    member this.Authorization(builder: HttpBuilder<_>, credentials) =
        Dsl.Header.authorization credentials builder.Context |> HttpBuilder

    /// Authorization header using Bearer authorization token
    [<CustomOperation("AuthorizationBearer")>]
    member this.AuthorizationBearer(builder: HttpBuilder<_>, token) =
        Dsl.Header.authorizationBearer token builder.Context |> HttpBuilder

    /// Authorization header using Basic (User/Password) authorization
    [<CustomOperation("AuthorizationUserPw")>]
    member this.AuthorizationUserPw(builder: HttpBuilder<_>, username, password) =
        Dsl.Header.authorizationUserPw username password builder.Context |> HttpBuilder

    /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
    [<CustomOperation("CacheControl")>]
    member this.CacheControl(builder: HttpBuilder<_>, control) =
        Dsl.Header.cacheControl control builder.Context |> HttpBuilder

    /// What type of connection the user-agent would prefer
    [<CustomOperation("Connection")>]
    member this.Connection(builder: HttpBuilder<_>, connection) =
        Dsl.Header.connection connection builder.Context |> HttpBuilder

    /// An HTTP cookie previously sent by the server with 'Set-Cookie'.
    [<CustomOperation("Cookie")>]
    member this.SetCookie(builder: HttpBuilder<_>, name, value) =
        Dsl.Header.cookie name value builder.Context |> HttpBuilder

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies.
    [<CustomOperation("CookieForPath")>]
    member this.SetCookieForPath(builder: HttpBuilder<_>, name, value, path) =
        Dsl.Header.cookieForPath name value path builder.Context |> HttpBuilder

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies
    /// and the internet domain for which this Cookie is valid.
    [<CustomOperation("CookieForDomain")>]
    member this.SetCookieForDomain(builder: HttpBuilder<_>, name, value, path, domain) =
        Dsl.Header.cookieForDomain name value path domain builder.Context |> HttpBuilder

    /// The date and time that the message was sent
    [<CustomOperation("Date")>]
    member this.Date(builder: HttpBuilder<_>, date) =
        Dsl.Header.date date builder.Context |> HttpBuilder

    /// Indicates that particular server behaviors are required by the client
    [<CustomOperation("Expect")>]
    member this.Expect(builder: HttpBuilder<_>, behaviors) =
        Dsl.Header.expect behaviors builder.Context |> HttpBuilder

    /// Gives the date/time after which the response is considered stale
    [<CustomOperation("Expires")>]
    member this.Expires(builder: HttpBuilder<_>, dateTime) =
        Dsl.Header.expires dateTime builder.Context |> HttpBuilder

    /// The email address of the user making the request
    [<CustomOperation("From")>]
    member this.From(builder: HttpBuilder<_>, email) =
        Dsl.Header.from email builder.Context |> HttpBuilder

    /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
    /// The port number may be omitted if the port is the standard port for the service requested.
    [<CustomOperation("Host")>]
    member this.Host(builder: HttpBuilder<_>, host) =
        Dsl.Header.host host builder.Context |> HttpBuilder

    /// Only perform the action if the client supplied entity matches the same entity on the server.
    /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
    [<CustomOperation("IfMatch")>]
    member this.IfMatch(builder: HttpBuilder<_>, entity) =
        Dsl.Header.ifMatch entity builder.Context |> HttpBuilder

    /// Allows a 304 Not Modified to be returned if content is unchanged
    [<CustomOperation("IfModifiedSince")>]
    member this.IfModifiedSince(builder: HttpBuilder<_>, dateTime) =
        Dsl.Header.ifModifiedSince dateTime builder.Context |> HttpBuilder

    /// Allows a 304 Not Modified to be returned if content is unchanged
    [<CustomOperation("IfNoneMatch")>]
    member this.IfNoneMatch(builder: HttpBuilder<_>, etag) =
        Dsl.Header.ifNoneMatch etag builder.Context |> HttpBuilder

    /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
    [<CustomOperation("IfRange")>]
    member this.IfRange(builder: HttpBuilder<_>, range) =
        Dsl.Header.ifRange range builder.Context |> HttpBuilder

    /// Only send the response if the entity has not been modified since a specific time
    [<CustomOperation("IfUnmodifiedSince")>]
    member this.IfUnmodifiedSince(builder: HttpBuilder<_>, dateTime) =
        Dsl.Header.ifUnmodifiedSince dateTime builder.Context |> HttpBuilder

    /// Specifies a parameter used into order to maintain a persistent connection
    [<CustomOperation("KeepAlive")>]
    member this.KeepAlive(builder: HttpBuilder<_>, keepAlive) =
        Dsl.Header.keepAlive keepAlive builder.Context |> HttpBuilder

    /// Specifies the date and time at which the accompanying body data was last modified
    [<CustomOperation("LastModified")>]
    member this.LastModified(builder: HttpBuilder<_>, dateTime) =
        Dsl.Header.lastModified dateTime builder.Context |> HttpBuilder

    /// Limit the number of times the message can be forwarded through proxies or gateways
    [<CustomOperation("MaxForwards")>]
    member this.MaxForwards(builder: HttpBuilder<_>, count) =
        Dsl.Header.maxForwards count builder.Context |> HttpBuilder

    /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
    [<CustomOperation("Origin")>]
    member this.Origin(builder: HttpBuilder<_>, origin) =
        Dsl.Header.origin origin builder.Context |> HttpBuilder

    /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
    [<CustomOperation("Pragma")>]
    member this.Pragma(builder: HttpBuilder<_>, pragma) =
        Dsl.Header.pragma pragma builder.Context |> HttpBuilder

    /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
    [<CustomOperation("Prefer")>]
    member this.Prefer(builder: HttpBuilder<_>, prefer) =
        Dsl.Header.prefer prefer builder.Context |> HttpBuilder

    /// Authorization credentials for connecting to a proxy.
    [<CustomOperation("ProxyAuthorization")>]
    member this.ProxyAuthorization(builder: HttpBuilder<_>, credentials) =
        Dsl.Header.proxyAuthorization credentials builder.Context |> HttpBuilder

    /// Request only part of an entity. Bytes are numbered from 0
    [<CustomOperation("Range")>]
    member this.Range(builder: HttpBuilder<_>, start, finish) =
        Dsl.Header.range start finish builder.Context |> HttpBuilder

    /// This is the address of the previous web page from which a link to the currently requested page was followed.
    /// (The word "referrer" is misspelled in the RFC as well as in most implementations.)
    [<CustomOperation("Referer")>]
    member this.Referer(builder: HttpBuilder<_>, referer) =
        Dsl.Header.referer referer builder.Context |> HttpBuilder

    /// The transfer encodings the user agent is willing to accept: the same values as for the response header
    /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
    /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
    [<CustomOperation("TE")>]
    member this.TE(builder: HttpBuilder<_>, te) =
        Dsl.Header.te te builder.Context |> HttpBuilder

    /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
    [<CustomOperation("Trailer")>]
    member this.Trailer(builder: HttpBuilder<_>, trailer) =
        Dsl.Header.trailer trailer builder.Context |> HttpBuilder

    /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.
    /// The valid directives are one of: chunked, compress, deflate, gzip, orentity.
    [<CustomOperation("TransferEncoding")>]
    member this.TransferEncoding(builder: HttpBuilder<_>, directive) =
        Dsl.Header.transferEncoding directive builder.Context |> HttpBuilder

    /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
    [<CustomOperation("Translate")>]
    member this.Translate(builder: HttpBuilder<_>, translate) =
        Dsl.Header.translate translate builder.Context |> HttpBuilder

    /// Specifies additional communications protocols that the client supports.
    [<CustomOperation("Upgrade")>]
    member this.Upgrade(builder: HttpBuilder<_>, upgrade) =
        Dsl.Header.upgrade upgrade builder.Context |> HttpBuilder

    /// The user agent string of the user agent
    [<CustomOperation("UserAgent")>]
    member this.UserAgent(builder: HttpBuilder<_>, userAgent) =
        Dsl.Header.userAgent userAgent builder.Context |> HttpBuilder

    /// Informs the server of proxies through which the request was sent
    [<CustomOperation("Via")>]
    member this.Via(builder: HttpBuilder<_>, server) =
        Dsl.Header.via server builder.Context |> HttpBuilder

    /// A general warning about possible problems with the entity body
    [<CustomOperation("Warning")>]
    member this.Warning(builder: HttpBuilder<_>, message) =
        Dsl.Header.warning message builder.Context |> HttpBuilder

    /// Override HTTP method.
    [<CustomOperation("XHTTPMethodOverride")>]
    member this.XHTTPMethodOverride(builder: HttpBuilder<_>, httpMethod) =
        Dsl.Header.xhttpMethodOverride httpMethod builder.Context |> HttpBuilder


// ---------
// Body
// ---------

type HttpBuilder<'context when 'context :> IToRequest> with

    // we keep this in for better intellisense support (eventhough it's redundant)
    [<CustomOperation("body")>]
    member this.Body<'toBodyContext
            when 'toBodyContext :> IToRequest
            and 'toBodyContext :> IToBodyContext
            >
        (builder: HttpBuilder<'toBodyContext>) 
        = (builder.Context :> IToBodyContext).Transform() |> HttpBuilder

    [<CustomOperation("content")>]
    member this.Content(builder: HttpBuilder<_>, contentType, data) =
        Dsl.Body.content contentType data builder.Context |> HttpBuilder

    [<CustomOperation("binary")>]
    member this.Binary(builder: HttpBuilder<_>, data) =
        Dsl.Body.binary data builder.Context |> HttpBuilder

    [<CustomOperation("stream")>]
    member this.Stream(builder: HttpBuilder<_>, stream) =
        Dsl.Body.stream stream builder.Context |> HttpBuilder

    [<CustomOperation("text")>]
    member this.Text(builder: HttpBuilder<_>, text) =
        Dsl.Body.text text builder.Context |> HttpBuilder

    [<CustomOperation("json")>]
    member this.Json(builder: HttpBuilder<_>, json) =
        Dsl.Body.json json builder.Context |> HttpBuilder

    [<CustomOperation("formUrlEncoded")>]
    member this.FormUrlEncoded(builder: HttpBuilder<_>, data) =
        Dsl.Body.formUrlEncoded data builder.Context |> HttpBuilder

    [<CustomOperation("file")>]
    member this.File(builder: HttpBuilder<_>, path) =
        Dsl.Body.file path builder.Context |> HttpBuilder

    /// The type of encoding used on the data
    [<CustomOperation("ContentEncoding")>]
    member this.ContentEncoding(builder: HttpBuilder<_>, encoding) =
        Dsl.Body.contentEncoding encoding builder.Context |> HttpBuilder

    /// The MIME type of the body of the request (used with POST and PUT requests)
    [<CustomOperation("ContentType")>]
    member this.ContentType(builder: HttpBuilder<_>, contentType) =
        Dsl.Body.contentType contentType builder.Context |> HttpBuilder

    /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
    [<CustomOperation("ContentTypeWithEncoding")>]
    member this.ContentTypeWithEncoding(builder: HttpBuilder<_>, contentType, charset) =
        Dsl.Body.contentTypeWithEncoding contentType charset builder.Context |> HttpBuilder


// ---------
// Multipart
// ---------

type HttpBuilder<'context when 'context :> IToRequest> with

    /// The MIME type of the body of the request (used with POST and PUT requests)
    [<CustomOperation("ContentTypeForPart")>]
    member this.ContentTypeForPart(builder: HttpBuilder<_>, contentType) =
        Dsl.Multipart.contentType contentType builder.Context |> HttpBuilder

    // -----
    // PARTS
    // -----

    // we keep this in for better intellisense support (eventhough it's redundant)
    [<CustomOperation("multipart")>]
    member this.Multipart<'toMultipartContext
            when 'toMultipartContext :> IToRequest
            and 'toMultipartContext :> IToMultipartContext
            >
        (builder: HttpBuilder<'toMultipartContext>)
        = (builder.Context :> IToMultipartContext).Transform() |> HttpBuilder

    [<CustomOperation("part")>]
    member this.Part(builder: HttpBuilder<_>, content, defaultContentType, name) =
        Dsl.Multipart.part content defaultContentType name builder.Context |> HttpBuilder

    [<CustomOperation("stringPart")>]
    member this.StringPart(builder: HttpBuilder<_>, name, value) =
        Dsl.Multipart.stringPart name value builder.Context |> HttpBuilder

    [<CustomOperation("filePartWithName")>]
    member this.FilePartWithName(builder: HttpBuilder<_>, name, path) =
        Dsl.Multipart.filePartWithName name path builder.Context |> HttpBuilder

    [<CustomOperation("filePart")>]
    member this.FilePart(builder: HttpBuilder<_>, path) =
        Dsl.Multipart.filePart path builder.Context |> HttpBuilder

    [<CustomOperation("byteArrayPart")>]
    member this.ByteArrayPart(builder: HttpBuilder<_>, name, value) =
        Dsl.Multipart.byteArrayPart name value builder.Context |> HttpBuilder

    [<CustomOperation("streamPart")>]
    member this.StreamPart(builder: HttpBuilder<_>, name, value) =
        Dsl.Multipart.streamPart name value builder.Context |> HttpBuilder


// ---------
// Config
// ---------

type HttpBuilder<'context when 'context :> IToRequest> with

    [<CustomOperation("config_update")>]
    member inline this.Update(builder: HttpBuilder<_>, configTransformer) =
        Dsl.Config.update configTransformer builder.Context |> HttpBuilder

    [<CustomOperation("config_set")>]
    member inline this.Set(builder: HttpBuilder<_>, configTransformer) =
        Dsl.Config.set configTransformer builder.Context |> HttpBuilder

    // TODO: Provide certStrategy configs
    [<CustomOperation("config_ignoreCertIssues")>]
    member inline this.IgnoreCertIssues(builder: HttpBuilder<_>) =
        Dsl.Config.ignoreCertIssues builder.Context |> HttpBuilder

    [<CustomOperation("config_timeout")>]
    member inline this.Timeout(builder: HttpBuilder<_>, value) =
        Dsl.Config.timeout value builder.Context |> HttpBuilder

    [<CustomOperation("config_timeoutInSeconds")>]
    member inline this.TimeoutInSeconds(builder: HttpBuilder<_>, value) =
        Dsl.Config.timeoutInSeconds value builder.Context |> HttpBuilder

    [<CustomOperation("config_setHttpClient")>]
    member inline this.SetHttpClient(builder: HttpBuilder<_>, httpClient) =
        Dsl.Config.setHttpClient httpClient builder.Context |> HttpBuilder

    [<CustomOperation("config_setHttpClientFactory")>]
    member inline this.SetHttpClientFactory(builder: HttpBuilder<_>, httpClientFactory) =
        Dsl.Config.setHttpClientFactory httpClientFactory builder.Context |> HttpBuilder

    [<CustomOperation("config_transformHttpClient")>]
    member inline this.TransformHttpClient(builder: HttpBuilder<_>, transformer) =
        Dsl.Config.transformHttpClient transformer builder.Context |> HttpBuilder

    [<CustomOperation("config_transformHttpRequestMessage")>]
    member inline this.TransformHttpRequestMessage(builder: HttpBuilder<_>, transformer) =
        Dsl.Config.transformHttpRequestMessage transformer builder.Context |> HttpBuilder

    [<CustomOperation("config_transformHttpClientHandler")>]
    member inline this.TransformHttpClientHandler(builder: HttpBuilder<_>, transformer) =
        Dsl.Config.transformHttpClientHandler transformer builder.Context |> HttpBuilder

    [<CustomOperation("config_proxy")>]
    member inline this.Proxy(builder: HttpBuilder<_>, url) =
        Dsl.Config.proxy url builder.Context |> HttpBuilder

    [<CustomOperation("config_proxyWithCredentials")>]
    member inline this.ProxyWithCredentials(builder: HttpBuilder<_>, url, credentials) =
        Dsl.Config.proxyWithCredentials url credentials builder.Context |> HttpBuilder


// ---------
// Print
// ---------

let inline private modifyPrintHint f (context: ^t when ^t :> IToRequest) =
    let transformPrintHint (config: Config) = { config with printHint = f config.printHint }
    let res = (^t: (member Configure: (Config -> Config) -> ^t) (context, transformPrintHint))
    res |> HttpBuilder

type HttpBuilder<'context when 'context :> IToRequest> with

    [<CustomOperation("print_withConfig")>]
    member inline this.WithConfig(builder: HttpBuilder<_>, updatePrintHint) =
        Dsl.Print.withConfig updatePrintHint builder.Context |> HttpBuilder

    [<CustomOperation("print_withRequestPrintMode")>]
    member inline this.WithRequestPrintMode(builder: HttpBuilder<_>, updatePrintMode) =
        Dsl.Print.withRequestPrintMode updatePrintMode builder.Context |> HttpBuilder

    [<CustomOperation("print_withResponsePrintMode")>]
    member inline this.WithResponsePrintMode(builder: HttpBuilder<_>, updatePrintMode) =
        Dsl.Print.withResponsePrintMode updatePrintMode builder.Context |> HttpBuilder

    [<CustomOperation("print_withResponseBody")>]
    member inline this.WithResponseBody(builder: HttpBuilder<_>, updateBodyPrintMode) =
        Dsl.Print.withResponseBody updateBodyPrintMode builder.Context |> HttpBuilder
    
    [<CustomOperation("print_useObjectFormatting")>]
    member inline this.UseObjectFormatting(builder: HttpBuilder<_>) =
        Dsl.Print.useObjectFormatting builder.Context |> HttpBuilder
    
    [<CustomOperation("print_headerOnly")>]
    member inline this.HeaderOnly(builder: HttpBuilder<_>) =
        Dsl.Print.headerOnly builder.Context |> HttpBuilder
    
    [<CustomOperation("print_withResponseBodyLength")>]
    member inline this.WithResponseBodyLength(builder: HttpBuilder<_>, maxLength) =
        Dsl.Print.withResponseBodyLength maxLength builder.Context |> HttpBuilder
    
    [<CustomOperation("print_withResponseBodyFormat")>]
    member inline this.WithResponseBodyFormat(builder: HttpBuilder<_>, format) =
        Dsl.Print.withResponseBodyFormat format builder.Context |> HttpBuilder
    
    [<CustomOperation("print_withResponseBodyExpanded")>]
    member inline this.WithResponseBodyExpanded(builder: HttpBuilder<_>) =
        Dsl.Print.withResponseBodyExpanded builder.Context |> HttpBuilder

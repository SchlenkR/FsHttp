[<AutoOpen>]
module FsHttp.DslCE

open FsHttp


// ---------
// Builder instances
// ---------

type IRequestContext<'self> with
    member this.Yield(_) = this

[<AutoOpen>]
type HttpBuilder =
    // This is important to always have a new instance of the builder
    // that uses the config present at the time of the call.
    static member http = HeaderContext.create ()


// ---------
// Methods
// ---------

type IRequestContext<'self> with

    [<CustomOperation("Method")>]
    member this.Method(context: IRequestContext<HeaderContext>, method, url) = 
        context.Self |> HeaderContext.setUrl method url

    // Important: Do NOT use the Dsl.get etc., because they 
    // initialize the builder with the current config from scratch,
    // which means: Test `Pre-Configured Requests` will fail.

    // RFC 2626 specifies 8 methods
    [<CustomOperation("GET")>]
    member this.Get(context: IRequestContext<HeaderContext>, url) = 
        this.Method(context, HttpMethods.get, url)

    [<CustomOperation("PUT")>]
    member this.Put(context: IRequestContext<HeaderContext>, url) = 
        this.Method(context, HttpMethods.put, url)

    [<CustomOperation("POST")>]
    member this.Post(context: IRequestContext<HeaderContext>, url) = 
        this.Method(context, HttpMethods.post, url)

    [<CustomOperation("DELETE")>]
    member this.Delete(context: IRequestContext<HeaderContext>, url) = 
        this.Method(context, HttpMethods.delete, url)

    [<CustomOperation("OPTIONS")>]
    member this.Options(context: IRequestContext<HeaderContext>, url) =
        this.Method(context, HttpMethods.options, url)

    [<CustomOperation("HEAD")>]
    member this.Head(context: IRequestContext<HeaderContext>, url) = 
        this.Method(context, HttpMethods.head, url)

    [<CustomOperation("TRACE")>]
    member this.Trace(context: IRequestContext<HeaderContext>, url) = 
        this.Method(context, HttpMethods.trace, url)

    [<CustomOperation("CONNECT")>]
    member this.Connect(context: IRequestContext<HeaderContext>, url) =
        this.Method(context, HttpMethods.connect, url)

    [<CustomOperation("PATCH")>]
    member this.Patch(context: IRequestContext<HeaderContext>, url) = 
        this.Method(context, HttpMethods.patch, url)


// ---------
// Headers
// ---------

type IRequestContext<'self> with

    /// Append query params
    [<CustomOperation("query")>]
    member this.Query(context: IRequestContext<HeaderContext>, queryParams) = 
        Header.query queryParams context.Self

    /// Custom headers
    [<CustomOperation("headers")>]
    member this.Headers(context: IRequestContext<HeaderContext>, headers) = 
        Header.headers headers context.Self

    /// Custom header
    [<CustomOperation("header")>]
    member this.Header(context: IRequestContext<HeaderContext>, key, value) = 
        Header.header key value context.Self

    /// Content-Types that are acceptable for the response
    [<CustomOperation("Accept")>]
    member this.Accept(context: IRequestContext<HeaderContext>, contentType) = 
        Header.accept contentType context.Self

    /// Character sets that are acceptable
    [<CustomOperation("AcceptCharset")>]
    member this.AcceptCharset(context: IRequestContext<HeaderContext>, characterSets) =
        Header.acceptCharset characterSets context.Self

    /// Acceptable version in time
    [<CustomOperation("AcceptDatetime")>]
    member this.AcceptDatetime(context: IRequestContext<HeaderContext>, dateTime) =
        Header.acceptDatetime dateTime context.Self

    /// List of acceptable encodings. See HTTP compression.
    [<CustomOperation("AcceptEncoding")>]
    member this.AcceptEncoding(context: IRequestContext<HeaderContext>, encoding) =
        Header.acceptEncoding encoding context.Self

    /// List of acceptable human languages for response
    [<CustomOperation("AcceptLanguage")>]
    member this.AcceptLanguage(context: IRequestContext<HeaderContext>, language) =
        Header.acceptLanguage language context.Self

    /// Authorization credentials for HTTP authorization
    [<CustomOperation("Authorization")>]
    member this.Authorization(context: IRequestContext<HeaderContext>, credentials) =
        Header.authorization credentials context.Self

    /// Authorization header using Bearer authorization token
    [<CustomOperation("AuthorizationBearer")>]
    member this.AuthorizationBearer(context: IRequestContext<HeaderContext>, token) =
        Header.authorizationBearer token context.Self

    /// Authorization header using Basic (User/Password) authorization
    [<CustomOperation("AuthorizationUserPw")>]
    member this.AuthorizationUserPw(context: IRequestContext<HeaderContext>, username, password) =
        Header.authorizationUserPw username password context.Self

    /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
    [<CustomOperation("CacheControl")>]
    member this.CacheControl(context: IRequestContext<HeaderContext>, control) =
        Header.cacheControl control context.Self

    /// What type of connection the user-agent would prefer
    [<CustomOperation("Connection")>]
    member this.Connection(context: IRequestContext<HeaderContext>, connection) =
        Header.connection connection context.Self

    /// An HTTP cookie previously sent by the server with 'Set-Cookie'.
    [<CustomOperation("Cookie")>]
    member this.SetCookie(context: IRequestContext<HeaderContext>, name, value) = 
        Header.cookie name value context.Self

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies.
    [<CustomOperation("CookieForPath")>]
    member this.SetCookieForPath(context: IRequestContext<HeaderContext>, name, value, path) =
        Header.cookieForPath name value path context.Self

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies
    /// and the internet domain for which this Cookie is valid.
    [<CustomOperation("CookieForDomain")>]
    member this.SetCookieForDomain(context: IRequestContext<HeaderContext>, name, value, path, domain) =
        Header.cookieForDomain name value path domain context.Self

    /// The date and time that the message was sent
    [<CustomOperation("Date")>]
    member this.Date(context: IRequestContext<HeaderContext>, date) = 
        Header.date date context.Self

    /// Indicates that particular server behaviors are required by the client
    [<CustomOperation("Expect")>]
    member this.Expect(context: IRequestContext<HeaderContext>, behaviors) = 
        Header.expect behaviors context.Self

    /// Gives the date/time after which the response is considered stale
    [<CustomOperation("Expires")>]
    member this.Expires(context: IRequestContext<HeaderContext>, dateTime) = 
        Header.expires dateTime context.Self

    /// The email address of the user making the request
    [<CustomOperation("From")>]
    member this.From(context: IRequestContext<HeaderContext>, email) = 
        Header.from email context.Self

    /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
    /// The port number may be omitted if the port is the standard port for the service requested.
    [<CustomOperation("Host")>]
    member this.Host(context: IRequestContext<HeaderContext>, host) = 
        Header.host host context.Self

    /// Only perform the action if the client supplied entity matches the same entity on the server.
    /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
    [<CustomOperation("IfMatch")>]
    member this.IfMatch(context: IRequestContext<HeaderContext>, entity) = 
        Header.ifMatch entity context.Self

    /// Allows a 304 Not Modified to be returned if content is unchanged
    [<CustomOperation("IfModifiedSince")>]
    member this.IfModifiedSince(context: IRequestContext<HeaderContext>, dateTime) =
        Header.ifModifiedSince dateTime context.Self

    /// Allows a 304 Not Modified to be returned if content is unchanged
    [<CustomOperation("IfNoneMatch")>]
    member this.IfNoneMatch(context: IRequestContext<HeaderContext>, etag) = 
        Header.ifNoneMatch etag context.Self

    /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
    [<CustomOperation("IfRange")>]
    member this.IfRange(context: IRequestContext<HeaderContext>, range) = 
        Header.ifRange range context.Self

    /// Only send the response if the entity has not been modified since a specific time
    [<CustomOperation("IfUnmodifiedSince")>]
    member this.IfUnmodifiedSince(context: IRequestContext<HeaderContext>, dateTime) =
        Header.ifUnmodifiedSince dateTime context.Self

    /// Specifies a parameter used into order to maintain a persistent connection
    [<CustomOperation("KeepAlive")>]
    member this.KeepAlive(context: IRequestContext<HeaderContext>, keepAlive) = 
        Header.keepAlive keepAlive context.Self

    /// Specifies the date and time at which the accompanying body data was last modified
    [<CustomOperation("LastModified")>]
    member this.LastModified(context: IRequestContext<HeaderContext>, dateTime) =
        Header.lastModified dateTime context.Self

    /// Limit the number of times the message can be forwarded through proxies or gateways
    [<CustomOperation("MaxForwards")>]
    member this.MaxForwards(context: IRequestContext<HeaderContext>, count) = 
        Header.maxForwards count context.Self

    /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
    [<CustomOperation("Origin")>]
    member this.Origin(context: IRequestContext<HeaderContext>, origin) =
        Header.origin origin context.Self

    /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
    [<CustomOperation("Pragma")>]
    member this.Pragma(context: IRequestContext<HeaderContext>, pragma) =
        Header.pragma pragma context.Self

    /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
    [<CustomOperation("Prefer")>]
    member this.Prefer(context: IRequestContext<HeaderContext>, prefer) =
        Header.prefer prefer context.Self

    /// Authorization credentials for connecting to a proxy.
    [<CustomOperation("ProxyAuthorization")>]
    member this.ProxyAuthorization(context: IRequestContext<HeaderContext>, credentials) =
        Header.proxyAuthorization credentials context.Self

    /// Request only part of an entity. Bytes are numbered from 0
    [<CustomOperation("Range")>]
    member this.Range(context: IRequestContext<HeaderContext>, start, finish) = 
        Header.range start finish context.Self

    /// This is the address of the previous web page from which a link to the currently requested page was followed.
    /// (The word "referrer" is misspelled in the RFC as well as in most implementations.)
    [<CustomOperation("Referer")>]
    member this.Referer(context: IRequestContext<HeaderContext>, referer) = 
        Header.referer referer context.Self

    /// The transfer encodings the user agent is willing to accept: the same values as for the response header
    /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
    /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
    [<CustomOperation("TE")>]
    member this.TE(context: IRequestContext<HeaderContext>, te) = 
        Header.te te context.Self

    /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
    [<CustomOperation("Trailer")>]
    member this.Trailer(context: IRequestContext<HeaderContext>, trailer) = 
        Header.trailer trailer context.Self

    /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.
    /// The valid directives are one of: chunked, compress, deflate, gzip, orentity.
    [<CustomOperation("TransferEncoding")>]
    member this.TransferEncoding(context: IRequestContext<HeaderContext>, directive) =
        Header.transferEncoding directive context.Self

    /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
    [<CustomOperation("Translate")>]
    member this.Translate(context: IRequestContext<HeaderContext>, translate) = 
        Header.translate translate context.Self

    /// Specifies additional communications protocols that the client supports.
    [<CustomOperation("Upgrade")>]
    member this.Upgrade(context: IRequestContext<HeaderContext>, upgrade) = 
        Header.upgrade upgrade context.Self

    /// The user agent string of the user agent
    [<CustomOperation("UserAgent")>]
    member this.UserAgent(context: IRequestContext<HeaderContext>, userAgent) = 
        Header.userAgent userAgent context.Self

    /// Informs the server of proxies through which the request was sent
    [<CustomOperation("Via")>]
    member this.Via(context: IRequestContext<HeaderContext>, server) = 
        Header.via server context.Self

    /// A general warning about possible problems with the entity body
    [<CustomOperation("Warning")>]
    member this.Warning(context: IRequestContext<HeaderContext>, message) = 
        Header.warning message context.Self

    /// Override HTTP method.
    [<CustomOperation("XHTTPMethodOverride")>]
    member this.XHTTPMethodOverride(context: IRequestContext<HeaderContext>, httpMethod) =
        Header.xhttpMethodOverride httpMethod context.Self


// ---------
// Body
// ---------

type IRequestContext<'self> with

    /// An explicit transformation from a previous context to allow for describing the request body.
    [<CustomOperation("body")>]
    member this.Body(context: IRequestContext<#IToBodyContext>) = 
        context.Self.ToBodyContext()

    [<CustomOperation("content")>]
    member this.Content(context: IRequestContext<BodyContext>, contentType, data) =
        Body.content contentType data context.Self

    [<CustomOperation("binary")>]
    member this.Binary(context: IRequestContext<BodyContext>, data) = 
        Body.binary data context.Self

    [<CustomOperation("stream")>]
    member this.Stream(context: IRequestContext<BodyContext>, stream) = 
        Body.stream stream context.Self

    [<CustomOperation("enumerable")>]
    member this.Enumerable(context: IRequestContext<BodyContext>, data) = 
        Body.enumerable data context.Self

    [<CustomOperation("text")>]
    member this.Text(context: IRequestContext<BodyContext>, text) = 
        Body.text text context.Self

    [<CustomOperation("json")>]
    member this.Json(context: IRequestContext<BodyContext>, json) = 
        Body.json json context.Self

    [<CustomOperation("jsonSerializeWith")>]
    member this.JsonSerializeWith(context: IRequestContext<BodyContext>, options, instance) =
        Body.jsonSerializeWith options instance context.Self

    [<CustomOperation("jsonSerialize")>]
    member this.JsonSerialize(context: IRequestContext<BodyContext>, instance) =
        Body.jsonSerialize instance context.Self

    [<CustomOperation("formUrlEncoded")>]
    member this.FormUrlEncoded(context: IRequestContext<BodyContext>, data) = 
        Body.formUrlEncoded data context.Self

    [<CustomOperation("file")>]
    member this.File(context: IRequestContext<BodyContext>, path) = 
        Body.file path context.Self

    /// The type of encoding used on the data
    [<CustomOperation("ContentEncoding")>]
    member this.ContentEncoding(context: IRequestContext<BodyContext>, encoding) =
        Body.contentEncoding encoding context.Self

    /// The MIME type of the body of the request (used with POST and PUT requests) with an optional encoding
    [<CustomOperation("ContentType")>]
    member this.ContentType(context: IRequestContext<BodyContext>, contentType, ?charset) =
        Body.contentType contentType charset context.Self


// -----------------
// Multipart Element
// -----------------

type IRequestContext<'self> with

    /// The MIME type of the body of the request (used with POST and PUT requests) with an optional encoding
    [<CustomOperation("ContentType")>]
    member this.ContentType(context: IRequestContext<MultipartElementContext>, contentType, ?charset) =
        MultipartElement.contentType contentType charset context.Self


// ---------
// Multipart
// ---------

type IRequestContext<'self> with

    /// An explicit transformation from a previous context to allow for describing the request multiparts.
    [<CustomOperation("multipart")>]
    member this.Multipart(context: IRequestContext<#IToMultipartContext>) = 
        context.Self.ToMultipartContext()

    [<CustomOperation("textPart")>]
    member this.TextPart(context: IRequestContext<#IToMultipartContext>, value, name, ?fileName) =
        Multipart.textPart value name fileName context.Self

    [<CustomOperation("filePart")>]
    member this.FilePart(context: IRequestContext<#IToMultipartContext>, path, ?name, ?fileName) =
        Multipart.filePart path name fileName context.Self

    [<CustomOperation("binaryPart")>]
    member this.BinaryPart(context: IRequestContext<#IToMultipartContext>, value, name, ?fileName) =
        Multipart.binaryPart value name fileName context.Self

    [<CustomOperation("streamPart")>]
    member this.StreamPart(context: IRequestContext<#IToMultipartContext>, value, name, ?fileName) =
        Multipart.streamPart value name fileName context.Self

    [<CustomOperation("enumerablePart")>]
    member this.EnumerablePart(context: IRequestContext<#IToMultipartContext>, value, name, ?fileName) =
        Multipart.enumerablePart value name fileName context.Self


// ---------
// Config
// ---------

type IRequestContext<'self> with

    [<CustomOperation("config_update")>]
    member this.Update(context: IRequestContext<#IUpdateConfig<_>>, transformer) =
        Config.update transformer context.Self

    [<CustomOperation("config_set")>]
    member this.Set(context: IRequestContext<#IUpdateConfig<_>>, transformer) =
        Config.set transformer context.Self

    // ----------------

    // TODO: Provide certStrategy configs
    [<CustomOperation("config_ignoreCertIssues")>]
    member this.IgnoreCertIssues(context: IRequestContext<#IUpdateConfig<_>>) =
        Config.ignoreCertIssues context.Self

    [<CustomOperation("config_timeout")>]
    member this.Timeout(context: IRequestContext<#IUpdateConfig<_>>, value) =
        Config.timeout value context.Self

    [<CustomOperation("config_noTimeout")>]
    member this.NoTimeout(context: IRequestContext<#IUpdateConfig<_>>) =
        Config.noTimeout context.Self

    [<CustomOperation("config_timeoutInSeconds")>]
    member this.TimeoutInSeconds(context: IRequestContext<#IUpdateConfig<_>>, value) =
        Config.timeoutInSeconds value context.Self

    [<CustomOperation("config_transformHeader")>]
    member this.TransformHeader(context: IRequestContext<#IUpdateConfig<_>>, transformer) =
        Config.transformHeader transformer context.Self

    [<CustomOperation("config_transformUrl")>]
    member this.TransformUrl(context: IRequestContext<#IUpdateConfig<_>>, transformer) =
        Config.transformUrl transformer context.Self
    
    [<CustomOperation("config_useBaseUrl")>]
    member this.UseBaseUrl(context: IRequestContext<#IUpdateConfig<_>>, baseUrl) =
        Config.useBaseUrl baseUrl context.Self

    [<CustomOperation("config_setHttpClientFactory")>]
    member this.SetHttpClientFactory(context: IRequestContext<#IUpdateConfig<_>>, httpClientFactory) =
        Config.setHttpClientFactory httpClientFactory context.Self

    [<CustomOperation("config_transformHttpClient")>]
    member this.TransformHttpClient(context: IRequestContext<#IUpdateConfig<_>>, transformer) =
        Config.transformHttpClient transformer context.Self

    [<CustomOperation("config_transformHttpRequestMessage")>]
    member this.TransformHttpRequestMessage(context: IRequestContext<#IUpdateConfig<_>>, transformer) =
        Config.transformHttpRequestMessage transformer context.Self

    [<CustomOperation("config_transformHttpClientHandler")>]
    member this.TransformHttpClientHandler(context: IRequestContext<#IUpdateConfig<_>>, transformer) =
        Config.transformHttpClientHandler transformer context.Self

    [<CustomOperation("config_proxy")>]
    member this.Proxy(context: IRequestContext<#IUpdateConfig<_>>, url) = 
        Config.proxy url context.Self

    [<CustomOperation("config_proxyWithCredentials")>]
    member this.ProxyWithCredentials(context: IRequestContext<#IUpdateConfig<_>>, url, credentials) =
        Config.proxyWithCredentials url credentials context.Self

    [<CustomOperation("config_decompressionMethods")>]
    member this.DecompressionMethods
        (
            context: IRequestContext<#IUpdateConfig<_>>,
            decompressionMethods
        ) =
        Config.decompressionMethods decompressionMethods context.Self

    [<CustomOperation("config_noDecompression")>]
    member this.NoDecompression(context: IRequestContext<#IUpdateConfig<_>>) =
        Config.noDecompression context.Self

    [<CustomOperation("config_cancellationToken")>]
    member this.CancellationToken(context: IRequestContext<#IUpdateConfig<_>>, cancellationToken) =
        Config.cancellationToken cancellationToken context.Self


// ---------
// Print
// ---------

type IRequestContext<'self> with

    [<CustomOperation("print_update")>]
    member this.Update(context: IRequestContext<#IUpdatePrintHint<_>>, transformer) =
        Print.update transformer context.Self

    [<CustomOperation("print_set")>]
    member this.Set(context: IRequestContext<#IUpdatePrintHint<_>>, printHint) =
        Print.set printHint context.Self

    // ----------------

    [<CustomOperation("print_withRequestPrintMode")>]
    member this.WithRequestPrintMode(context: IRequestContext<#IUpdatePrintHint<_>>, updatePrintMode) =
        Print.withRequestPrintMode updatePrintMode context.Self

    [<CustomOperation("print_withResponsePrintMode")>]
    member this.WithResponsePrintMode(context: IRequestContext<#IUpdatePrintHint<_>>, updatePrintMode) =
        Print.withResponsePrintMode updatePrintMode context.Self

    [<CustomOperation("print_withResponseBody")>]
    member this.WithResponseBody(context: IRequestContext<#IUpdatePrintHint<_>>, updateBodyPrintMode) =
        Print.withResponseBody updateBodyPrintMode context.Self

    [<CustomOperation("print_useObjectFormatting")>]
    member this.UseObjectFormatting(context: IRequestContext<#IUpdatePrintHint<_>>) =
        Print.useObjectFormatting context.Self

    [<CustomOperation("print_headerOnly")>]
    member this.HeaderOnly(context: IRequestContext<#IUpdatePrintHint<_>>) =
        Print.headerOnly context.Self

    [<CustomOperation("print_withResponseBodyLength")>]
    member this.WithResponseBodyLength(context: IRequestContext<#IUpdatePrintHint<_>>, maxLength) =
        Print.withResponseBodyLength maxLength context.Self

    [<CustomOperation("print_withResponseBodyFormat")>]
    member this.WithResponseBodyFormat(context: IRequestContext<#IUpdatePrintHint<_>>, format) =
        Print.withResponseBodyFormat format context.Self

    [<CustomOperation("print_withResponseBodyExpanded")>]
    member this.WithResponseBodyExpanded(context: IRequestContext<#IUpdatePrintHint<_>>) =
        Print.withResponseBodyExpanded context.Self

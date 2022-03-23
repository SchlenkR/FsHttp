[<AutoOpen>]
module FsHttp.DslCE

open System.Net
open FsHttp

    
// ---------
// Builder instances
// ---------

type IRequestContext<'self> with
    member this.Yield(_) = this

type StartingContext =
    { config: Config option }
    member this.ActualConfig =
        this.config |> Option.defaultValue GlobalConfig.mutableDefaults
    interface IRequestContext<StartingContext> with
        member this.Self = this
    interface IConfigure<ConfigTransformer, StartingContext> with
        member this.Configure(transformConfig) =
            { this with config = Some (transformConfig this.ActualConfig) }
    interface IConfigure<PrintHintTransformer, StartingContext> with
        member this.Configure(transformPrintHint) =
            configPrinter this transformPrintHint
    
let http = { config = None }


// ---------
// Methods
// ---------
    
type IRequestContext<'self> with
    
    [<CustomOperation("Method")>]
    member this.Method(_: IRequestContext<StartingContext>, method, url) =
        Http.method method url
    
    // RFC 2626 specifies 8 methods
    [<CustomOperation("GET")>]
    member this.Get(context: IRequestContext<StartingContext>, url) =
        getWithConfig context.Self.ActualConfig url
    
    [<CustomOperation("PUT")>]
    member this.Put(context: IRequestContext<StartingContext>, url) =
        putWithConfig context.Self.ActualConfig url
    
    [<CustomOperation("POST")>]
    member this.Post(context: IRequestContext<StartingContext>, url) =
        postWithConfig context.Self.ActualConfig url
    
    [<CustomOperation("DELETE")>]
    member this.Delete(context: IRequestContext<StartingContext>, url) =
        deleteWithConfig context.Self.ActualConfig url
    
    [<CustomOperation("OPTIONS")>]
    member this.Options(context: IRequestContext<StartingContext>, url) =
        optionsWithConfig context.Self.ActualConfig url
    
    [<CustomOperation("HEAD")>]
    member this.Head(context: IRequestContext<StartingContext>, url) =
        headWithConfig context.Self.ActualConfig url
    
    [<CustomOperation("TRACE")>]
    member this.Trace(context: IRequestContext<StartingContext>, url) =
        traceWithConfig context.Self.ActualConfig url
    
    [<CustomOperation("CONNECT")>]
    member this.Connect(context: IRequestContext<StartingContext>, url) =
        connectWithConfig context.Self.ActualConfig url
    
    [<CustomOperation("PATCH")>]
    member this.Patch(context: IRequestContext<StartingContext>, url) =
        patchWithConfig context.Self.ActualConfig url
    
    
// ---------
// Headers
// ---------
    
type IRequestContext<'self> with
    
    /// Append query params
    [<CustomOperation("query")>]
    member this.Query(context: IRequestContext<HeaderContext>, queryParams) =
        Header.query queryParams context.Self
    
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
    member this.Body(context: IRequestContext<#IToBodyContext>) 
        = context.Self.Transform()
    
    [<CustomOperation("content")>]
    member this.Content(context: IRequestContext<BodyContext>, contentType, data) =
        Body.content contentType data context.Self
    
    [<CustomOperation("binary")>]
    member this.Binary(context: IRequestContext<BodyContext>, data) =
        Body.binary data context.Self
    
    [<CustomOperation("stream")>]
    member this.Stream(context: IRequestContext<BodyContext>, stream) =
        Body.stream stream context.Self
    
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
    
    /// The MIME type of the body of the request (used with POST and PUT requests)
    [<CustomOperation("ContentType")>]
    member this.ContentType(context: IRequestContext<BodyContext>, contentType) =
        Body.contentType contentType context.Self
    
    /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
    [<CustomOperation("ContentTypeWithEncoding")>]
    member this.ContentTypeWithEncoding(context: IRequestContext<BodyContext>, contentType, charset) =
        Body.contentTypeWithEncoding contentType charset context.Self
    
    
// ---------
// Multipart
// ---------
    
type IRequestContext<'self> with
    
    /// The MIME type of the body of the request (used with POST and PUT requests)
    [<CustomOperation("ContentTypeForPart")>]
    member this.ContentTypeForPart(context: IRequestContext<MultipartContext>, contentType) =
        Multipart.contentType contentType context.Self
    
    // -----
    // PARTS
    // -----
    
    /// An explicit transformation from a previous context to allow for describing the request multiparts.
    [<CustomOperation("multipart")>]
    member this.Multipart(context: IRequestContext<#IToMultipartContext>)
        = context.Self.Transform()
    
    [<CustomOperation("part")>]
    member this.Part(context: IRequestContext<MultipartContext>, content, defaultContentType, name) =
        Multipart.part content defaultContentType name context.Self
    
    [<CustomOperation("stringPart")>]
    member this.StringPart(context: IRequestContext<MultipartContext>, name, value) =
        Multipart.stringPart name value context.Self
    
    [<CustomOperation("filePartWithName")>]
    member this.FilePartWithName(context: IRequestContext<MultipartContext>, name, path) =
        Multipart.filePartWithName name path context.Self
    
    [<CustomOperation("filePart")>]
    member this.FilePart(context: IRequestContext<MultipartContext>, path) =
        Multipart.filePart path context.Self
    
    [<CustomOperation("byteArrayPart")>]
    member this.ByteArrayPart(context: IRequestContext<MultipartContext>, name, value) =
        Multipart.byteArrayPart name value context.Self
    
    [<CustomOperation("streamPart")>]
    member this.StreamPart(context: IRequestContext<MultipartContext>, name, value) =
        Multipart.streamPart name value context.Self


// ---------
// Config
// ---------

type IRequestContext<'self> with
    
    [<CustomOperation("config_update")>]
    member inline this.Update(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, configTransformer) =
        Config.update configTransformer context.Self
    
    [<CustomOperation("config_set")>]
    member inline this.Set(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, configTransformer) =
        Config.set configTransformer context.Self
    
    // TODO: Provide certStrategy configs
    [<CustomOperation("config_ignoreCertIssues")>]
    member inline this.IgnoreCertIssues(context: IRequestContext<#IConfigure<ConfigTransformer, _>>) =
        Config.ignoreCertIssues context.Self
    
    [<CustomOperation("config_timeout")>]
    member inline this.Timeout(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, value) =
        Config.timeout value context.Self
    
    [<CustomOperation("config_timeoutInSeconds")>]
    member inline this.TimeoutInSeconds(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, value) =
        Config.timeoutInSeconds value context.Self
    
    [<CustomOperation("config_setHttpClient")>]
    member inline this.SetHttpClient(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, httpClient) =
        Config.setHttpClient httpClient context.Self
    
    [<CustomOperation("config_setHttpClientFactory")>]
    member inline this.SetHttpClientFactory(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, httpClientFactory) =
        Config.setHttpClientFactory httpClientFactory context.Self
    
    [<CustomOperation("config_transformHttpClient")>]
    member inline this.TransformHttpClient(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, transformer) =
        Config.transformHttpClient transformer context.Self
    
    [<CustomOperation("config_transformHttpRequestMessage")>]
    member inline this.TransformHttpRequestMessage(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, transformer) =
        Config.transformHttpRequestMessage transformer context.Self
    
    [<CustomOperation("config_transformHttpClientHandler")>]
    member inline this.TransformHttpClientHandler(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, transformer) =
        Config.transformHttpClientHandler transformer context.Self
    
    [<CustomOperation("config_proxy")>]
    member inline this.Proxy(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, url) =
        Config.proxy url context.Self
    
    [<CustomOperation("config_proxyWithCredentials")>]
    member inline this.ProxyWithCredentials(context: IRequestContext<#IConfigure<ConfigTransformer, _>>, url, credentials) =
        Config.proxyWithCredentials url credentials context.Self
    
    
// ---------
// Print
// ---------
    
type IRequestContext<'self> with

    [<CustomOperation("print_withConfig")>]
    member inline this.WithConfig(context: IRequestContext<#IConfigure<PrintHintTransformer, _>>, updatePrintHint) =
        Print.withConfig updatePrintHint context.Self
    
    [<CustomOperation("print_withRequestPrintMode")>]
    member inline this.WithRequestPrintMode(context: IRequestContext<#IConfigure<PrintHintTransformer, _>>, updatePrintMode) =
        Print.withRequestPrintMode updatePrintMode context.Self
    
    [<CustomOperation("print_withResponsePrintMode")>]
    member inline this.WithResponsePrintMode(context: IRequestContext<#IConfigure<PrintHintTransformer, _>>, updatePrintMode) =
        Print.withResponsePrintMode updatePrintMode context.Self
    
    [<CustomOperation("print_withResponseBody")>]
    member inline this.WithResponseBody(context: IRequestContext<#IConfigure<PrintHintTransformer, _>>, updateBodyPrintMode) =
        Print.withResponseBody updateBodyPrintMode context.Self
        
    [<CustomOperation("print_useObjectFormatting")>]
    member inline this.UseObjectFormatting(context: IRequestContext<#IConfigure<PrintHintTransformer, _>>) =
        Print.useObjectFormatting context.Self
        
    [<CustomOperation("print_headerOnly")>]
    member inline this.HeaderOnly(context: IRequestContext<#IConfigure<PrintHintTransformer, _>>) =
        Print.headerOnly context.Self
        
    [<CustomOperation("print_withResponseBodyLength")>]
    member inline this.WithResponseBodyLength(context: IRequestContext<#IConfigure<PrintHintTransformer, _>>, maxLength) =
        Print.withResponseBodyLength maxLength context.Self
        
    [<CustomOperation("print_withResponseBodyFormat")>]
    member inline this.WithResponseBodyFormat(context: IRequestContext<#IConfigure<PrintHintTransformer, _>>, format) =
        Print.withResponseBodyFormat format context.Self
        
    [<CustomOperation("print_withResponseBodyExpanded")>]
    member inline this.WithResponseBodyExpanded(context: IRequestContext<#IConfigure<PrintHintTransformer, _>>) =
        Print.withResponseBodyExpanded context.Self

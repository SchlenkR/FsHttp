namespace FsHttp.CSharp

open System
open System.Runtime.CompilerServices
open FsHttp


// ---------
// Methods
// ---------

[<Extension>]
type Http =
    [<Extension>]
    static member Method(url, method) = Http.method method url
    
    [<Extension>] static member Get(url) = get url
    [<Extension>] static member Get(uri: Uri) = get (uri.ToString())
    
    [<Extension>] static member Put(url) = put url
    [<Extension>] static member Put(uri: Uri) = put (uri.ToString())
    
    [<Extension>] static member Post(url) = post url
    [<Extension>] static member Post(uri: Uri) = post (uri.ToString())
    
    [<Extension>] static member Delete(url) = delete url
    [<Extension>] static member Delete(uri: Uri) = delete (uri.ToString())
    
    [<Extension>] static member Options(url) = options url
    [<Extension>] static member Options(uri: Uri) = options (uri.ToString())
    
    [<Extension>] static member Head(url) = head url
    [<Extension>] static member Head(uri: Uri) = head (uri.ToString())
    
    [<Extension>] static member Trace(url) = trace url
    [<Extension>] static member Trace(uri: Uri) = trace (uri.ToString())
    
    [<Extension>] static member Connect(url) = connect url
    [<Extension>] static member Connect(uri: Uri) = connect (uri.ToString())
    
    [<Extension>] static member Patch(url) = patch url
    [<Extension>] static member Patch(uri: Uri) = patch (uri.ToString())


// ---------
// Headers
// ---------

[<Extension>]
type Header =

    /// Append query params
    [<Extension>]
    static member Query(context: IRequestContext<HeaderContext>, queryParams) =
        Header.query (queryParams |> Seq.toList) context.Self

    /// Custom header
    [<Extension>]
    static member Header(context: IRequestContext<HeaderContext>, key, value) =
        Header.header key value context.Self

    /// Content-Types that are acceptable for the response
    [<Extension>]
    static member Accept(context: IRequestContext<HeaderContext>, contentType) =
        Header.accept contentType context.Self

    /// Character sets that are acceptable
    [<Extension>]
    static member AcceptCharset(context: IRequestContext<HeaderContext>, characterSets) =
        Header.acceptCharset characterSets context.Self

    /// Acceptable version in time
    [<Extension>]
    static member AcceptDatetime(context: IRequestContext<HeaderContext>, dateTime) =
        Header.acceptDatetime dateTime context.Self

    /// List of acceptable encodings. See HTTP compression.
    [<Extension>]
    static member AcceptEncoding(context: IRequestContext<HeaderContext>, encoding) =
        Header.acceptEncoding encoding context.Self

    /// List of acceptable human languages for response
    [<Extension>]
    static member AcceptLanguage(context: IRequestContext<HeaderContext>, language) =
        Header.acceptLanguage language context.Self
        
    /// Authorization credentials for HTTP authorization
    [<Extension>]
    static member Authorization(context: IRequestContext<HeaderContext>, credentials) =
        Header.authorization credentials context.Self

    /// Authorization header using Bearer authorization token
    [<Extension>]
    static member AuthorizationBearer(context: IRequestContext<HeaderContext>, token) =
        Header.authorizationBearer token context.Self

    /// Authorization header using Basic (User/Password) authorization
    [<Extension>]
    static member AuthorizationUserPw(context: IRequestContext<HeaderContext>, username, password) =
        Header.authorizationUserPw username password context.Self

    /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
    [<Extension>]
    static member CacheControl(context: IRequestContext<HeaderContext>, control) =
        Header.cacheControl control context.Self

    /// What type of connection the user-agent would prefer
    [<Extension>]
    static member Connection(context: IRequestContext<HeaderContext>, connection) =
        Header.connection connection context.Self

    /// An HTTP cookie previously sent by the server with 'Set-Cookie'.
    [<Extension>]
    static member SetCookie(context: IRequestContext<HeaderContext>, name, value) =
        Header.cookie name value context.Self

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies.
    [<Extension>]
    static member SetCookieForPath(context: IRequestContext<HeaderContext>, name, value, path) =
        Header.cookieForPath name value path context.Self

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies
    /// and the internet domain for which this Cookie is valid.
    [<Extension>]
    static member SetCookieForDomain(context: IRequestContext<HeaderContext>, name, value, path, domain) =
        Header.cookieForDomain name value path domain context.Self

    /// The date and time that the message was sent
    [<Extension>]
    static member Date(context: IRequestContext<HeaderContext>, date) =
        Header.date date context.Self

    /// Indicates that particular server behaviors are required by the client
    [<Extension>]
    static member Expect(context: IRequestContext<HeaderContext>, behaviors) =
        Header.expect behaviors context.Self

    /// Gives the date/time after which the response is considered stale
    [<Extension>]
    static member Expires(context: IRequestContext<HeaderContext>, dateTime) =
        Header.expires dateTime context.Self

    /// The email address of the user making the request
    [<Extension>]
    static member From(context: IRequestContext<HeaderContext>, email) =
        Header.from email context.Self

    /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
    /// The port number may be omitted if the port is the standard port for the service requested.
    [<Extension>]
    static member Host(context: IRequestContext<HeaderContext>, host) =
        Header.host host context.Self

    /// Only perform the action if the client supplied entity matches the same entity on the server.
    /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
    [<Extension>]
    static member IfMatch(context: IRequestContext<HeaderContext>, entity) =
        Header.ifMatch entity context.Self

    /// Allows a 304 Not Modified to be returned if content is unchanged
    [<Extension>]
    static member IfModifiedSince(context: IRequestContext<HeaderContext>, dateTime) =
        Header.ifModifiedSince dateTime context.Self

    /// Allows a 304 Not Modified to be returned if content is unchanged
    [<Extension>]
    static member IfNoneMatch(context: IRequestContext<HeaderContext>, etag) =
        Header.ifNoneMatch etag context.Self

    /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
    [<Extension>]
    static member IfRange(context: IRequestContext<HeaderContext>, range) =
        Header.ifRange range context.Self

    /// Only send the response if the entity has not been modified since a specific time
    [<Extension>]
    static member IfUnmodifiedSince(context: IRequestContext<HeaderContext>, dateTime) =
        Header.ifUnmodifiedSince dateTime context.Self

    /// Specifies a parameter used into order to maintain a persistent connection
    [<Extension>]
    static member KeepAlive(context: IRequestContext<HeaderContext>, keepAlive) =
        Header.keepAlive keepAlive context.Self

    /// Specifies the date and time at which the accompanying body data was last modified
    [<Extension>]
    static member LastModified(context: IRequestContext<HeaderContext>, dateTime) =
        Header.lastModified dateTime context.Self

    /// Limit the number of times the message can be forwarded through proxies or gateways
    [<Extension>]
    static member MaxForwards(context: IRequestContext<HeaderContext>, count) =
        Header.maxForwards count context.Self

    /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
    [<Extension>]
    static member Origin(context: IRequestContext<HeaderContext>, origin) =
        Header.origin origin context.Self

    /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
    [<Extension>]
    static member Pragma(context: IRequestContext<HeaderContext>, pragma) =
        Header.pragma pragma context.Self

    /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
    [<Extension>]
    static member Prefer(context: IRequestContext<HeaderContext>, prefer) =
        Header.prefer prefer context.Self

    /// Authorization credentials for connecting to a proxy.
    [<Extension>]
    static member ProxyAuthorization(context: IRequestContext<HeaderContext>, credentials) =
        Header.proxyAuthorization credentials context.Self

    /// Request only part of an entity. Bytes are numbered from 0
    [<Extension>]
    static member Range(context: IRequestContext<HeaderContext>, start, finish) =
        Header.range start finish context.Self

    /// This is the address of the previous web page from which a link to the currently requested page was followed.
    /// (The word "referrer" is misspelled in the RFC as well as in most implementations.)
    [<Extension>]
    static member Referer(context: IRequestContext<HeaderContext>, referer) =
        Header.referer referer context.Self

    /// The transfer encodings the user agent is willing to accept: the same values as for the response header
    /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
    /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
    [<Extension>]
    static member TE(context: IRequestContext<HeaderContext>, te) =
        Header.te te context.Self

    /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
    [<Extension>]
    static member Trailer(context: IRequestContext<HeaderContext>, trailer) =
        Header.trailer trailer context.Self

    /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.
    /// The valid directives are one of: chunked, compress, deflate, gzip, orentity.
    [<Extension>]
    static member TransferEncoding(context: IRequestContext<HeaderContext>, directive) =
        Header.transferEncoding directive context.Self

    /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
    [<Extension>]
    static member Translate(context: IRequestContext<HeaderContext>, translate) =
        Header.translate translate context.Self

    /// Specifies additional communications protocols that the client supports.
    [<Extension>]
    static member Upgrade(context: IRequestContext<HeaderContext>, upgrade) =
        Header.upgrade upgrade context.Self

    /// The user agent string of the user agent
    [<Extension>]
    static member UserAgent(context: IRequestContext<HeaderContext>, userAgent) =
        Header.userAgent userAgent context.Self

    /// Informs the server of proxies through which the request was sent
    [<Extension>]
    static member Via(context: IRequestContext<HeaderContext>, server) =
        Header.via server context.Self

    /// A general warning about possible problems with the entity body
    [<Extension>]
    static member Warning(context: IRequestContext<HeaderContext>, message) =
        Header.warning message context.Self

    /// Override HTTP method.
    [<Extension>]
    static member XHTTPMethodOverride(context: IRequestContext<HeaderContext>, httpMethod) =
        Header.xhttpMethodOverride httpMethod context.Self
    

// ---------
// Body
// ---------

[<Extension>]
type Body =

    /// An explicit transformation from a previous context to allow for describing the request body.
    [<Extension>]
    static member Body(context: IRequestContext<#IToBodyContext>) 
        = context.Self.Transform()

    [<Extension>]
    static member Content(context: IRequestContext<BodyContext>, contentType, data) =
        Body.content contentType data context.Self

    [<Extension>]
    static member Binary(context: IRequestContext<BodyContext>, data) =
        Body.binary data context.Self

    [<Extension>]
    static member Stream(context: IRequestContext<BodyContext>, stream) =
        Body.stream stream context.Self

    [<Extension>]
    static member Text(context: IRequestContext<BodyContext>, text) =
        Body.text text context.Self

    [<Extension>]
    static member Json(context: IRequestContext<BodyContext>, json) =
        Body.json json context.Self

    [<Extension>]
    static member JsonSerializeWith(context: IRequestContext<BodyContext>, options, json) =
        Body.jsonSerializeWith options json context.Self

    [<Extension>]
    static member JsonSerialize(context: IRequestContext<BodyContext>, json) =
        Body.jsonSerialize json context.Self

    [<Extension>]
    static member FormUrlEncoded(context: IRequestContext<BodyContext>, data) =
        Body.formUrlEncoded data context.Self

    [<Extension>]
    static member File(context: IRequestContext<BodyContext>, path) =
        Body.file path context.Self

    /// The type of encoding used on the data
    [<Extension>]
    static member ContentEncoding(context: IRequestContext<BodyContext>, encoding) =
        Body.contentEncoding encoding context.Self

    /// The MIME type of the body of the request (used with POST and PUT requests)
    [<Extension>]
    static member ContentType(context: IRequestContext<BodyContext>, contentType) =
        Body.contentType contentType context.Self

    /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
    [<Extension>]
    static member ContentTypeWithEncoding(context: IRequestContext<BodyContext>, contentType, charset) =
        Body.contentTypeWithEncoding contentType charset context.Self


// ---------
// Multipart
// ---------

[<Extension>]
type Multipart =

    /// The MIME type of the body of the request (used with POST and PUT requests)
    [<Extension>]
    static member ContentTypeForPart(context: IRequestContext<MultipartContext>, contentType) =
        Multipart.contentType contentType context.Self

    // -----
    // PARTS
    // -----

    /// An explicit transformation from a previous context to allow for describing the request multiparts.
    [<Extension>]
    static member Multipart(context: IRequestContext<#IToMultipartContext>)
        = context.Self.Transform()

    [<Extension>]
    static member Part(context: IRequestContext<MultipartContext>, content, defaultContentType, name) =
        Multipart.part content defaultContentType name context.Self

    [<Extension>]
    static member StringPart(context: IRequestContext<MultipartContext>, name, value) =
        Multipart.stringPart name value context.Self

    [<Extension>]
    static member FilePartWithName(context: IRequestContext<MultipartContext>, name, path) =
        Multipart.filePartWithName name path context.Self

    [<Extension>]
    static member FilePart(context: IRequestContext<MultipartContext>, path) =
        Multipart.filePart path context.Self

    [<Extension>]
    static member ByteArrayPart(context: IRequestContext<MultipartContext>, name, value) =
        Multipart.byteArrayPart name value context.Self

    [<Extension>]
    static member StreamPart(context: IRequestContext<MultipartContext>, name, value) =
        Multipart.streamPart name value context.Self


// ---------
// Config
// ---------

type ConfigTransformer = Func<Domain.Config, Domain.Config>

[<Extension>]
type Config =
    
    [<Extension>]
    static member Configure(context: HeaderContext, configTransformer: ConfigTransformer) =
        Config.update configTransformer.Invoke context
    
    [<Extension>]
    static member Configure(context: BodyContext, configTransformer: ConfigTransformer) =
        Config.update configTransformer.Invoke context
    
    [<Extension>]
    static member Configure(context: MultipartContext, configTransformer: ConfigTransformer) =
        Config.update configTransformer.Invoke context

    // ----------------
    
    [<Extension>]
    static member IgnoreCertIssues(config: Domain.Config) =
        Config.With.ignoreCertIssues config
    
    [<Extension>]
    static member Timeout(config: Domain.Config, value) =
        Config.With.timeout value config
    
    [<Extension>]
    static member TimeoutInSeconds(config: Domain.Config, value) =
        Config.With.timeoutInSeconds value config
    
    [<Extension>]
    static member SetHttpClient(config: Domain.Config, httpClient) =
        Config.With.setHttpClient httpClient config
    
    [<Extension>]
    static member SetHttpClientFactory(config: Domain.Config, httpClientFactory) =
        Config.With.setHttpClientFactory httpClientFactory config
    
    [<Extension>]
    static member TransformHttpClient(config: Domain.Config, transformer) =
        Config.With.transformHttpClient transformer config
    
    [<Extension>]
    static member TransformHttpRequestMessage(config: Domain.Config, transformer) =
        Config.With.transformHttpRequestMessage transformer config
    
    [<Extension>]
    static member TransformHttpClientHandler(config: Domain.Config, transformer) =
        Config.With.transformHttpClientHandler transformer config
    
    [<Extension>]
    static member Proxy(config: Domain.Config, url) =
        Config.With.proxy url config
    
    [<Extension>]
    static member ProxyWithCredentials(config: Domain.Config, url, credentials) =
        Config.With.proxyWithCredentials url credentials config
    

// ---------
// Request
// ---------

[<Extension>]
type Request =
    
    [<Extension>]
    static member ToHttpRequestMessage(request: IToRequest) =
        Request.toHttpRequestMessage request

    [<Extension>]
    static member SendAsync(request: IToRequest) =
        Request.sendTAsync request


// ---------
// Request
// ---------

[<Extension>] 
type Response =
    
    [<Extension>]
    static member ToStreamAsync(response: Domain.Response) =
        Response.toStreamTAsync response
    
    [<Extension>]
    static member ToBytesAsync(response: Domain.Response) =
        Response.toBytesTAsync response
    
    [<Extension>]
    static member ToStringAsync(response: Domain.Response, maxLength) =
        Response.toStringTAsync maxLength response
    
    [<Extension>]
    static member ToTextAsync(response: Domain.Response) =
        Response.toTextTAsync response

    [<Extension>]
    static member ToXmlAsync(response: Domain.Response) =
        Response.toXmlTAsync response

    [<Extension>]
    static member ToJsonDocumentWithAsync(response: Domain.Response, options) =
        Response.toJsonDocumentWithTAsync options response

    [<Extension>]
    static member ToJsonDocumentAsync(response: Domain.Response) =
        Response.toJsonDocumentTAsync response

    [<Extension>]
    static member ToJsonWithAsync(response: Domain.Response, options) =
        Response.toJsonWithTAsync options response

    [<Extension>]
    static member ToJsonAsync(response: Domain.Response) =
        Response.toJsonTAsync response

    [<Extension>]
    static member ToJsonEnumerableWithAsync(response: Domain.Response, options) =
        Response.toJsonSeqWithTAsync options response

    [<Extension>]
    static member ToJsonEnumerableAsync(response: Domain.Response) =
        Response.toJsonSeqTAsync response

    [<Extension>]
    static member DeserializeJsonWithAsync<'T>(response: Domain.Response, options) =
        Response.deserializeJsonWithTAsync options response

    [<Extension>]
    static member DeserializeJsonAsync(response: Domain.Response) =
        Response.deserializeJsonTAsync response

    [<Extension>]
    static member ToFormattedTextAsync(response: Domain.Response) =
        Response.toFormattedTextTAsync response

    [<Extension>]
    static member SaveFileAsync(response: Domain.Response, fileName) =
        Response.saveFileTAsync fileName response

    [<Extension>]
    static member AssertStatusCodes(response: Domain.Response, statusCodes) =
        Response.assertStatusCodes (statusCodes |> Seq.toList) response

    [<Extension>]
    static member AssertStatusCode(response: Domain.Response, statusCode) =
        Response.assertStatusCode statusCode response

    [<Extension>]
    static member AssertOk(response: Domain.Response) =
        Response.assertOk response



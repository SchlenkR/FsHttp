namespace FsHttp

open System
open System.Runtime.CompilerServices
open System.Threading
open FsHttp


// ---------
// Methods
// ---------

[<Extension>]
type Http =

    [<Extension>]
    static member Method(url, method) = Http.method method url

    [<Extension>]
    static member Get(url) = get url

    [<Extension>]
    static member Get(uri: Uri) = get (uri.ToString())

    [<Extension>]
    static member Put(url) = put url

    [<Extension>]
    static member Put(uri: Uri) = put (uri.ToString())

    [<Extension>]
    static member Post(url) = post url

    [<Extension>]
    static member Post(uri: Uri) = post (uri.ToString())

    [<Extension>]
    static member Delete(url) = delete url

    [<Extension>]
    static member Delete(uri: Uri) = delete (uri.ToString())

    [<Extension>]
    static member Options(url) = options url

    [<Extension>]
    static member Options(uri: Uri) = options (uri.ToString())

    [<Extension>]
    static member Head(url) = head url

    [<Extension>]
    static member Head(uri: Uri) = head (uri.ToString())

    [<Extension>]
    static member Trace(url) = trace url

    [<Extension>]
    static member Trace(uri: Uri) = trace (uri.ToString())

    [<Extension>]
    static member Connect(url) = connect url

    [<Extension>]
    static member Connect(uri: Uri) = connect (uri.ToString())

    [<Extension>]
    static member Patch(url) = patch url

    [<Extension>]
    static member Patch(uri: Uri) = patch (uri.ToString())


// ---------
// Headers
// ---------

[<Extension>]
type HeaderExtensions =

    /// Append query params
    [<Extension>]
    static member Query(context: HeaderContext, queryParams) =
        Header.query (queryParams |> Seq.toList) context

    /// Custom header
    [<Extension>]
    static member Header(context: HeaderContext, key, value) =
        Header.header key value context

    /// Content-Types that are acceptable for the response
    [<Extension>]
    static member Accept(context: HeaderContext, contentType) =
        Header.accept contentType context

    /// Character sets that are acceptable
    [<Extension>]
    static member AcceptCharset(context: HeaderContext, characterSets) =
        Header.acceptCharset characterSets context

    /// Acceptable version in time
    [<Extension>]
    static member AcceptDatetime(context: HeaderContext, dateTime) =
        Header.acceptDatetime dateTime context

    /// List of acceptable encodings. See HTTP compression.
    [<Extension>]
    static member AcceptEncoding(context: HeaderContext, encoding) =
        Header.acceptEncoding encoding context

    /// List of acceptable human languages for response
    [<Extension>]
    static member AcceptLanguage(context: HeaderContext, language) =
        Header.acceptLanguage language context

    /// Authorization credentials for HTTP authorization
    [<Extension>]
    static member Authorization(context: HeaderContext, credentials) =
        Header.authorization credentials context

    /// Authorization header using Bearer authorization token
    [<Extension>]
    static member AuthorizationBearer(context: HeaderContext, token) =
        Header.authorizationBearer token context

    /// Authorization header using Basic (User/Password) authorization
    [<Extension>]
    static member AuthorizationUserPw(context: HeaderContext, username, password) =
        Header.authorizationUserPw username password context

    /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
    [<Extension>]
    static member CacheControl(context: HeaderContext, control) =
        Header.cacheControl control context

    /// What type of connection the user-agent would prefer
    [<Extension>]
    static member Connection(context: HeaderContext, connection) =
        Header.connection connection context

    /// An HTTP cookie previously sent by the server with 'Set-Cookie'.
    [<Extension>]
    static member SetCookie(context: HeaderContext, name, value) =
        Header.cookie name value context

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies.
    [<Extension>]
    static member SetCookieForPath(context: HeaderContext, name, value, path) =
        Header.cookieForPath name value path context

    /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
    /// the subset of URIs on the origin server to which this Cookie applies
    /// and the internet domain for which this Cookie is valid.
    [<Extension>]
    static member SetCookieForDomain(context: HeaderContext, name, value, path, domain) =
        Header.cookieForDomain name value path domain context

    /// The date and time that the message was sent
    [<Extension>]
    static member Date(context: HeaderContext, date) =
        Header.date date context

    /// Indicates that particular server behaviors are required by the client
    [<Extension>]
    static member Expect(context: HeaderContext, behaviors) =
        Header.expect behaviors context

    /// Gives the date/time after which the response is considered stale
    [<Extension>]
    static member Expires(context: HeaderContext, dateTime) =
        Header.expires dateTime context

    /// The email address of the user making the request
    [<Extension>]
    static member From(context: HeaderContext, email) =
        Header.from email context

    /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
    /// The port number may be omitted if the port is the standard port for the service requested.
    [<Extension>]
    static member Host(context: HeaderContext, host) =
        Header.host host context

    /// Only perform the action if the client supplied entity matches the same entity on the server.
    /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
    [<Extension>]
    static member IfMatch(context: HeaderContext, entity) =
        Header.ifMatch entity context

    /// Allows a 304 Not Modified to be returned if content is unchanged
    [<Extension>]
    static member IfModifiedSince(context: HeaderContext, dateTime) =
        Header.ifModifiedSince dateTime context

    /// Allows a 304 Not Modified to be returned if content is unchanged
    [<Extension>]
    static member IfNoneMatch(context: HeaderContext, etag) =
        Header.ifNoneMatch etag context

    /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
    [<Extension>]
    static member IfRange(context: HeaderContext, range) =
        Header.ifRange range context

    /// Only send the response if the entity has not been modified since a specific time
    [<Extension>]
    static member IfUnmodifiedSince(context: HeaderContext, dateTime) =
        Header.ifUnmodifiedSince dateTime context

    /// Specifies a parameter used into order to maintain a persistent connection
    [<Extension>]
    static member KeepAlive(context: HeaderContext, keepAlive) =
        Header.keepAlive keepAlive context

    /// Specifies the date and time at which the accompanying body data was last modified
    [<Extension>]
    static member LastModified(context: HeaderContext, dateTime) =
        Header.lastModified dateTime context

    /// Limit the number of times the message can be forwarded through proxies or gateways
    [<Extension>]
    static member MaxForwards(context: HeaderContext, count) =
        Header.maxForwards count context

    /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
    [<Extension>]
    static member Origin(context: HeaderContext, origin) =
        Header.origin origin context

    /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
    [<Extension>]
    static member Pragma(context: HeaderContext, pragma) =
        Header.pragma pragma context

    /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
    [<Extension>]
    static member Prefer(context: HeaderContext, prefer) =
        Header.prefer prefer context

    /// Authorization credentials for connecting to a proxy.
    [<Extension>]
    static member ProxyAuthorization(context: HeaderContext, credentials) =
        Header.proxyAuthorization credentials context

    /// Request only part of an entity. Bytes are numbered from 0
    [<Extension>]
    static member Range(context: HeaderContext, start, finish) =
        Header.range start finish context

    /// This is the address of the previous web page from which a link to the currently requested page was followed.
    /// (The word "referrer" is misspelled in the RFC as well as in most implementations.)
    [<Extension>]
    static member Referer(context: HeaderContext, referer) =
        Header.referer referer context

    /// The transfer encodings the user agent is willing to accept: the same values as for the response header
    /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
    /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
    [<Extension>]
    static member TE(context: HeaderContext, te) =
        Header.te te context

    /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
    [<Extension>]
    static member Trailer(context: HeaderContext, trailer) =
        Header.trailer trailer context

    /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.
    /// The valid directives are one of: chunked, compress, deflate, gzip, orentity.
    [<Extension>]
    static member TransferEncoding(context: HeaderContext, directive) =
        Header.transferEncoding directive context

    /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
    [<Extension>]
    static member Translate(context: HeaderContext, translate) =
        Header.translate translate context

    /// Specifies additional communications protocols that the client supports.
    [<Extension>]
    static member Upgrade(context: HeaderContext, upgrade) =
        Header.upgrade upgrade context

    /// The user agent string of the user agent
    [<Extension>]
    static member UserAgent(context: HeaderContext, userAgent) =
        Header.userAgent userAgent context

    /// Informs the server of proxies through which the request was sent
    [<Extension>]
    static member Via(context: HeaderContext, server) =
        Header.via server context

    /// A general warning about possible problems with the entity body
    [<Extension>]
    static member Warning(context: HeaderContext, message) =
        Header.warning message context

    /// Override HTTP method.
    [<Extension>]
    static member XHTTPMethodOverride(context: HeaderContext, httpMethod) =
        Header.xhttpMethodOverride httpMethod context


// ---------
// Body
// ---------

[<Extension>]
type BodyExtensions =

    /// An explicit transformation from a previous context to allow for describing the request body.
    [<Extension>]
    static member Body(context: #IToBodyContext) =
        context.ToBodyContext()

    [<Extension>]
    static member Content(context: BodyContext, contentType, data) =
        Body.content contentType data context

    [<Extension>]
    static member Binary(context: BodyContext, data) =
        Body.binary data context

    [<Extension>]
    static member Stream(context: BodyContext, stream) =
        Body.stream stream context

    [<Extension>]
    static member Text(context: BodyContext, text) =
        Body.text text context

    [<Extension>]
    static member Json(context: BodyContext, json) =
        Body.json json context

    [<Extension>]
    static member JsonSerialize(context: BodyContext, options, json) =
        Body.jsonSerializeWith options json context

    [<Extension>]
    static member JsonSerialize(context: BodyContext, json) =
        Body.jsonSerialize json context

    [<Extension>]
    static member FormUrlEncoded(context: BodyContext, data) =
        Body.formUrlEncoded data context

    [<Extension>]
    static member File(context: BodyContext, path) =
        Body.file path context

    /// The type of encoding used on the data
    [<Extension>]
    static member ContentEncoding(context: BodyContext, encoding) =
        Body.contentEncoding encoding context

    /// The MIME type of the body of the request (used with POST and PUT requests)
    [<Extension>]
    static member ContentType(context: BodyContext, contentType, ?charset) =
        Body.contentType contentType charset context


// -----------------
// Multipart Element
// -----------------

[<Extension>]
type MultipartElementExtensions =

    /// The MIME type of the body of the request (used with POST and PUT requests)
    [<Extension>]
    static member ContentType(context: MultipartElementContext, contentType, ?charset) =
        MultipartElement.contentType contentType charset context


// ---------
// Multipart
// ---------

[<Extension>]
type MultipartExtensions =

    /// An explicit transformation from a previous context to allow for describing the request multiparts.
    [<Extension>]
    static member Multipart(context: #IToMultipartContext) =
        context.ToMultipartContext()

    [<Extension>]
    static member TextPart(context: MultipartContext, value, name, ?fileName) =
        Multipart.textPart value name fileName context

    [<Extension>]
    static member FilePart(context: MultipartContext, path, ?name, ?fileName) =
        Multipart.filePart path name fileName context

    [<Extension>]
    static member BinaryPart(context: MultipartContext, value, name, ?fileName) =
        Multipart.binaryPart value name fileName context

    [<Extension>]
    static member StreamPart(context: MultipartContext, value, name, ?fileName) =
        Multipart.streamPart value name fileName context


// ---------
// Config
// ---------

type ConfigTransformerFunc = Func<Domain.Config, Domain.Config>

type FluentConfig<'self>(context: IUpdateConfig<'self>) =
    member _.Context = context

[<Extension>]
type ConfigExtensions =

    [<Extension>]
    static member Config(context: HeaderContext) = FluentConfig(context)

    [<Extension>]
    static member Config(context: BodyContext) = FluentConfig(context)

    [<Extension>]
    static member Config(context: MultipartContext) = FluentConfig(context)

    [<Extension>]
    static member Config(context: MultipartElementContext) = FluentConfig(context)

    // ----------------
    
    [<Extension>]
    static member Update(fluent: FluentConfig<_>, transformer) =
        Config.update transformer fluent.Context

    [<Extension>]
    static member Set(fluent: FluentConfig<_>, config) =
        Config.set config fluent.Context

    // ----------------

    [<Extension>]
    static member IgnoreCertIssues(fluent: FluentConfig<_>) =
        Config.ignoreCertIssues fluent.Context

    [<Extension>]
    static member Timeout(fluent: FluentConfig<_>, value) =
        Config.timeout value fluent.Context

    [<Extension>]
    static member NoTimeout(fluent: FluentConfig<_>) =
        Config.noTimeout fluent.Context

    [<Extension>]
    static member TimeoutInSeconds(fluent: FluentConfig<_>, value) =
        Config.timeoutInSeconds value fluent.Context

    [<Extension>]
    static member TransformHeader(fluent: FluentConfig<_>, transformer) =
        Config.transformHeader transformer fluent.Context

    [<Extension>]
    static member TransformUrl(fluent: FluentConfig<_>, transformer) =
        Config.transformUrl transformer fluent.Context

    [<Extension>]
    static member UseBaseUrl(fluent: FluentConfig<_>, baseUrl) =
        Config.useBaseUrl baseUrl fluent.Context

    [<Extension>]
    static member SetHttpClientFactory(fluent: FluentConfig<_>, httpClientFactory) =
        Config.setHttpClientFactory httpClientFactory fluent.Context

    [<Extension>]
    static member TransformHttpClient(fluent: FluentConfig<_>, transformer) =
        Config.transformHttpClient transformer fluent.Context

    [<Extension>]
    static member TransformHttpRequestMessage(fluent: FluentConfig<_>, transformer) =
        Config.transformHttpRequestMessage transformer fluent.Context

    [<Extension>]
    static member TransformHttpClientHandler(fluent: FluentConfig<_>, transformer) =
        Config.transformHttpClientHandler transformer fluent.Context

    [<Extension>]
    static member Proxy(fluent: FluentConfig<_>, url) =
        Config.proxy url fluent.Context

    [<Extension>]
    static member Proxy(fluent: FluentConfig<_>, url, credentials) =
        Config.proxyWithCredentials url credentials fluent.Context

    [<Extension>]
    static member DecompressionMethods(fluent: FluentConfig<_>, decompressionMethods) =
        Config.decompressionMethods decompressionMethods fluent.Context

    [<Extension>]
    static member NoDecompression(fluent: FluentConfig<_>) =
        Config.noDecompression fluent.Context

    [<Extension>]
    static member CancellationToken(fluent: FluentConfig<_>, cancellationToken) =
        Config.cancellationToken cancellationToken fluent.Context


// ---------
// PrintHint
// ---------

type PrintHintTransformerFunc = Func<Domain.PrintHint, Domain.PrintHint>

type FluentPrintHint<'self>(context: IUpdatePrintHint<'self>) =
    member _.Context = context

[<Extension>]
type PrintExtensions =

    [<Extension>]
    static member Print(context: HeaderContext) = FluentPrintHint(context)

    [<Extension>]
    static member Print(context: BodyContext) = FluentPrintHint(context)

    [<Extension>]
    static member Print(context: MultipartContext) = FluentPrintHint(context)

    [<Extension>]
    static member Print(context: MultipartElementContext) = FluentPrintHint(context)

    // ----------------
    
    [<Extension>]
    static member Update(fluent: FluentPrintHint<_>, transformer) =
        Print.update transformer fluent.Context

    [<Extension>]
    static member Set(fluent: FluentPrintHint<_>, printHint) =
        Print.set printHint fluent.Context

    // ----------------

    [<Extension>]
    static member WithRequestPrintMode(fluent: FluentPrintHint<_>, updatePrintMode) =
        Print.withRequestPrintMode updatePrintMode fluent.Context

    [<Extension>]
    static member WithResponsePrintMode(fluent: FluentPrintHint<_>, updatePrintMode) =
        Print.withResponsePrintMode updatePrintMode fluent.Context

    [<Extension>]
    static member WithResponseBody(fluent: FluentPrintHint<_>, updateBodyPrintMode) =
        Print.withResponseBody updateBodyPrintMode fluent.Context

    [<Extension>]
    static member UseObjectFormatting(fluent: FluentPrintHint<_>) =
        Print.useObjectFormatting fluent.Context

    [<Extension>]
    static member HeaderOnly(fluent: FluentPrintHint<_>) =
        Print.headerOnly fluent.Context

    [<Extension>]
    static member WithResponseBodyLength(fluent: FluentPrintHint<_>, maxLength) =
        Print.withResponseBodyLength maxLength fluent.Context

    [<Extension>]
    static member WithResponseBodyFormat(fluent: FluentPrintHint<_>, format) =
        Print.withResponseBodyFormat format fluent.Context

    [<Extension>]
    static member WithResponseBodyExpanded(fluent: FluentPrintHint<_>) =
        Print.withResponseBodyExpanded fluent.Context


// ---------
// Request
// ---------

[<Extension>]
type RequestExtensions =

    [<Extension>]
    static member ToHttpRequestMessage(request: IToRequest) =
        Request.toHttpRequestMessage request

    [<Extension>]
    static member Send(request: IToRequest) =
        request |> Request.send

    [<Extension>]
    static member SendAsync(request: IToRequest) =
        request |> Request.sendTAsync

    [<Extension>]
    static member SendAsync(request: IToRequest, cancellationToken: CancellationToken) =
        request |> Request.toAsync (Some cancellationToken) |> Async.StartAsTask


// ---------
// Request
// ---------

[<Extension>]
type ResponseExtensions =

    // ----------------
    // ToStream
    // ----------------
    
    [<Extension>]
    static member ToStream(response: Domain.Response) =
        response |> Response.toStream

    [<Extension>]
    static member ToStreamAsync(response: Domain.Response) =
        response |> Response.toStreamTAsync CancellationToken.None

    [<Extension>]
    static member ToStreamAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.toStreamTAsync cancellationToken

    // ----------------
    // ToBytes
    // ----------------

    [<Extension>]
    static member ToBytes(response: Domain.Response) =
        response |> Response.toBytes

    [<Extension>]
    static member ToBytesAsync(response: Domain.Response) =
        response |> Response.toBytesTAsync CancellationToken.None

    [<Extension>]
    static member ToBytesAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.toBytesTAsync cancellationToken

    // ----------------
    // ToString
    // ----------------

    [<Extension>]
    static member ToString(response: Domain.Response, maxLength) =
        response |> Response.toString maxLength

    [<Extension>]
    static member ToStringAsync(response: Domain.Response, maxLength) =
        response |> Response.toStringTAsync maxLength CancellationToken.None

    [<Extension>]
    static member ToStringAsync(response: Domain.Response, maxLength, cancellationToken: CancellationToken) =
        response |> Response.toStringTAsync maxLength cancellationToken

    // ----------------
    // ToText
    // ----------------

    [<Extension>]
    static member ToText(response: Domain.Response) =
        response |> Response.toText

    [<Extension>]
    static member ToTextAsync(response: Domain.Response) =
        response |> Response.toTextTAsync CancellationToken.None

    [<Extension>]
    static member ToTextAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.toTextTAsync cancellationToken

    // ----------------
    // ToXml
    // ----------------

    [<Extension>]
    static member ToXml(response: Domain.Response) =
        response |> Response.toXml

    [<Extension>]
    static member ToXmlAsync(response: Domain.Response) =
        response |> Response.toXmlTAsync CancellationToken.None

    [<Extension>]
    static member ToXmlAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.toXmlTAsync cancellationToken

    // ----------------
    // ToJsonDocument
    // ----------------

    [<Extension>]
    static member ToJsonDocument(response: Domain.Response, options) =
        response |> Response.toJsonDocumentWith options

    [<Extension>]
    static member ToJsonDocumentAsync(response: Domain.Response, options) =
        response |> Response.toJsonDocumentWithTAsync options CancellationToken.None

    [<Extension>]
    static member ToJsonDocumentAsync(response: Domain.Response, options, cancellationToken: CancellationToken) =
        response |> Response.toJsonDocumentWithTAsync options cancellationToken

    [<Extension>]
    static member ToJsonDocumen(response: Domain.Response) =
        response |> Response.toJsonDocument

    [<Extension>]
    static member ToJsonDocumenAsync(response: Domain.Response) =
        response |> Response.toJsonDocumentTAsync CancellationToken.None

    [<Extension>]
    static member ToJsonDocumenAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.toJsonDocumentTAsync cancellationToken

    // ----------------
    // ToJson
    // ----------------

    [<Extension>]
    static member ToJson(response: Domain.Response, options) =
        response |> Response.toJsonWith options

    [<Extension>]
    static member ToJsonAsync(response: Domain.Response, options) =
        response |> Response.toJsonWithTAsync options CancellationToken.None

    [<Extension>]
    static member ToJsonAsync(response: Domain.Response, options, cancellationToken: CancellationToken) =
        response |> Response.toJsonWithTAsync options cancellationToken

    [<Extension>]
    static member ToJson(response: Domain.Response) =
        response |> Response.toJson

    [<Extension>]
    static member ToJsonAsync(response: Domain.Response) =
        response |> Response.toJsonTAsync CancellationToken.None

    [<Extension>]
    static member ToJsonAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.toJsonTAsync cancellationToken

    // ----------------
    // ToJsonSeq
    // ----------------

    [<Extension>]
    static member ToJsonSeq(response: Domain.Response, options) =
        response |> Response.toJsonSeqWith options

    [<Extension>]
    static member ToJsonSeqAsync(response: Domain.Response, options) =
        response |> Response.toJsonSeqWithTAsync options CancellationToken.None

    [<Extension>]
    static member ToJsonSeqAsync(response: Domain.Response, options, cancellationToken: CancellationToken) =
        response |> Response.toJsonSeqWithTAsync options cancellationToken

    [<Extension>]
    static member ToJsonSeq(response: Domain.Response) =
        response |> Response.toJsonSeq

    [<Extension>]
    static member ToJsonSeqAsync(response: Domain.Response) =
        response |> Response.toJsonSeqTAsync CancellationToken.None

    [<Extension>]
    static member ToJsonSeqAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.toJsonSeqTAsync cancellationToken

    // ----------------
    // ToJsonArray
    // ----------------

    [<Extension>]
    static member ToJsonArray(response: Domain.Response, options) =
        response |> Response.toJsonArrayWith options

    [<Extension>]
    static member ToJsonArrayAsync(response: Domain.Response, options) =
        response |> Response.toJsonArrayWithTAsync options CancellationToken.None

    [<Extension>]
    static member ToJsonArrayAsync(response: Domain.Response, options, cancellationToken: CancellationToken) =
        response |> Response.toJsonArrayWithTAsync options cancellationToken

    [<Extension>]
    static member ToJsonArray(response: Domain.Response) =
        response |> Response.toJsonArray

    [<Extension>]
    static member ToJsonArrayAsync(response: Domain.Response) =
        response |> Response.toJsonArrayTAsync CancellationToken.None

    [<Extension>]
    static member ToJsonArrayAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.toJsonArrayTAsync cancellationToken

    // ----------------
    // ToJsonList
    // ----------------

    [<Extension>]
    static member ToJsonList(response: Domain.Response, options) =
        response |> Response.toJsonListWith options

    [<Extension>]
    static member ToJsonListAsync(response: Domain.Response, options) =
        response |> Response.toJsonListWithTAsync options CancellationToken.None

    [<Extension>]
    static member ToJsonListAsync(response: Domain.Response, options, cancellationToken: CancellationToken) =
        response |> Response.toJsonListWithTAsync options cancellationToken

    [<Extension>]
    static member ToJsonList(response: Domain.Response) =
        response |> Response.toJsonList

    [<Extension>]
    static member ToJsonListAsync(response: Domain.Response) =
        response |> Response.toJsonListTAsync CancellationToken.None

    [<Extension>]
    static member ToJsonListAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.toJsonListTAsync cancellationToken

    // ----------------
    // DeserializeJson
    // ----------------

    [<Extension>]
    static member DeserializeJson<'T>(response: Domain.Response, options) =
        response |> Response.deserializeJsonWith options

    [<Extension>]
    static member DeserializeJsonAsync<'T>(response: Domain.Response, options) =
        response |> Response.deserializeJsonWithTAsync options CancellationToken.None

    [<Extension>]
    static member DeserializeJsonAsync<'T>(response: Domain.Response, options, cancellationToken: CancellationToken) =
        response |> Response.deserializeJsonWithTAsync options cancellationToken

    [<Extension>]
    static member DeserializeJson(response: Domain.Response) =
        response |> Response.deserializeJson

    [<Extension>]
    static member DeserializeJsonAsync(response: Domain.Response) =
        response |> Response.deserializeJsonTAsync CancellationToken.None

    [<Extension>]
    static member DeserializeJsonAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.deserializeJsonTAsync cancellationToken

    // ----------------
    // ToFormattedText
    // ----------------

    [<Extension>]
    static member ToFormattedTex(response: Domain.Response) =
        response |> Response.toFormattedText

    [<Extension>]
    static member ToFormattedTexAsync(response: Domain.Response) =
        response |> Response.toFormattedTextTAsync CancellationToken.None

    [<Extension>]
    static member ToFormattedTexAsync(response: Domain.Response, cancellationToken: CancellationToken) =
        response |> Response.toFormattedTextTAsync cancellationToken

    // ----------------
    // SaveFile
    // ----------------

    [<Extension>]
    static member SaveFile(response: Domain.Response, fileName) =
        response |> Response.saveFile fileName

    [<Extension>]
    static member SaveFileAsync(response: Domain.Response, fileName) =
        response |> Response.saveFileTAsync fileName CancellationToken.None

    [<Extension>]
    static member SaveFileAsync(response: Domain.Response, fileName, cancellationToken: CancellationToken) =
        response |> Response.saveFileTAsync fileName cancellationToken

    // ----------------
    // Assert
    // ----------------

    [<Extension>]
    static member AssertStatusCodes(response: Domain.Response, statusCodes) =
        Response.assertStatusCodes (statusCodes |> Seq.toList) response

    [<Extension>]
    static member AssertStatusCode(response: Domain.Response, statusCode) =
        Response.assertStatusCode statusCode response

    [<Extension>]
    static member AssertOk(response: Domain.Response) =
        Response.assertOk response

module FsHttp.DslCE

open FsHttp.Domain

type LazyHttpBuilder<'context when 'context :> IContext>(context: 'context) =
        
    // need to implement this so that Request.send (etc.) are working.
    interface IContext with
        member this.ToRequest() = context.ToRequest()
        
    member this.Context = context

    member this.Bind(m, f) = f m
    member this.Return(x) = x
    member this.Yield(_) = LazyHttpBuilder context
    member this.For(m, f) = this.Bind m f
let httpLazy = LazyHttpBuilder(StartingContext)

/// Provides base support for ```http { METHOD ... }``` syntax
type EagerHttpBuilder() =
    inherit LazyHttpBuilder<StartingContext>(StartingContext)

/// Builder that executes blocking (synchronous) immediately.
type EagerSyncHttpBuilder() =
    inherit EagerHttpBuilder()
    member inline this.Delay(f: unit -> 'a) = f
    member inline this.Run(f: unit -> LazyHttpBuilder<#IContext>) =
        f() |> Request.send
let http = EagerSyncHttpBuilder()

/// Builder that executes non-blocking (asynchronous) and immediately.
type EagerAsyncHttpBuilder() =
    inherit EagerHttpBuilder()
    member inline this.Delay(f: unit -> 'a) = f
    member inline this.Run(f: unit -> LazyHttpBuilder<#IContext>) =
        f() |> Request.sendAsync
let httpAsync = EagerAsyncHttpBuilder()

/// Builder that executes non-blocking (asynchronous) and lazy.
type LazyAsyncHttpBuilder() =
    inherit EagerHttpBuilder()
    member inline this.Delay(f: unit -> 'a) = f
    member inline this.Run(f: unit -> LazyHttpBuilder<#IContext>) =
        f() |> Request.buildAsync
let httpLazyAsync = LazyAsyncHttpBuilder()

/// Builder that creates a System.Net.Http.HttpRequestMessage object.
type HttpMessageBuilder() =
    inherit LazyHttpBuilder<StartingContext>(StartingContext)
    member inline this.Delay(f: unit -> 'a) = f
    member inline this.Run(f: unit -> LazyHttpBuilder<#IContext>) =
        f()
        |> fun builder -> builder.Context.ToRequest()
        |> Request.toMessage
let httpMsg = HttpMessageBuilder()


[<AutoOpen>]
module Method =

    // RFC 2626 specifies 8 methods + PATCH
    let request (method: string) (url: string) = Dsl.Method.request method url|> LazyHttpBuilder
    let get (url: string) = Dsl.Method.get url|> LazyHttpBuilder
    let put (url: string) = Dsl.Method.put url|> LazyHttpBuilder
    let post (url: string) = Dsl.Method.post url|> LazyHttpBuilder
    let delete (url: string) = Dsl.Method.delete url|> LazyHttpBuilder
    let options (url: string) = Dsl.Method.options url|> LazyHttpBuilder
    let head (url: string) = Dsl.Method.head url|> LazyHttpBuilder
    let trace (url: string) = Dsl.Method.trace url|> LazyHttpBuilder
    let connect (url: string) = Dsl.Method.connect url|> LazyHttpBuilder
    let patch (url: string) = Dsl.Method.patch url|> LazyHttpBuilder

    // TODO: RFC 4918 (WebDAV) adds 7 methods

    type LazyHttpBuilder<'context when 'context :> IContext> with

        [<CustomOperation("Request")>]
        member this.Request(_: LazyHttpBuilder<StartingContext>, method, url) = request method url

        // RFC 2626 specifies 8 methods
        [<CustomOperation("GET")>]
        member this.Get(_: LazyHttpBuilder<StartingContext>, url) = get url

        [<CustomOperation("PUT")>]
        member this.Put(_: LazyHttpBuilder<StartingContext>, url) = put url

        [<CustomOperation("POST")>]
        member this.Post(_: LazyHttpBuilder<StartingContext>, url) = post url

        [<CustomOperation("DELETE")>]
        member this.Delete(_: LazyHttpBuilder<StartingContext>, url) = delete url

        [<CustomOperation("OPTIONS")>]
        member this.Options(_: LazyHttpBuilder<StartingContext>, url) = options url

        [<CustomOperation("HEAD")>]
        member this.Head(_: LazyHttpBuilder<StartingContext>, url) = head url

        [<CustomOperation("TRACE")>]
        member this.Trace(_: LazyHttpBuilder<StartingContext>, url) = trace url

        [<CustomOperation("CONNECT")>]
        member this.Connect(_: LazyHttpBuilder<StartingContext>, url) = connect url

        [<CustomOperation("PATCH")>]
        member this.Patch(_: LazyHttpBuilder<StartingContext>, url) = patch url

        /// Append query params
        [<CustomOperation("query")>]
        member this.Query(builder: LazyHttpBuilder<_>, queryParams) =
            Dsl.Header.query queryParams builder.Context|> LazyHttpBuilder


[<AutoOpen>]
module Header =
    type LazyHttpBuilder<'context when 'context :> IContext> with

        /// Content-Types that are acceptable for the response
        [<CustomOperation("Accept")>]
        member this.Accept(builder: LazyHttpBuilder<_>, contentType) =
            Dsl.Header.accept contentType builder.Context|> LazyHttpBuilder

        /// Character sets that are acceptable
        [<CustomOperation("AcceptCharset")>]
        member this.AcceptCharset(builder: LazyHttpBuilder<_>, characterSets) =
            Dsl.Header.acceptCharset characterSets builder.Context|> LazyHttpBuilder

        /// Acceptable version in time
        [<CustomOperation("AcceptDatetime")>]
        member this.AcceptDatetime(builder: LazyHttpBuilder<_>, dateTime) =
            Dsl.Header.acceptDatetime dateTime builder.Context|> LazyHttpBuilder

        /// List of acceptable encodings. See HTTP compression.
        [<CustomOperation("AcceptEncoding")>]
        member this.AcceptEncoding(builder: LazyHttpBuilder<_>, encoding) =
            Dsl.Header.acceptEncoding encoding builder.Context|> LazyHttpBuilder

        /// List of acceptable human languages for response
        [<CustomOperation("AcceptLanguage")>]
        member this.AcceptLanguage(builder: LazyHttpBuilder<_>, language) =
            Dsl.Header.acceptLanguage language builder.Context|> LazyHttpBuilder
        
        /// Authorization credentials for HTTP authorization
        [<CustomOperation("Authorization")>]
        member this.Authorization(builder: LazyHttpBuilder<_>, credentials) =
            Dsl.Header.authorization credentials builder.Context|> LazyHttpBuilder

        /// Authorization header using Bearer authorization token
        [<CustomOperation("AuthorizationBearer")>]
        member this.AuthorizationBearer(builder: LazyHttpBuilder<_>, token) =
            Dsl.Header.authorizationBearer token builder.Context|> LazyHttpBuilder

        /// Authorization header using Basic (User/Password) authorization
        [<CustomOperation("AuthorizationUserPw")>]
        member this.AuthorizationUserPw(builder: LazyHttpBuilder<_>, username, password) =
            Dsl.Header.authorizationUserPw username password builder.Context|> LazyHttpBuilder

        /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
        [<CustomOperation("CacheControl")>]
        member this.CacheControl(builder: LazyHttpBuilder<_>, control) =
            Dsl.Header.cacheControl control builder.Context|> LazyHttpBuilder

        /// What type of connection the user-agent would prefer
        [<CustomOperation("Connection")>]
        member this.Connection(builder: LazyHttpBuilder<_>, connection) =
            Dsl.Header.connection connection builder.Context|> LazyHttpBuilder

        /// An HTTP cookie previously sent by the server with 'Set-Cookie'.
        [<CustomOperation("Cookie")>]
        member this.SetCookie(builder: LazyHttpBuilder<_>, name, value) =
            Dsl.Header.cookie name value builder.Context|> LazyHttpBuilder

        /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
        /// the subset of URIs on the origin server to which this Cookie applies.
        [<CustomOperation("CookieForPath")>]
        member this.SetCookieForPath(builder: LazyHttpBuilder<_>, name, value, path) =
            Dsl.Header.cookieForPath name value path builder.Context|> LazyHttpBuilder

        /// An HTTP cookie previously sent by the server with 'Set-Cookie' with
        /// the subset of URIs on the origin server to which this Cookie applies
        /// and the internet domain for which this Cookie is valid.
        [<CustomOperation("CookieForDomain")>]
        member this.SetCookieForDomain(builder: LazyHttpBuilder<_>, name, value, path, domain) =
            Dsl.Header.cookieForDomain name value path domain builder.Context|> LazyHttpBuilder

        /// The date and time that the message was sent
        [<CustomOperation("Date")>]
        member this.Date(builder: LazyHttpBuilder<_>, date) =
            Dsl.Header.date date builder.Context|> LazyHttpBuilder

        /// Indicates that particular server behaviors are required by the client
        [<CustomOperation("Expect")>]
        member this.Expect(builder: LazyHttpBuilder<_>, behaviors) =
            Dsl.Header.expect behaviors builder.Context|> LazyHttpBuilder

        /// Gives the date/time after which the response is considered stale
        [<CustomOperation("Expires")>]
        member this.Expires(builder: LazyHttpBuilder<_>, dateTime) =
            Dsl.Header.expires dateTime builder.Context|> LazyHttpBuilder

        /// The email address of the user making the request
        [<CustomOperation("From")>]
        member this.From(builder: LazyHttpBuilder<_>, email) =
            Dsl.Header.from email builder.Context|> LazyHttpBuilder

        /// Custom header
        [<CustomOperation("Header")>]
        member this.Header(builder: LazyHttpBuilder<_>, key, value) =
            Dsl.Header.header key value builder.Context|> LazyHttpBuilder

        /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
        /// The port number may be omitted if the port is the standard port for the service requested.
        [<CustomOperation("Host")>]
        member this.Host(builder: LazyHttpBuilder<_>, host) =
            Dsl.Header.host host builder.Context|> LazyHttpBuilder

        /// Only perform the action if the client supplied entity matches the same entity on the server.
        /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
        [<CustomOperation("IfMatch")>]
        member this.IfMatch(builder: LazyHttpBuilder<_>, entity) =
            Dsl.Header.ifMatch entity builder.Context|> LazyHttpBuilder

        /// Allows a 304 Not Modified to be returned if content is unchanged
        [<CustomOperation("IfModifiedSince")>]
        member this.IfModifiedSince(builder: LazyHttpBuilder<_>, dateTime) =
            Dsl.Header.ifModifiedSince dateTime builder.Context|> LazyHttpBuilder

        /// Allows a 304 Not Modified to be returned if content is unchanged
        [<CustomOperation("IfNoneMatch")>]
        member this.IfNoneMatch(builder: LazyHttpBuilder<_>, etag) =
            Dsl.Header.ifNoneMatch etag builder.Context|> LazyHttpBuilder

        /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
        [<CustomOperation("IfRange")>]
        member this.IfRange(builder: LazyHttpBuilder<_>, range) =
            Dsl.Header.ifRange range builder.Context|> LazyHttpBuilder

        /// Only send the response if the entity has not been modified since a specific time
        [<CustomOperation("IfUnmodifiedSince")>]
        member this.IfUnmodifiedSince(builder: LazyHttpBuilder<_>, dateTime) =
            Dsl.Header.ifUnmodifiedSince dateTime builder.Context|> LazyHttpBuilder

        /// Specifies a parameter used into order to maintain a persistent connection
        [<CustomOperation("KeepAlive")>]
        member this.KeepAlive(builder: LazyHttpBuilder<_>, keepAlive) =
            Dsl.Header.keepAlive keepAlive builder.Context|> LazyHttpBuilder

        /// Specifies the date and time at which the accompanying body data was last modified
        [<CustomOperation("LastModified")>]
        member this.LastModified(builder: LazyHttpBuilder<_>, dateTime) =
            Dsl.Header.lastModified dateTime builder.Context|> LazyHttpBuilder

        /// Limit the number of times the message can be forwarded through proxies or gateways
        [<CustomOperation("MaxForwards")>]
        member this.MaxForwards(builder: LazyHttpBuilder<_>, count) =
            Dsl.Header.maxForwards count builder.Context|> LazyHttpBuilder

        /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
        [<CustomOperation("Origin")>]
        member this.Origin(builder: LazyHttpBuilder<_>, origin) =
            Dsl.Header.origin origin builder.Context|> LazyHttpBuilder

        /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
        [<CustomOperation("Pragma")>]
        member this.Pragma(builder: LazyHttpBuilder<_>, pragma) =
            Dsl.Header.pragma pragma builder.Context|> LazyHttpBuilder

        /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
        [<CustomOperation("Prefer")>]
        member this.Prefer(builder: LazyHttpBuilder<_>, prefer) =
            Dsl.Header.prefer prefer builder.Context|> LazyHttpBuilder

        /// Authorization credentials for connecting to a proxy.
        [<CustomOperation("ProxyAuthorization")>]
        member this.ProxyAuthorization(builder: LazyHttpBuilder<_>, credentials) =
            Dsl.Header.proxyAuthorization credentials builder.Context|> LazyHttpBuilder

        /// Request only part of an entity. Bytes are numbered from 0
        [<CustomOperation("Range")>]
        member this.Range(builder: LazyHttpBuilder<_>, start, finish) =
            Dsl.Header.range start finish builder.Context|> LazyHttpBuilder

        /// This is the address of the previous web page from which a link to the currently requested page was followed.
        /// (The word "referrer" is misspelled in the RFC as well as in most implementations.)
        [<CustomOperation("Referer")>]
        member this.Referer(builder: LazyHttpBuilder<_>, referer) =
            Dsl.Header.referer referer builder.Context|> LazyHttpBuilder

        /// The transfer encodings the user agent is willing to accept: the same values as for the response header
        /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
        /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
        [<CustomOperation("TE")>]
        member this.TE(builder: LazyHttpBuilder<_>, te) =
            Dsl.Header.te te builder.Context|> LazyHttpBuilder

        /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
        [<CustomOperation("Trailer")>]
        member this.Trailer(builder: LazyHttpBuilder<_>, trailer) =
            Dsl.Header.trailer trailer builder.Context|> LazyHttpBuilder

        /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.
        /// The valid directives are one of: chunked, compress, deflate, gzip, orentity.
        [<CustomOperation("TransferEncoding")>]
        member this.TransferEncoding(builder: LazyHttpBuilder<_>, directive) =
            Dsl.Header.transferEncoding directive builder.Context|> LazyHttpBuilder

        /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
        [<CustomOperation("Translate")>]
        member this.Translate(builder: LazyHttpBuilder<_>, translate) =
            Dsl.Header.translate translate builder.Context|> LazyHttpBuilder

        /// Specifies additional communications protocols that the client supports.
        [<CustomOperation("Upgrade")>]
        member this.Upgrade(builder: LazyHttpBuilder<_>, upgrade) =
            Dsl.Header.upgrade upgrade builder.Context|> LazyHttpBuilder

        /// The user agent string of the user agent
        [<CustomOperation("UserAgent")>]
        member this.UserAgent(builder: LazyHttpBuilder<_>, userAgent) =
            Dsl.Header.userAgent userAgent builder.Context|> LazyHttpBuilder

        /// Informs the server of proxies through which the request was sent
        [<CustomOperation("Via")>]
        member this.Via(builder: LazyHttpBuilder<_>, server) =
            Dsl.Header.via server builder.Context|> LazyHttpBuilder

        /// A general warning about possible problems with the entity body
        [<CustomOperation("Warning")>]
        member this.Warning(builder: LazyHttpBuilder<_>, message) =
            Dsl.Header.warning message builder.Context|> LazyHttpBuilder

        /// Override HTTP method.
        [<CustomOperation("XHTTPMethodOverride")>]
        member this.XHTTPMethodOverride(builder: LazyHttpBuilder<_>, httpMethod) =
            Dsl.Header.xhttpMethodOverride httpMethod builder.Context|> LazyHttpBuilder


[<AutoOpen>]
module Body =
    type LazyHttpBuilder<'context when 'context :> IContext> with

        [<CustomOperation("body")>]
        member this.Body(builder: LazyHttpBuilder<_>) =
            Dsl.Body.body builder.Context|> LazyHttpBuilder

        [<CustomOperation("binary")>]
        member this.Binary(builder: LazyHttpBuilder<_>, data) =
            Dsl.Body.binary data builder.Context|> LazyHttpBuilder

        [<CustomOperation("stream")>]
        member this.Stream(builder: LazyHttpBuilder<_>, stream) =
            Dsl.Body.stream stream builder.Context|> LazyHttpBuilder

        [<CustomOperation("text")>]
        member this.Text(builder: LazyHttpBuilder<_>, text) =
            Dsl.Body.text text builder.Context|> LazyHttpBuilder

        [<CustomOperation("json")>]
        member this.Json(builder: LazyHttpBuilder<_>, json) =
            Dsl.Body.json json builder.Context|> LazyHttpBuilder

        [<CustomOperation("formUrlEncoded")>]
        member this.FormUrlEncoded(builder: LazyHttpBuilder<_>, data) =
            Dsl.Body.formUrlEncoded data builder.Context|> LazyHttpBuilder

        [<CustomOperation("file")>]
        member this.File(builder: LazyHttpBuilder<_>, path) =
            Dsl.Body.file path builder.Context|> LazyHttpBuilder

        /// The type of encoding used on the data
        [<CustomOperation("ContentEncoding")>]
        member this.ContentEncoding(builder: LazyHttpBuilder<_>, encoding) =
            Dsl.Body.contentEncoding encoding builder.Context|> LazyHttpBuilder

        /// The MIME type of the body of the request (used with POST and PUT requests)
        [<CustomOperation("ContentType")>]
        member this.ContentType(builder: LazyHttpBuilder<_>, contentType) =
            Dsl.Body.contentType contentType builder.Context|> LazyHttpBuilder

        /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        [<CustomOperation("ContentTypeWithEncoding")>]
        member this.ContentTypeWithEncoding(builder: LazyHttpBuilder<_>, contentType, charset) =
            Dsl.Body.contentTypeWithEncoding contentType charset builder.Context|> LazyHttpBuilder


[<AutoOpen>]
module Multipart =
    type LazyHttpBuilder<'context when 'context :> IContext> with

        [<CustomOperation("multipart")>]
        member this.Multipart(builder: LazyHttpBuilder<_>) =
            Dsl.Multipart.multipart builder.Context|> LazyHttpBuilder

        [<CustomOperation("part")>]
        member this.Part(builder: LazyHttpBuilder<_>, content, defaultContentType, name) =
            Dsl.Multipart.part content defaultContentType name builder.Context|> LazyHttpBuilder

        [<CustomOperation("valuePart")>]
        member this.ValuePart(builder: LazyHttpBuilder<_>, name, value) =
            Dsl.Multipart.valuePart name value builder.Context|> LazyHttpBuilder

        [<CustomOperation("filePart")>]
        member this.FilePart(builder: LazyHttpBuilder<_>, path) =
            Dsl.Multipart.filePart path builder.Context|> LazyHttpBuilder

        [<CustomOperation("filePartWithName")>]
        member this.FilePartWithName(builder: LazyHttpBuilder<_>, name, path) =
            Dsl.Multipart.filePartWithName name path builder.Context|> LazyHttpBuilder

        /// The MIME type of the body of the request (used with POST and PUT requests)
        [<CustomOperation("ContentTypePart")>]
        member this.ContentTypePart(builder: LazyHttpBuilder<_>, contentType) =
            Dsl.Multipart.contentType contentType builder.Context|> LazyHttpBuilder


[<AutoOpen>]
module Config =

    type LazyHttpBuilder<'context when 'context :> IContext> with

        [<CustomOperation("configure")>]
        member inline this.Configure(builder: LazyHttpBuilder<_>, configTransformer) =
            Dsl.Config.configure configTransformer builder.Context |> LazyHttpBuilder

        // TODO: Provide certStrategy configs
        [<CustomOperation("ignoreCertIssues")>]
        member inline this.IgnoreCertIssues(builder: LazyHttpBuilder<_>) =
            Dsl.Config.ignoreCertIssues builder.Context |> LazyHttpBuilder

        [<CustomOperation("timeout")>]
        member inline this.Timeout(builder: LazyHttpBuilder<_>, value) =
            Dsl.Config.timeout value builder.Context|> LazyHttpBuilder

        [<CustomOperation("timeoutInSeconds")>]
        member inline this.TimeoutInSeconds(builder: LazyHttpBuilder<_>, value) =
            Dsl.Config.timeoutInSeconds value builder.Context|> LazyHttpBuilder

        [<CustomOperation("transformHttpRequestMessage")>]
        member inline this.TransformHttpRequestMessage(builder: LazyHttpBuilder<_>, map) =
            Dsl.Config.transformHttpRequestMessage map builder.Context|> LazyHttpBuilder

        [<CustomOperation("transformHttpClient")>]
        member inline this.TransformHttpClient(builder: LazyHttpBuilder<_>, map) =
            Dsl.Config.transformHttpClient map builder.Context|> LazyHttpBuilder

        [<CustomOperation("transformHttpClientHandler")>]
        member inline this.TransformHttpClientHandler(builder: LazyHttpBuilder<_>, map) =
            Dsl.Config.transformHttpClientHandler map builder.Context|> LazyHttpBuilder

        [<CustomOperation("proxy")>]
        member inline this.Proxy(builder: LazyHttpBuilder<_>, url) =
            Dsl.Config.proxy url builder.Context|> LazyHttpBuilder

        [<CustomOperation("proxyWithCredentials")>]
        member inline this.ProxyWithCredentials(builder: LazyHttpBuilder<_>, url, credentials) =
            Dsl.Config.proxyWithCredentials url credentials builder.Context|> LazyHttpBuilder

        /// Inject a HttpClient that will be used directly (most config parameters specified here will be ignored). 
        [<CustomOperation("useHttpClient")>]
        member inline this.UseHttpClient(builder: LazyHttpBuilder<_>, client: System.Net.Http.HttpClient) =
            Dsl.Config.useHttpClient client builder.Context|> LazyHttpBuilder


[<AutoOpen>]
module Execution =

    type LazyHttpBuilder<'context when 'context :> IContext> with
    
        [<CustomOperation("send")>]
        member this.Send(builder: LazyHttpBuilder<_>) =
            builder.Context |> Request.send
    
        [<CustomOperation("sendAsync")>]
        member this.SendAsync(builder: LazyHttpBuilder<_>) =
            builder.Context |> Request.sendAsync


[<AutoOpen>]
module Fsi =

    open FsHttp.Fsi

    let raw = rawPrinterTransformer |> modifyPrinter
    let headerOnly = headerOnlyPrinterTransformer |> modifyPrinter
    let show maxLength = showPrinterTransformer maxLength |> modifyPrinter
    let preview = previewPrinterTransformer |> modifyPrinter
    let prv = preview
    let expand = expandPrinterTransformer |> modifyPrinter
    let exp = expand

    let inline private modifyPrintHint f (context: ^t when ^t :> IContext) =
        let transformPrintHint (config: Config) = { config with printHint = f config.printHint }
        let res = (^t: (member Configure: (Config -> Config) -> ^t) (context, transformPrintHint))
        res |> LazyHttpBuilder<_>

    //let inline private modifyPrintHintAndSend f (context: ^t when ^t :> IContext) =
    //    modifyPrintHint f context |> Request.send

    type LazyHttpBuilder<'context when 'context :> IContext> with

        [<CustomOperation("raw")>]
        member inline this.Raw(builder: LazyHttpBuilder<_>) =
            modifyPrintHint rawPrinterTransformer builder.Context

        [<CustomOperation("headerOnly")>]
        member inline this.HeaderOnly(builder: LazyHttpBuilder<_>) =
            modifyPrintHint headerOnlyPrinterTransformer builder.Context

        [<CustomOperation("show")>]
        member inline this.Show(builder: LazyHttpBuilder<_>, maxLength) =
            modifyPrintHint (showPrinterTransformer maxLength) builder.Context

        [<CustomOperation("preview")>]
        member inline this.Preview(builder: LazyHttpBuilder<_>) =
            modifyPrintHint previewPrinterTransformer builder.Context

        [<CustomOperation("prv")>]
        member inline this.Prv(builder: LazyHttpBuilder<_>) =
            modifyPrintHint previewPrinterTransformer builder.Context

        [<CustomOperation("expand")>]
        member inline this.Expand(builder: LazyHttpBuilder<_>) =
            modifyPrintHint expandPrinterTransformer builder.Context

        [<CustomOperation("exp")>]
        member inline this.Exp(builder: LazyHttpBuilder<_>) =
            modifyPrintHint expandPrinterTransformer builder.Context

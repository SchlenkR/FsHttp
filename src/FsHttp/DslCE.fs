module FsHttp.DslCE

open Domain

type HttpBuilder<'context>(context: 'context) =
    member this.Context = context

    member this.Yield(_) = HttpBuilder context
    member this.Delay(f) = f


[<AutoOpen>]
module Method =
    // RFC 2626 specifies 8 methods + PATCH
    let request (method: string) (url: string) = Dsl.Method.request method url |> HttpBuilder
    let get (url: string) = Dsl.Method.get url |> HttpBuilder
    let put (url: string) = Dsl.Method.put url |> HttpBuilder
    let post (url: string) = Dsl.Method.post url |> HttpBuilder
    let delete (url: string) = Dsl.Method.delete url |> HttpBuilder
    let options (url: string) = Dsl.Method.options url |> HttpBuilder
    let head (url: string) = Dsl.Method.head url |> HttpBuilder
    let trace (url: string) = Dsl.Method.trace url |> HttpBuilder
    let connect (url: string) = Dsl.Method.connect url |> HttpBuilder
    let patch (url: string) = Dsl.Method.patch url |> HttpBuilder

    // TODO
    //type HttpBuilder<'context> with

    //    [<CustomOperation("Request")>]
    //    member this.Request(StartingContext, method, url) = Request.request method url

    //    // RFC 2626 specifies 8 methods
    //    [<CustomOperation("GET")>]
    //    member this.Get(StartingContext, url) = Request.get url

    //    [<CustomOperation("PUT")>]
    //    member this.Put(StartingContext, url) = Request.put url

    //    [<CustomOperation("POST")>]
    //    member this.Post(StartingContext, url) = Request.post url

    //    [<CustomOperation("DELETE")>]
    //    member this.Delete(StartingContext, url) = Request.delete url

    //    [<CustomOperation("OPTIONS")>]
    //    member this.Options(StartingContext, url) = Request.options url

    //    [<CustomOperation("HEAD")>]
    //    member this.Head(StartingContext, url) = Request.head url

    //    [<CustomOperation("TRACE")>]
    //    member this.Trace(StartingContext, url) = Request.trace url

    //    [<CustomOperation("CONNECT")>]
    //    member this.Connect(StartingContext, url) = Request.connect url

    //    [<CustomOperation("PATCH")>]
    //    member this.Patch(StartingContext, url) = Request.patch url

// RFC 4918 (WebDAV) adds 7 methods
// TODO


[<AutoOpen>]
module Header =
    type HttpBuilder<'context> with

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

        /// Append query params
        [<CustomOperation("Query")>]
        member this.Query(builder: HttpBuilder<_>, queryParams) =
            Dsl.Header.query queryParams builder.Context |> HttpBuilder
        
        /// Authentication credentials for HTTP authentication
        [<CustomOperation("Authorization")>]
        member this.Authorization(builder: HttpBuilder<_>, credentials) =
            Dsl.Header.authorization credentials builder.Context |> HttpBuilder

        /// Authentication header using Bearer Auth token
        [<CustomOperation("BearerAuth")>]
        member this.BearerAuth(builder: HttpBuilder<_>, token) =
            Dsl.Header.bearerAuth token builder.Context |> HttpBuilder

        /// Authentication header using Basic Auth encoding
        [<CustomOperation("BasicAuth")>]
        member this.BasicAuth(builder: HttpBuilder<_>, username, password) =
            Dsl.Header.basicAuth username password builder.Context |> HttpBuilder

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

        /// Custom header
        [<CustomOperation("Header")>]
        member this.Header(builder: HttpBuilder<_>, key, value) =
            Dsl.Header.header key value builder.Context |> HttpBuilder

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


[<AutoOpen>]
module Body =
    type HttpBuilder<'context> with

        [<CustomOperation("body")>]
        member this.Body(builder: HttpBuilder<_>) =
            Dsl.Body.body builder.Context |> HttpBuilder

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


[<AutoOpen>]
module Multipart =
    type HttpBuilder<'context> with

        [<CustomOperation("multipart")>]
        member this.Multipart(builder: HttpBuilder<_>) =
            Dsl.Multipart.multipart builder.Context |> HttpBuilder

        [<CustomOperation("part")>]
        member this.Part(builder: HttpBuilder<_>, content, defaultContentType, name) =
            Dsl.Multipart.part content defaultContentType name builder.Context |> HttpBuilder

        [<CustomOperation("valuePart")>]
        member this.ValuePart(builder: HttpBuilder<_>, name, value) =
            Dsl.Multipart.valuePart name value builder.Context |> HttpBuilder

        [<CustomOperation("filePart")>]
        member this.FilePart(builder: HttpBuilder<_>, path) =
            Dsl.Multipart.filePart path builder.Context |> HttpBuilder

        [<CustomOperation("filePartWithName")>]
        member this.FilePartWithName(builder: HttpBuilder<_>, name, path) =
            Dsl.Multipart.filePartWithName name path builder.Context |> HttpBuilder

        /// The MIME type of the body of the request (used with POST and PUT requests)
        [<CustomOperation("ContentTypePart")>]
        member this.ContentTypePart(builder: HttpBuilder<_>, contentType) =
            Dsl.Multipart.contentType contentType builder.Context |> HttpBuilder


[<AutoOpen>]
module Config =
    
    open System.Net.Http
    
    type HttpBuilder<'context> with

        [<CustomOperation("timeout")>]
        member this.Timeout(builder: HttpBuilder<_>, value) =
            Dsl.Config.timeout value builder.Context |> HttpBuilder

        [<CustomOperation("timeoutInSeconds")>]
        member this.TimeoutInSeconds(builder: HttpBuilder<_>, value) =
            Dsl.Config.timeoutInSeconds value builder.Context |> HttpBuilder

        [<CustomOperation("transformHttpRequestMessage")>]
        member this.TransformHttpRequestMessage(builder: HttpBuilder<_>, map) =
            Dsl.Config.transformHttpRequestMessage map builder.Context |> HttpBuilder

        [<CustomOperation("transformHttpClient")>]
        member this.TransformHttpClient(builder: HttpBuilder<_>, map) =
            Dsl.Config.transformHttpClient map builder.Context |> HttpBuilder

        [<CustomOperation("transformHttpClientHandler")>]
        member this.TransformHttpClientHandler(builder: HttpBuilder<_>, map) =
            Dsl.Config.transformHttpClientHandler map builder.Context |> HttpBuilder

        [<CustomOperation("proxy")>]
        member this.Proxy(builder: HttpBuilder<_>, url) =
            Dsl.Config.proxy url builder.Context |> HttpBuilder

        [<CustomOperation("proxyWithCredentials")>]
        member this.ProxyWithCredentials(builder: HttpBuilder<_>, url, credentials) =
            Dsl.Config.proxyWithCredentials url credentials builder.Context |> HttpBuilder

        /// Inject a HttpClient that will be used directly (most config parameters specified here will be ignored). 
        [<CustomOperation("useHttpClient")>]
        member this.UseHttpClient(builder: HttpBuilder<_>, client: HttpClient) =
            Dsl.Config.useHttpClient client builder.Context |> HttpBuilder

[<AutoOpen>]
module Builder =

    /// supports the ```http { .. }``` syntax
    type ExplicitHttpBuilder() =
        inherit HttpBuilder<StartingContext>(StartingContext)

        [<CustomOperation("Request")>]
        member this.Request(_: HttpBuilder<StartingContext>, method, url) = request method url

        // RFC 2626 specifies 8 methods
        [<CustomOperation("GET")>]
        member this.Get(_: HttpBuilder<StartingContext>, url) = get url

        [<CustomOperation("PUT")>]
        member this.Put(_: HttpBuilder<StartingContext>, url) = put url

        [<CustomOperation("POST")>]
        member this.Post(_: HttpBuilder<StartingContext>, url) = post url

        [<CustomOperation("DELETE")>]
        member this.Delete(_: HttpBuilder<StartingContext>, url) = delete url

        [<CustomOperation("OPTIONS")>]
        member this.Options(_: HttpBuilder<StartingContext>, url) = options url

        [<CustomOperation("HEAD")>]
        member this.Head(_: HttpBuilder<StartingContext>, url) = head url

        [<CustomOperation("TRACE")>]
        member this.Trace(_: HttpBuilder<StartingContext>, url) = trace url

        [<CustomOperation("CONNECT")>]
        member this.Connect(_: HttpBuilder<StartingContext>, url) = connect url

        [<CustomOperation("PATCH")>]
        member this.Patch(_: HttpBuilder<StartingContext>, url) = patch url

    let http = ExplicitHttpBuilder()

//    type HttpBuilderBase with
//        member this.Bind(m, f) = f m
//        member this.Return(x) = x
//        member this.For(m, f) = this.Bind m f

//    type HttpRequestBuilder<'a>(context: 'a) =
//        inherit HttpBuilderBase()
//        member this.Yield(x) = context

//    let httpRequest context = HttpRequestBuilder context

//    // TODO: this can be a better way of chaining requests
//    // (as a replacement / enhancement for HttpRequestBuilder):
//    //type HttpChainableBuilder<'a>(context: 'a) =
//    //    inherit HttpBuilderBase()
//    //    member this.Yield(x) = context
//    //    member this.Delay(f: unit -> 'a) = HttpChainableBuilder<'a>(f())
//    //let httpChain context = HttpContextBuilder context

//    type HttpStartingBuilder() =
//        inherit HttpRequestBuilder<StartingContext>(StartingContext)

//    type HttpBuilderSync() =
//        inherit HttpStartingBuilder()
//        member inline this.Delay(f: unit -> 'a) = f() |> Request.send

//    let http = HttpBuilderSync()

//    type HttpBuilderAsync() =
//        inherit HttpStartingBuilder()
//        member inline this.Delay(f: unit -> 'a) = f() |> Request.sendAsync

//    let httpAsync = HttpBuilderAsync()

//    type HttpBuilderLazy() =
//        inherit HttpStartingBuilder()

//    let httpLazy = HttpBuilderLazy()

//    type HttpMessageBuilder() =
//        inherit HttpStartingBuilder()
//        member inline this.Delay(f: unit -> IContext) =
//            f()
//            |> fun context -> context.ToRequest()
//            |> Request.toMessage

//    let httpMsg = HttpMessageBuilder()


[<AutoOpen>]
module Fsi =

    open FsHttp.Fsi

    let raw = rawPrinterTransformer |> modifyPrinter
    let headerOnly = headerOnlyPrinterTransformer |> modifyPrinter
    let show maxLength = showPrinterTransformer maxLength |> modifyPrinter
    let preview = previewPrinterTransformer |> modifyPrinter
    let prv = preview
    let go = preview
    let expand = expandPrinterTransformer |> modifyPrinter
    let exp = expand

    let inline private modifyPrintHint f (context: ^t) =
        let transformPrintHint (config: Config) = { config with printHint = f config.printHint }
        (^t: (member Configure: (Config -> Config) -> ^t) (context, transformPrintHint))

    type HttpBuilder<'context> with

        [<CustomOperation("raw")>]
        member inline this.Raw(context: ^t) = modifyPrintHint rawPrinterTransformer context

        [<CustomOperation("headerOnly")>]
        member inline this.HeaderOnly(context: ^t) = modifyPrintHint headerOnlyPrinterTransformer context

        [<CustomOperation("show")>]
        member inline this.Show(context: ^t, maxLength) = modifyPrintHint (showPrinterTransformer maxLength) context

        [<CustomOperation("preview")>]
        member inline this.Preview(context: ^t) = modifyPrintHint previewPrinterTransformer context

        [<CustomOperation("prv")>]
        member inline this.Prv(context: ^t) = modifyPrintHint previewPrinterTransformer context

        [<CustomOperation("go")>]
        member inline this.Go(context: ^t) = modifyPrintHint previewPrinterTransformer context

        [<CustomOperation("expand")>]
        member inline this.Expand(context: ^t) = modifyPrintHint expandPrinterTransformer context

        [<CustomOperation("exp")>]
        member inline this.Exp(context: ^t) = modifyPrintHint expandPrinterTransformer context

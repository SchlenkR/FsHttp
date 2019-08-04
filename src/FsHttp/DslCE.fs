
namespace FsHttp

open Dsl

[<AutoOpen>]
module DslCE =

    [<AutoOpen>]
    module Operators =

        /// synchronous request invocation
        let inline ( .> ) context f = send context |>  f

        /// asynchronous request invocation
        let inline ( >. ) context f = sendAsync |> context f

    // Request Methods
    type HttpBuilder with

        [<CustomOperation("Request")>]
        member this.Request(StartingContext, method, url) =
            request method url id

        // RFC 2626 specifies 8 methods
        [<CustomOperation("GET")>]
        member this.Get(StartingContext, url) =
            get url id
        
        [<CustomOperation("PUT")>]
        member this.Put(StartingContext, url) =
            put url id
        
        [<CustomOperation("POST")>]
        member this.Post(StartingContext, url) =
            post url id
        
        [<CustomOperation("DELETE")>]
        member this.Delete(StartingContext, url) =
            delete url id
        
        [<CustomOperation("OPTIONS")>]
        member this.Options(StartingContext, url) =
            options url id
        
        [<CustomOperation("HEAD")>]
        member this.Head(StartingContext, url) =
            head url id
        
        [<CustomOperation("TRACE")>]
        member this.Trace(StartingContext, url) =
            trace url id
                
        [<CustomOperation("CONNECT")>]
        member this.Connect (StartingContext, url) =
            connect url id
                
        [<CustomOperation("PATCH")>]
        member this.Patch (StartingContext, url) =
            patch url id

        // RFC 4918 (WebDAV) adds 7 methods
        // TODO


    // Request Headers
    type HttpBuilder with

        /// Content-Types that are acceptable for the response
        [<CustomOperation("Accept")>]
        member this.Accept (context, contentType) =
            Dsl.H.accept context contentType id

        /// Character sets that are acceptable
        [<CustomOperation("AcceptCharset")>]
        member this.AcceptCharset (context, characterSets) =
            Dsl.H.acceptCharset context characterSets id

        /// Acceptable version in time
        [<CustomOperation("AcceptDatetime")>]
        member this.AcceptDatetime (context, dateTime) =
            Dsl.H.acceptDatetime context dateTime id
        
        /// List of acceptable encodings. See HTTP compression.
        [<CustomOperation("AcceptEncoding")>]
        member this.AcceptEncoding (context, encoding) =
            Dsl.H.acceptEncoding context encoding id
        
        /// List of acceptable human languages for response
        [<CustomOperation("AcceptLanguage")>]
        member this.AcceptLanguage (context, language) =
            Dsl.H.acceptLanguage context language id
        
        /// The Allow header, which specifies the set of HTTP methods supported.
        [<CustomOperation("Allow")>]
        member this.Allow (context, methods) =
            Dsl.H.allow context methods id
        
        /// Authentication credentials for HTTP authentication
        [<CustomOperation("Authorization")>]
        member this.Authorization (context, credentials) =
            Dsl.H.authorization context credentials id
        
        /// Authentication header using Bearer Auth token
        [<CustomOperation("BearerAuth")>]
        member this.BearerAuth (context, token) =
            Dsl.H.bearerAuth context token id
        
        /// Authentication header using Basic Auth encoding
        [<CustomOperation("BasicAuth")>]
        member this.BasicAuth (context, username, password) =
            Dsl.H.basicAuth context username password id
        
        /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
        [<CustomOperation("CacheControl")>]
        member this.CacheControl (context, control) =
            Dsl.H.cacheControl context control id
        
        /// What type of connection the user-agent would prefer
        [<CustomOperation("Connection")>]
        member this.Connection (context, connection) =
            Dsl.H.connection context connection id
        
        /// Describes the placement of the content. Valid dispositions are: inline, attachment, form-data
        [<CustomOperation("ContentDisposition")>]
        member this.ContentDisposition (context, placement, name, fileName) =
            Dsl.H.contentDisposition context placement name fileName id
        
        /// The type of encoding used on the data
        [<CustomOperation("ContentEncoding")>]
        member this.ContentEncoding (context, encoding) =
            Dsl.H.contentEncoding context encoding id
        
        /// The language the content is in
        [<CustomOperation("ContentLanguage")>]
        member this.ContentLanguage (context, language) =
            Dsl.H.contentLanguage context language id
        
        /// An alternate location for the returned data
        [<CustomOperation("ContentLocation")>]
        member this.ContentLocation (context, location) =
            Dsl.H.contentLocation context location id
        
        /// A Base64-encoded binary MD5 sum of the content of the request body
        [<CustomOperation("ContentMD5")>]
        member this.ContentMD5 (context, md5sum) =
            Dsl.H.contentMD5 context md5sum id
        
        /// Where in a full body message this partial message belongs
        [<CustomOperation("ContentRange")>]
        member this.ContentRange (context, range) =
            Dsl.H.contentRange context range id

        /// The date and time that the message was sent
        [<CustomOperation("Date")>]
        member this.Date (context, date, id) =
            Dsl.H.date context date id
        
        /// Indicates that particular server behaviors are required by the client
        [<CustomOperation("Expect")>]
        member this.Expect (context, behaviors) =
            Dsl.H.expect context behaviors id
        
        /// Gives the date/time after which the response is considered stale
        [<CustomOperation("Expires")>]
        member this.Expires (context, dateTime) =
            Dsl.H.expires context dateTime id
        
        /// The email address of the user making the request
        [<CustomOperation("From")>]
        member this.From (context, email) =
            Dsl.H.from context email id
        
        /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
        /// The port number may be omitted if the port is the standard port for the service requested.
        [<CustomOperation("Host")>]
        member this.Host (context, host) =
            Dsl.H.host context host id
        
        /// Only perform the action if the client supplied entity matches the same entity on the server.
        /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
        [<CustomOperation("IfMatch")>]
        member this.IfMatch (context, entity) =
            Dsl.H.ifMatch context entity id
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        [<CustomOperation("IfModifiedSince")>]
        member this.IfModifiedSince (context, dateTime) =
            Dsl.H.ifModifiedSince context dateTime id
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        [<CustomOperation("IfNoneMatch")>]
        member this.IfNoneMatch (context, etag) =
            Dsl.H.ifNoneMatch context etag id
        
        /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
        [<CustomOperation("IfRange")>]
        member this.IfRange (context, range) =
            Dsl.H.ifRange context range id
        
        /// Only send the response if the entity has not been modified since a specific time
        [<CustomOperation("IfUnmodifiedSince")>]
        member this.IfUnmodifiedSince (context, dateTime) =
            Dsl.H.ifUnmodifiedSince context dateTime id
        
        /// Specifies a parameter used into order to maintain a persistent connection
        [<CustomOperation("KeepAlive")>]
        member this.KeepAlive (context, keepAlive) =
            Dsl.H.keepAlive context keepAlive id
        
        /// Specifies the date and time at which the accompanying body data was last modified
        [<CustomOperation("LastModified")>]
        member this.LastModified (context, dateTime) =
            Dsl.H.lastModified context dateTime id
        
        /// Limit the number of times the message can be forwarded through proxies or gateways
        [<CustomOperation("MaxForwards")>]
        member this.MaxForwards (context, count) =
            Dsl.H.maxForwards context count id
        
        /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
        [<CustomOperation("Origin")>]
        member this.Origin (context, origin) =
            Dsl.H.origin context origin id
        
        /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
        [<CustomOperation("Pragma")>]
        member this.Pragma (context, pragma) =
            Dsl.H.pragma context pragma id
        
        /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
        [<CustomOperation("Prefer")>]
        member this.Prefer (context, prefer) =
            Dsl.H.prefer context prefer id
        
        /// Authorization credentials for connecting to a proxy.
        [<CustomOperation("ProxyAuthorization")>]
        member this.ProxyAuthorization (context, credentials) =
            Dsl.H.proxyAuthorization context credentials id
        
        /// Request only part of an entity. Bytes are numbered from 0
        [<CustomOperation("Range")>]
        member this.Range (context, start, finish) =
            Dsl.H.range context start finish id
        
        /// This is the address of the previous web page from which a link to the currently requested page was followed.
        /// (The word "referrer" is misspelled in the RFC as well as in most implementations.)
        [<CustomOperation("Referer")>]
        member this.Referer (context, referer) =
            Dsl.H.referer context referer id
        
        /// The transfer encodings the user agent is willing to accept: the same values as for the response header
        /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
        /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
        [<CustomOperation("TE")>]
        member this.TE (context, te) =
            Dsl.H.te context te id
        
        /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
        [<CustomOperation("Trailer")>]
        member this.Trailer (context, trailer) =
            Dsl.H.trailer context trailer id
        
        /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.
        /// The valid directives are one of: chunked, compress, deflate, gzip, or identity.
        [<CustomOperation("TransferEncoding")>]
        member this.TransferEncoding (context, directive) =
            Dsl.H.transferEncoding context directive id
        
        /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
        [<CustomOperation("Translate")>]
        member this.Translate (context, translate) =
            Dsl.H.translate context translate id
        
        /// Specifies additional communications protocols that the client supports.
        [<CustomOperation("Upgrade")>]
        member this.Upgrade (context, upgrade) =
            Dsl.H.upgrade context upgrade id
        
        /// The user agent string of the user agent
        [<CustomOperation("UserAgent")>]
        member this.UserAgent (context, userAgent) =
            Dsl.H.userAgent context userAgent id
        
        /// Informs the server of proxies through which the request was sent
        [<CustomOperation("Via")>]
        member this.Via (context, server) =
            Dsl.H.via context server id
        
        /// A general warning about possible problems with the entity body
        [<CustomOperation("Warning")>]
        member this.Warning (context, message) =
            Dsl.H.warning context message id
        
        /// Override HTTP method.
        [<CustomOperation("XHTTPMethodOverride")>]
        member this.XHTTPMethodOverride (context, httpMethod) =
            Dsl.H.xhttpMethodOverride context httpMethod id


    // Body
    type HttpBuilder with

        [<CustomOperation("body")>]
        member this.Body(context) =
            Dsl.B.body context

        // TODO: Binary
        // TODO: Base64
        
        // TODO
        // // [<CustomOperation("binary")>]
        // // member this.Binary(context, data: byte[]) =
        // //     let content = context.content
        // //     let contentType =
        // //         if context.content.contentType = null then
        // //             "text/plain" 
        // //         else 
        // //             context.content.contentType
        // //     { context with
        // //         content = { content with content=text; contentType=contentType;  }
        // //     }
        
        [<CustomOperation("text")>]
        member this.Text(context, text) =
            Dsl.B.text context text id
        
        [<CustomOperation("json")>]
        member this.Json(context, json) =
            Dsl.B.json context json id

        [<CustomOperation("formUrlEncoded")>]
        member this.FormUrlEncoded(context, data) =
            Dsl.B.formUrlEncoded context data id

        /// The MIME type of the body of the request (used with POST and PUT requests)
        [<CustomOperation("ContentType")>]
        member this.ContentType (context, contentType) =
            Dsl.B.contentType context contentType id
                    
        /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        [<CustomOperation("ContentTypeWithEncoding")>]
        member this.ContentTypeWithEncoding (context, contentType, charset) =
            Dsl.B.contentTypeWithEncoding context contentType charset id


    // Request Headers
    type HttpBuilder with
        
        [<CustomOperation("timeout")>]
        member inline this.Timeout (context, value) =
            Dsl.Config.timeout context value id
        
        [<CustomOperation("timeoutInSeconds")>]
        member inline this.TimeoutInSeconds (context, value) =
            Dsl.Config.timeoutInSeconds context value id
        
        [<CustomOperation("transformHttpRequestMessage")>]
        member inline this.TransformHttpRequestMessage (context, map) =
            Dsl.Config.transformHttpRequestMessage context map id
        
        [<CustomOperation("transformHttpClient")>]
        member inline this.TransformHttpClient (context, map) =
            Dsl.Config.transformHttpClient context map id

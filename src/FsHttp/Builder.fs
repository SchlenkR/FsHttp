
namespace FsHttp

open System.Net.Http
open FsHttp.Dsl

[<AutoOpen>]
module Builder =


    // Request Methods
    type HttpBuilder with

        [<CustomOperation("Request")>]
        member this.Request(StartingContext, method, url) = Requests.request method url

        // RFC 2626 specifies 8 methods
        [<CustomOperation("GET")>]
        member this.Get(StartingContext, url) = Requests.get url
        
        [<CustomOperation("PUT")>]
        member this.Put(StartingContext, url) = Requests.put url
        
        [<CustomOperation("POST")>]
        member this.Post(StartingContext, url) = Requests.post url
        
        [<CustomOperation("DELETE")>]
        member this.Delete(StartingContext, url) = Requests.delete url
        
        [<CustomOperation("OPTIONS")>]
        member this.Options(StartingContext, url) = Requests.options url
        
        [<CustomOperation("HEAD")>]
        member this.Head(StartingContext, url) = Requests.head url
        
        [<CustomOperation("TRACE")>]
        member this.Trace(StartingContext, url) = Requests.trace url
        
        // TODO: Connect
        // [<CustomOperation("CONNECT")>]
        // member this.Post(StartingContext, url) =
        //     this.CreateRequest StartingContext HttpMethod.Connect url

        // RFC 4918 (WebDAV) adds 7 methods
        // TODO


    // Request Headers
    type HttpBuilder with

        /// Content-Types that are acceptable for the response
        [<CustomOperation("Accept")>]
        member this.Accept (context, contentType) = Headers.accept contentType context

        /// Character sets that are acceptable
        [<CustomOperation("AcceptCharset")>]
        member this.AcceptCharset (context, characterSets) = Headers.acceptCharset characterSets context

        /// Acceptable version in time
        [<CustomOperation("AcceptDatetime")>]
        member this.AcceptDatetime (context, dateTime) = Headers.acceptDatetime dateTime context
        
        /// List of acceptable encodings. See HTTP compression.
        [<CustomOperation("AcceptEncoding")>]
        member this.AcceptEncoding (context, encoding) = Headers.acceptEncoding encoding context
        
        /// List of acceptable human languages for response
        [<CustomOperation("AcceptLanguage")>]
        member this.AcceptLanguage (context, language) = Headers.acceptLanguage language context
        
        /// The Allow header, which specifies the set of HTTP methods supported.
        [<CustomOperation("Allow")>]
        member this.Allow (context, methods) = Headers.allow methods context
        
        /// Authentication credentials for HTTP authentication
        [<CustomOperation("Authorization")>]
        member this.Authorization (context, credentials) = Headers.authorization credentials context
        
        /// Authentication header using Basic Auth encoding
        [<CustomOperation("BasicAuth")>]
        member this.BasicAuth (context, username, password) = Headers.basicAuth username password context
        
        /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
        [<CustomOperation("CacheControl")>]
        member this.CacheControl (context, control) = Headers.cacheControl control context
        
        /// What type of connection the user-agent would prefer
        [<CustomOperation("Connection")>]
        member this.Connection (context, connection) = Headers.connection connection context
        
        /// Describes the placement of the content. Valid dispositions are: inline, attachment, form-data
        [<CustomOperation("ContentDisposition")>]
        member this.ContentDisposition (context, placement, name, fileName) = Headers.contentDisposition placement name fileName context
        
        /// The type of encoding used on the data
        [<CustomOperation("ContentEncoding")>]
        member this.ContentEncoding (context, encoding) = Headers.contentEncoding encoding context
        
        /// The language the content is in
        [<CustomOperation("ContentLanguage")>]
        member this.ContentLanguage (context, language) = Headers.contentLanguage language context
        
        /// An alternate location for the returned data
        [<CustomOperation("ContentLocation")>]
        member this.ContentLocation (context, location) = Headers.contentLocation location context
        
        /// A Base64-encoded binary MD5 sum of the content of the request body
        [<CustomOperation("ContentMD5")>]
        member this.ContentMD5 (context, md5sum) = Headers.contentMD5 md5sum context
        
        /// Where in a full body message this partial message belongs
        [<CustomOperation("ContentRange")>]
        member this.ContentRange (context, range) = Headers.contentRange range context

        /// The date and time that the message was sent
        [<CustomOperation("Date")>]
        member this.Date (context, date) = Headers.date date context
        
        /// Indicates that particular server behaviors are required by the client
        [<CustomOperation("Expect")>]
        member this.Expect (context, behaviors) = Headers.expect behaviors context
        
        /// Gives the date/time after which the response is considered stale
        [<CustomOperation("Expires")>]
        member this.Expires (context, dateTime) = Headers.expires dateTime context
        
        /// The email address of the user making the request
        [<CustomOperation("From")>]
        member this.From (context, email) = Headers.from email context
        
        /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
        /// The port number may be omitted if the port is the standard port for the service requested.
        [<CustomOperation("Host")>]
        member this.Host (context, host) = Headers.host host context
        
        /// Only perform the action if the client supplied entity matches the same entity on the server.
        /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
        [<CustomOperation("IfMatch")>]
        member this.IfMatch (context, entity) = Headers.ifMatch entity context
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        [<CustomOperation("IfModifiedSince")>]
        member this.IfModifiedSince (context, dateTime) = Headers.ifModifiedSince dateTime context
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        [<CustomOperation("IfNoneMatch")>]
        member this.IfNoneMatch (context, etag) = Headers.ifNoneMatch etag context
        
        /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
        [<CustomOperation("IfRange")>]
        member this.IfRange (context, range) = Headers.ifRange range context
        
        /// Only send the response if the entity has not been modified since a specific time
        [<CustomOperation("IfUnmodifiedSince")>]
        member this.IfUnmodifiedSince (context, dateTime) = Headers.ifUnmodifiedSince dateTime context
        
        /// Specifies a parameter used into order to maintain a persistent connection
        [<CustomOperation("KeepAlive")>]
        member this.KeepAlive (context, keepAlive) = Headers.keepAlive keepAlive context
        
        /// Specifies the date and time at which the accompanying body data was last modified
        [<CustomOperation("LastModified")>]
        member this.LastModified (context, dateTime) = Headers.lastModified dateTime context
        
        /// Limit the number of times the message can be forwarded through proxies or gateways
        [<CustomOperation("MaxForwards")>]
        member this.MaxForwards (context, count) = Headers.maxForwards count context
        
        /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
        [<CustomOperation("Origin")>]
        member this.Origin (context, origin) = Headers.origin origin context
        
        /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
        [<CustomOperation("Pragma")>]
        member this.Pragma (context, pragma) = Headers.pragma pragma context
        
        /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
        [<CustomOperation("Prefer")>]
        member this.Prefer (context, prefer) = Headers.prefer prefer context
        
        /// Authorization credentials for connecting to a proxy.
        [<CustomOperation("ProxyAuthorization")>]
        member this.ProxyAuthorization (context, credentials) = Headers.proxyAuthorization credentials context
        
        /// Request only part of an entity. Bytes are numbered from 0
        [<CustomOperation("Range")>]
        member this.Range (context, start, finish) = Headers.range start finish context
        
        /// This is the address of the previous web page from which a link to the currently requested page was followed.
        /// (The word "referrer" is misspelled in the RFC as well as in most implementations.)
        [<CustomOperation("Referer")>]
        member this.Referer (context, referer) = Headers.referer referer context
        
        /// The transfer encodings the user agent is willing to accept: the same values as for the response header
        /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
        /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
        [<CustomOperation("TE")>]
        member this.TE (context, te) = Headers.te te context
        
        /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
        [<CustomOperation("Trailer")>]
        member this.Trailer (context, trailer) = Headers.trailer trailer context
        
        /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.
        /// The valid directives are one of: chunked, compress, deflate, gzip, or identity.
        [<CustomOperation("TransferEncoding")>]
        member this.TransferEncoding (context, directive) = Headers.transferEncoding directive context
        
        /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
        [<CustomOperation("Translate")>]
        member this.Translate (context, translate) = Headers.translate translate context
        
        /// Specifies additional communications protocols that the client supports.
        [<CustomOperation("Upgrade")>]
        member this.Upgrade (context, upgrade) = Headers.upgrade upgrade context
        
        /// The user agent string of the user agent
        [<CustomOperation("UserAgent")>]
        member this.UserAgent (context, userAgent) = Headers.userAgent userAgent context
        
        /// Informs the server of proxies through which the request was sent
        [<CustomOperation("Via")>]
        member this.Via (context, server) = Headers.via server context
        
        /// A general warning about possible problems with the entity body
        [<CustomOperation("Warning")>]
        member this.Warning (context, message) = Headers.warning message context
        
        /// Override HTTP method.
        [<CustomOperation("XHTTPMethodOverride")>]
        member this.XHTTPMethodOverride (context, httpMethod) = Headers.xhttpMethodOverride httpMethod context


    // Body
    type HttpBuilder with

        [<CustomOperation("body")>]
        member this.Body(context) = Body.body context

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
        member this.Text(context, text) = Body.text text context
        
        [<CustomOperation("json")>]
        member this.Json(context, json) = Body.json json context

        [<CustomOperation("formUrlEncoded")>]
        member this.FormUrlEncoded(context, data) = Body.formUrlEncoded data context

        /// The MIME type of the body of the request (used with POST and PUT requests)
        [<CustomOperation("ContentType")>]
        member this.ContentType (context, contentType) = Body.contentType contentType context
                    
        /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        [<CustomOperation("ContentTypeWithEncoding")>]
        member this.ContentTypeWithEncoding (context, contentType, charset) = Body.contentTypeWithEncoding contentType charset context

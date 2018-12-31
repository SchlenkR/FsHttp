
namespace FsHttp

open System.Net.Http
open FsHttp
open FsHttp.Dsl

[<AutoOpen>]
module Builder =

    // Request
    type HttpBuilder with

        [<CustomOperation("Request")>]
        member this.request(c, m, r) = Requests.request c m r

        // RFC 2626 specifies 8 methods

        [<CustomOperation("GET")>]
        member this.Get(c, url) = Requests.get c url
        
        [<CustomOperation("PUT")>]
        member this.Put(c, url) = Requests.put c url
        
        [<CustomOperation("POST")>]
        member this.Post(c, url) = Requests.post c url
        
        [<CustomOperation("DELETE")>]
        member this.Delete(c, url) = Requests.delete c url
        
        [<CustomOperation("OPTIONS")>]
        member this.Options(c, url) = Requests.options c url
        
        [<CustomOperation("HEAD")>]
        member this.Head(c, url) = Requests.head c url
        
        [<CustomOperation("TRACE")>]
        member this.Trace(c, url) = Requests.trace c url
        
        // TODO: Connect
        // [<CustomOperation("CONNECT")>]
        // member this.Post(StartingContext, url: string) =
        //     this.CreateRequest StartingContext HttpMethod.Connect url

        // RFC 4918 (WebDAV) adds 7 methods
        // TODO

    // HeaderAndBody
    type HttpBuilder with

        [<CustomOperation("header")>]
        member inline this.Header(context: ^t, name, value) =
            (^t: (static member header: ^t * string * string -> ^t) (context,name,value))

    // RequestHeaders
    type HttpBuilder with

        /// Content-Types that are acceptable for the response
        [<CustomOperation("Accept")>]
        member this.Accept(c, x) = Headers.accept c x

        /// Character sets that are acceptable
        [<CustomOperation("AcceptCharset")>]
        member this.AcceptCharset(c, x) = Headers.acceptCharset c x

        /// Acceptable version in time
        [<CustomOperation("AcceptDatetime")>]
        member this.AcceptDatetime(c, x) = Headers.acceptDatetime c x
        
        /// List of acceptable encodings. See HTTP compression.
        [<CustomOperation("AcceptEncoding")>]
        member this.AcceptEncoding(c, x) = Headers.acceptEncoding c x
        
        /// List of acceptable human languages for response
        [<CustomOperation("AcceptLanguage")>]
        member this.AcceptLanguage(c, x) = Headers.acceptLanguage c x
        
        /// The Allow header, which specifies the set of HTTP methods supported.
        [<CustomOperation("Allow")>]
        member this.Allow(c, x) = Headers.allow c x
        
        /// Authentication credentials for HTTP authentication
        [<CustomOperation("Authorization")>]
        member this.Authorization(c, x) = Headers.authorization c x
        
        /// Authentication header using Basic Auth encoding
        [<CustomOperation("BasicAuth")>]
        member this.BasicAuth(c, x) = Headers.basicAuth c x
        
        /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
        [<CustomOperation("CacheControl")>]
        member this.CacheControl(c, x) = Headers.cacheControl c x
        
        /// What type of connection the user-agent would prefer
        [<CustomOperation("Connection")>]
        member this.Connection(c, x) = Headers.connection c x
        
        /// Describes the placement of the content. Valid dispositions are: inline, attachment, form-data
        [<CustomOperation("ContentDisposition")>]
        member this.ContentDisposition(c, x) = Headers.contentDisposition c x
        
        /// The type of encoding used on the data
        [<CustomOperation("ContentEncoding")>]
        member this.ContentEncoding(c, x) = Headers.contentEncoding c x
        
        /// The language the content is in
        [<CustomOperation("ContentLanguage")>]
        member this.ContentLanguage(c, x) = Headers.contentLanguage c x
        
        /// An alternate location for the returned data
        [<CustomOperation("ContentLocation")>]
        member this.ContentLocation(c, x) = Headers.contentLocation c x
        
        /// A Base64-encoded binary MD5 sum of the content of the request body
        [<CustomOperation("ContentMD5")>]
        member this.ContentMD5(c, x) = Headers.contentMD5 c x
        
        /// Where in a full body message this partial message belongs
        [<CustomOperation("ContentRange")>]
        member this.ContentRange(c, x) = Headers.contentRange c x

        // this is a property of the body.        
        // // /// The MIME type of the body of the request (used with POST and PUT requests)
        // // [<CustomOperation("ContentType")>]
        // // member this.ContentType (context: HeaderContext, contentType: string) =
        // //     this.Header(context, "Content-Type", contentType)
        /////// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        ////[<CustomOperation("ContentTypeWithEncoding")>]
        ////member this.ContentTypeWithEncoding (context: HeaderContext, contentType, charset:Encoding) =
        ////    this.Header(context, "Content-Type", sprintf "%s; charset=%s" contentType (charset.WebName))
        
        /// The date and time that the message was sent
        [<CustomOperation("Date")>]
        member this.Date(c, x) = Headers.date c x
        
        /// Indicates that particular server behaviors are required by the client
        [<CustomOperation("Expect")>]
        member this.Expect(c, x) = Headers.expect c x
        
        /// Gives the date/time after which the response is considered stale
        [<CustomOperation("Expires")>]
        member this.Expires(c, x) = Headers.expires c x
        
        /// The email address of the user making the request
        [<CustomOperation("From")>]
        member this.From(c, x) = Headers.from c x
        
        /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
        /// The port number may be omitted if the port is the standard port for the service requested.
        [<CustomOperation("Host")>]
        member this.Host(c, x) = Headers.host c x
        
        /// Only perform the action if the client supplied entity matches the same entity on the server.
        /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
        [<CustomOperation("IfMatch")>]
        member this.IfMatch(c, x) = Headers.ifMatch c x
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        [<CustomOperation("IfModifiedSince")>]
        member this.IfModifiedSince(c, x) = Headers.ifModifiedSince c x
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        [<CustomOperation("IfNoneMatch")>]
        member this.IfNoneMatch(c, x) = Headers.ifNoneMatch c x
        
        /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
        [<CustomOperation("IfRange")>]
        member this.IfRange(c, x) = Headers.ifRange c x
        
        /// Only send the response if the entity has not been modified since a specific time
        [<CustomOperation("IfUnmodifiedSince")>]
        member this.IfUnmodifiedSince(c, x) = Headers.ifUnmodifiedSince c x
        
        /// Specifies a parameter used into order to maintain a persistent connection
        [<CustomOperation("KeepAlive")>]
        member this.KeepAlive(c, x) = Headers.keepAlive c x
        
        /// Specifies the date and time at which the accompanying body data was last modified
        [<CustomOperation("LastModified")>]
        member this.LastModified(c, x) = Headers.lastModified c x
        
        /// Limit the number of times the message can be forwarded through proxies or gateways
        [<CustomOperation("MaxForwards")>]
        member this.MaxForwards(c, x) = Headers.maxForwards c x
        
        /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
        [<CustomOperation("Origin")>]
        member this.Origin(c, x) = Headers.origin c x
        
        /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
        [<CustomOperation("Pragma")>]
        member this.Pragma(c, x) = Headers.pragma c x
        
        /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
        [<CustomOperation("Prefer")>]
        member this.Prefer(c, x) = Headers.prefer c x
        
        /// Authorization credentials for connecting to a proxy.
        [<CustomOperation("ProxyAuthorization")>]
        member this.ProxyAuthorization(c, x) = Headers.proxyAuthorization c x
        
        /// Request only part of an entity. Bytes are numbered from 0
        [<CustomOperation("Range")>]
        member this.Range(c, x) = Headers.range c x
        
        /// This is the address of the previous web page from which a link to the currently requested page was followed. (The word "referrer" is misspelled in the RFC as well as in most implementations.)
        [<CustomOperation("Referer")>]
        member this.Referer(c, x) = Headers.referer c x
        
        /// The transfer encodings the user agent is willing to accept: the same values as for the response header
        /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
        /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
        [<CustomOperation("TE")>]
        member this.TE(c, x) = Headers.te c x
        
        /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
        [<CustomOperation("Trailer")>]
        member this.Trailer(c, x) = Headers.trailer c x
        
        /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.  The valid directives are one of: chunked, compress, deflate, gzip, or identity.
        [<CustomOperation("TransferEncoding")>]
        member this.TransferEncoding(c, x) = Headers.transferEncoding c x
        
        /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
        [<CustomOperation("Translate")>]
        member this.Translate(c, x) = Headers.translate c x
        
        /// Specifies additional communications protocols that the client supports.
        [<CustomOperation("Upgrade")>]
        member this.Upgrade(c, x) = Headers.upgrade c x
        
        /// The user agent string of the user agent
        [<CustomOperation("UserAgent")>]
        member this.UserAgent(c, x) = Headers.userAgent c x
        
        /// Informs the server of proxies through which the request was sent
        [<CustomOperation("Via")>]
        member this.Via(c, x) = Headers.via c x
        
        /// A general warning about possible problems with the entity body
        [<CustomOperation("Warning")>]
        member this.Warning(c, x) = Headers.warning c x
        
        /// Override HTTP method.
        [<CustomOperation("XHTTPMethodOverride")>]
        member this.XHTTPMethodOverride(c, x) = Headers.xhttpMethodOverride c x


    // Body
    type HttpBuilder with

        [<CustomOperation("body")>]
        member this.Body c = HeaderContext.Body c

        // TODO: Binary
        // TODO: Base64
        
        // TODO
        // // [<CustomOperation("binary")>]
        // // member this.Binary(context: BodyContext, data: byte[]) =
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
        member this.Text(c, x) = Body.text c x
        
        [<CustomOperation("json")>]
        member this.Json(c, x) = Body.json c x

        [<CustomOperation("formUrlEncoded")>]
        member this.FormUrlEncoded(c, x) = Body.formUrlEncoded c x

        /// The MIME type of the body of the request (used with POST and PUT requests)
        [<CustomOperation("ContentType")>]
        member this.ContentType(c, x) = Body.contentType c x
                    
        /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        [<CustomOperation("ContentTypeWithEncoding")>]
        member this.ContentTypeWithEncoding(c, x) = Body.contentTypeWithEncoding c x

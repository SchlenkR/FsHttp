
namespace FsHttp

open System
open System.Net.Http
open System.Text
open System.Globalization

module Dsl =

    [<AutoOpen>]
    module Operators =

        // TODO: Document
        let (%%) = (<|)

        // TODO: Document
        let (--) = (|>)

    [<AutoOpen>]
    module Request =
        
        // RFC 2626 specifies 8 methods + PATCH
        
        let get (url:string) =
            Dsl2.Request.get url id
        
        let put (url:string) =
            Dsl2.Request.put url id
        
        let post (url:string) =
            Dsl2.Request.post url id
        
        let delete (url:string) =
            Dsl2.Request.delete url id
        
        let options (url:string) =
            Dsl2.Request.options url id
        
        let head (url:string) =
            Dsl2.Request.head url id
        
        let trace (url:string) =
            Dsl2.Request.trace url id
        
        let connect (url:string) =
            Dsl2.Request.connect url id
        
        let patch (url:string) =
            Dsl2.Request.patch url id

        // RFC 4918 (WebDAV) adds 7 methods
        // TODO

    [<AutoOpen>]
    module Header =

        /// Content-Types that are acceptable for the response
        let accept contentType context =
            Dsl2.Header.accept context contentType id

        /// Character sets that are acceptable
        let acceptCharset characterSets context =
            Dsl2.Header.acceptCharset context characterSets id

        /// Acceptable version in time
        let acceptDatetime dateTime context =
            Dsl2.Header.acceptDatetime context dateTime id
        
        /// List of acceptable encodings. See HTTP compression.
        let acceptEncoding encoding context =
            Dsl2.Header.acceptEncoding context encoding id
        
        /// List of acceptable human languages for response
        let acceptLanguage language context =
            Dsl2.Header.acceptLanguage context language id
        
        /// The Allow header, which specifies the set of HTTP methods supported.
        let allow methods context =
            Dsl2.Header.allow context methods id
        
        /// Authentication credentials for HTTP authentication
        let authorization credentials context =
            Dsl2.Header.authorization context credentials id
                
        /// Authentication header using Bearer Auth token
        let bearerAuth token context =
            Dsl2.Header.bearerAuth token context id

        /// Authentication header using Basic Auth encoding
        let basicAuth username password context =
            Dsl2.Header.basicAuth context username password id
        
        /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
        let cacheControl control context =
            Dsl2.Header.cacheControl context control id
        
        /// What type of connection the user-agent would prefer
        let connection connection context =
            Dsl2.Header.connection context connection id
        
        /// Describes the placement of the content. Valid dispositions are: inline, attachment, form-data
        let contentDisposition placement name fileName context =
            Dsl2.Header.contentDisposition context placement name fileName id
        
        /// The type of encoding used on the data
        let contentEncoding encoding context =
            Dsl2.Header.contentEncoding context encoding id
        
        /// The language the content is in
        let contentLanguage language context =
            Dsl2.Header.contentLanguage context language id
        
        /// An alternate location for the returned data
        let contentLocation location context =
            Dsl2.Header.contentLocation context location id
        
        /// A Base64-encoded binary MD5 sum of the content of the request body
        let contentMD5 md5sum context =
            Dsl2.Header.contentMD5 context md5sum id
        
        /// Where in a full body message this partial message belongs
        let contentRange range context =
            Dsl2.Header.contentRange context range id

        /// The date and time that the message was sent
        let date date context =
            Dsl2.Header.date context date id
        
        /// Indicates that particular server behaviors are required by the client
        let expect behaviors context =
            Dsl2.Header.expect context behaviors id
        
        /// Gives the date/time after which the response is considered stale
        let expires dateTime context =
            Dsl2.Header.expires context dateTime id
        
        /// The email address of the user making the request
        let from email context =
            Dsl2.Header.from context email id
        
        /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
        /// The port number may be omitted if the port is the standard port for the service requested.
        let host host context =
            Dsl2.Header.host context host id
        
        /// Only perform the action if the client supplied entity matches the same entity on the server.
        /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
        let ifMatch entity context =
            Dsl2.Header.ifMatch context entity id
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        let ifModifiedSince dateTime context =
            Dsl2.Header.ifModifiedSince context dateTime id
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        let ifNoneMatch etag context =
            Dsl2.Header.ifNoneMatch context etag id
        
        /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
        let ifRange range context =
            Dsl2.Header.ifRange context range id
        
        /// Only send the response if the entity has not been modified since a specific time
        let ifUnmodifiedSince dateTime context =
            Dsl2.Header.ifUnmodifiedSince context dateTime id
        
        /// Specifies a parameter used into order to maintain a persistent connection
        let keepAlive keepAlive context =
            Dsl2.Header.keepAlive context keepAlive id
        
        /// Specifies the date and time at which the accompanying body data was last modified
        let lastModified dateTime context =
            Dsl2.Header.lastModified context dateTime id
        
        /// Limit the number of times the message can be forwarded through proxies or gateways
        let maxForwards count context =
            Dsl2.Header.maxForwards context count id
        
        /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
        let origin origin context =
            Dsl2.Header.origin context origin id
        
        /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
        let pragma pragma context =
            Dsl2.Header.pragma context pragma id
        
        /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
        let prefer prefer context =
            Dsl2.Header.prefer context prefer id
        
        /// Authorization credentials for connecting to a proxy.
        let proxyAuthorization credentials context =
            Dsl2.Header.proxyAuthorization context credentials id
        
        /// Request only part of an entity. Bytes are numbered from 0
        let range start finish context =
            Dsl2.Header.range context start finish id
        
        /// This is the address of the previous web page from which a link to the currently requested page was followed. (The word "referrer" is misspelled in the RFC as well as in most implementations.)
        let referer referer context =
            Dsl2.Header.referer context referer id
        
        /// The transfer encodings the user agent is willing to accept: the same values as for the response header
        /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
        /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
        let te te context =
            Dsl2.Header.te context te id
        
        /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
        let trailer trailer context =
            Dsl2.Header.trailer context trailer id
        
        /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.  The valid directives are one of: chunked, compress, deflate, gzip, or identity.
        let transferEncoding directive context =
            Dsl2.Header.transferEncoding context directive id
        
        /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
        let translate translate context =
            Dsl2.Header.translate context translate id
        
        /// Specifies additional communications protocols that the client supports.
        let upgrade upgrade context =
            Dsl2.Header.upgrade context upgrade id
        
        /// The user agent string of the user agent
        let userAgent userAgent context =
            Dsl2.Header.userAgent context userAgent id
        
        /// Informs the server of proxies through which the request was sent
        let via server context =
            Dsl2.Header.via context server id
        
        /// A general warning about possible problems with the entity body
        let warning message context =
            Dsl2.Header.warning context message id
        
        /// Override HTTP method.
        let xhttpMethodOverride httpMethod context =
            Dsl2.Header.xhttpMethodOverride context httpMethod id

    [<AutoOpen>]
    module Body =

        let body headerContext =
            Dsl2.Body.body headerContext id
        
        let text text context =
            Dsl2.Body.text context text id

        let json json context =
            Dsl2.Body.json context json id

        let formUrlEncoded data context =
            Dsl2.Body.formUrlEncoded context data id

        /// The MIME type of the body of the request (used with POST and PUT requests)
        let contentType contentType context =
            Dsl2.Body.contentType context contentType id

        /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        let contentTypeWithEncoding contentTypeString charset context =
            Dsl2.Body.contentTypeWithEncoding context contentTypeString charset id

    [<AutoOpen>]
    module Config =
        
        let inline timeout value context =
            Dsl2.Config.timeout context value id

        let inline timeoutInSeconds value context =
            Dsl2.Config.timeoutInSeconds context value id
        
        let inline transformHttpRequestMessage map context =
            Dsl2.Config.transformHttpRequestMessage context map id
        
        let inline transformHttpClient map context =
            Dsl2.Config.transformHttpClient context map id


module FsHttp.DslPipe


[<AutoOpen>]
module Operators =

    /// Alias for pipe-backward
    let (%%) = (<|)

    /// Alias for pipe-forward
    let (--) = (|>)


[<AutoOpen>]
module R =
    
    // RFC 2626 specifies 8 methods + PATCH
    
    let request (method) url =
        Dsl.R.request method url id
    
    let get url =
        Dsl.R.get url id
    
    let put url =
        Dsl.R.put url id
    
    let post url =
        Dsl.R.post url id
    
    let delete url =
        Dsl.R.delete url id
    
    let options url =
        Dsl.R.options url id
    
    let head url =
        Dsl.R.head url id
    
    let trace url =
        Dsl.R.trace url id
    
    let connect url =
        Dsl.R.connect url id
    
    let patch url =
        Dsl.R.patch url id

    // RFC 4918 (WebDAV) adds 7 methods
    // TODO

[<AutoOpen>]
module H =

    /// Content-Types that are acceptable for the response
    let accept contentType context =
        Dsl.H.accept context contentType id

    /// Character sets that are acceptable
    let acceptCharset characterSets context =
        Dsl.H.acceptCharset context characterSets id

    /// Acceptable version in time
    let acceptDatetime dateTime context =
        Dsl.H.acceptDatetime context dateTime id
    
    /// List of acceptable encodings. See HTTP compression.
    let acceptEncoding encoding context =
        Dsl.H.acceptEncoding context encoding id
    
    /// List of acceptable human languages for response
    let acceptLanguage language context =
        Dsl.H.acceptLanguage context language id
    
    /// The Allow header, which specifies the set of HTTP methods supported.
    let allow methods context =
        Dsl.H.allow context methods id
    
    /// Authentication credentials for HTTP authentication
    let authorization credentials context =
        Dsl.H.authorization context credentials id
            
    /// Authentication header using Bearer Auth token
    let bearerAuth token context =
        Dsl.H.bearerAuth token context id

    /// Authentication header using Basic Auth encoding
    let basicAuth username password context =
        Dsl.H.basicAuth context username password id
    
    /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
    let cacheControl control context =
        Dsl.H.cacheControl context control id
    
    /// What type of connection the user-agent would prefer
    let connection connection context =
        Dsl.H.connection context connection id
    
    /// Describes the placement of the content. Valid dispositions are: inline, attachment, form-data
    let contentDisposition placement name fileName context =
        Dsl.H.contentDisposition context placement name fileName id
    
    /// The type of encoding used on the data
    let contentEncoding encoding context =
        Dsl.H.contentEncoding context encoding id
    
    /// The language the content is in
    let contentLanguage language context =
        Dsl.H.contentLanguage context language id
    
    /// An alternate location for the returned data
    let contentLocation location context =
        Dsl.H.contentLocation context location id
    
    /// A Base64-encoded binary MD5 sum of the content of the request body
    let contentMD5 md5sum context =
        Dsl.H.contentMD5 context md5sum id
    
    /// Where in a full body message this partial message belongs
    let contentRange range context =
        Dsl.H.contentRange context range id

    /// The date and time that the message was sent
    let date date context =
        Dsl.H.date context date id
    
    /// Indicates that particular server behaviors are required by the client
    let expect behaviors context =
        Dsl.H.expect context behaviors id
    
    /// Gives the date/time after which the response is considered stale
    let expires dateTime context =
        Dsl.H.expires context dateTime id
    
    /// The email address of the user making the request
    let from email context =
        Dsl.H.from context email id
    
    /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
    /// The port number may be omitted if the port is the standard port for the service requested.
    let host host context =
        Dsl.H.host context host id
    
    /// Only perform the action if the client supplied entity matches the same entity on the server.
    /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
    let ifMatch entity context =
        Dsl.H.ifMatch context entity id
    
    /// Allows a 304 Not Modified to be returned if content is unchanged
    let ifModifiedSince dateTime context =
        Dsl.H.ifModifiedSince context dateTime id
    
    /// Allows a 304 Not Modified to be returned if content is unchanged
    let ifNoneMatch etag context =
        Dsl.H.ifNoneMatch context etag id
    
    /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
    let ifRange range context =
        Dsl.H.ifRange context range id
    
    /// Only send the response if the entity has not been modified since a specific time
    let ifUnmodifiedSince dateTime context =
        Dsl.H.ifUnmodifiedSince context dateTime id
    
    /// Specifies a parameter used into order to maintain a persistent connection
    let keepAlive keepAlive context =
        Dsl.H.keepAlive context keepAlive id
    
    /// Specifies the date and time at which the accompanying body data was last modified
    let lastModified dateTime context =
        Dsl.H.lastModified context dateTime id
    
    /// Limit the number of times the message can be forwarded through proxies or gateways
    let maxForwards count context =
        Dsl.H.maxForwards context count id
    
    /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
    let origin origin context =
        Dsl.H.origin context origin id
    
    /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
    let pragma pragma context =
        Dsl.H.pragma context pragma id
    
    /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
    let prefer prefer context =
        Dsl.H.prefer context prefer id
    
    /// Authorization credentials for connecting to a proxy.
    let proxyAuthorization credentials context =
        Dsl.H.proxyAuthorization context credentials id
    
    /// Request only part of an entity. Bytes are numbered from 0
    let range start finish context =
        Dsl.H.range context start finish id
    
    /// This is the address of the previous web page from which a link to the currently requested page was followed. (The word "referrer" is misspelled in the RFC as well as in most implementations.)
    let referer referer context =
        Dsl.H.referer context referer id
    
    /// The transfer encodings the user agent is willing to accept: the same values as for the response header
    /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
    /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
    let te te context =
        Dsl.H.te context te id
    
    /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
    let trailer trailer context =
        Dsl.H.trailer context trailer id
    
    /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.  The valid directives are one of: chunked, compress, deflate, gzip, or identity.
    let transferEncoding directive context =
        Dsl.H.transferEncoding context directive id
    
    /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
    let translate translate context =
        Dsl.H.translate context translate id
    
    /// Specifies additional communications protocols that the client supports.
    let upgrade upgrade context =
        Dsl.H.upgrade context upgrade id
    
    /// The user agent string of the user agent
    let userAgent userAgent context =
        Dsl.H.userAgent context userAgent id
    
    /// Informs the server of proxies through which the request was sent
    let via server context =
        Dsl.H.via context server id
    
    /// A general warning about possible problems with the entity body
    let warning message context =
        Dsl.H.warning context message id
    
    /// Override HTTP method.
    let xhttpMethodOverride httpMethod context =
        Dsl.H.xhttpMethodOverride context httpMethod id

    let setCookie name value context =
        Dsl.H.setCookie context name value id
    let setCookieForPath name value path context =
        Dsl.H.setCookieForPath context name value path id
    let setCookieForDomain name value path domain context =
        Dsl.H.setCookieForDomain context name value path domain id

[<AutoOpen>]
module B =

    let body headerContext =
        Dsl.B.body headerContext id
    
    let binary data context =
        Dsl.B.binary context data id
    
    let stream stream context =
        Dsl.B.stream context stream id

    let text text context =
        Dsl.B.text context text id

    let json json context =
        Dsl.B.json context json id

    let formUrlEncoded data context =
        Dsl.B.formUrlEncoded context data id

    /// The MIME type of the body of the request (used with POST and PUT requests)
    let contentType contentType context =
        Dsl.B.contentType context contentType id

    /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
    let contentTypeWithEncoding contentTypeString charset context =
        Dsl.B.contentTypeWithEncoding context contentTypeString charset id

[<AutoOpen>]
module Config =
    
    let timeout value context =
        Dsl.Config.timeout context value id

    let timeoutInSeconds value context =
        Dsl.Config.timeoutInSeconds context value id
    
    let transformHttpRequestMessage map context =
        Dsl.Config.transformHttpRequestMessage context map id
    
    let transformHttpClient map context =
        Dsl.Config.transformHttpClient context map id

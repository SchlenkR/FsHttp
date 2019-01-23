
namespace FsHttp

open System
open System.Net.Http
open System.Text
open System.Globalization

module Helper =

    let urlEncode (s: string) = System.Web.HttpUtility.UrlEncode(s)

    let toBase64 (s: string) =
        let utf8Bytes = Encoding.UTF8.GetBytes(s)
        Convert.ToBase64String(utf8Bytes)

    let fromBase64 (s: string) =
        let base64Bytes = Convert.FromBase64String(s)
        Encoding.UTF8.GetString(base64Bytes)


module Dsl =

    [<AutoOpen>]
    module Operators =

        let (/>) = (<|)

        let (--) = (|>)

    [<AutoOpen>]
    module Request =
        
        let request (method: HttpMethod) (url: string) =

            let formattedUrl =
                url.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries)
                |> Seq.map (fun x -> x.Trim())
                |> Seq.filter (fun x -> not (x.StartsWith("//")))
                // TODO
                //|> Seq.map (fun x ->
                //    if x.StartsWith("?") || x.StartsWith("&")
                //    then x.Substring(1)
                //    else x
                //)
                |> Seq.reduce (+)

            { request = { url=formattedUrl; method=method; headers=[] } }

        // RFC 2626 specifies 8 methods + PATCH
        
        let get (url:string) =
            request HttpMethod.Get url
        
        let put (url:string) =
            request HttpMethod.Put url
        
        let post (url:string) =
            request HttpMethod.Post url
        
        let delete (url:string) =
            request HttpMethod.Delete url
        
        let options (url:string) =
            request HttpMethod.Options url
        
        let head (url:string) =
            request HttpMethod.Head url
        
        let trace (url:string) =
            request HttpMethod.Trace url
        
        let connect (url:string) =
            request (HttpMethod("CONNECT")) url
        
        let patch (url:string) =
            request (HttpMethod("PATCH")) url

        // TODO: Connect
        // [<CustomOperation("CONNECT")>]
        // let Post StartingContext (url:string) =
        //     this.CreateRequest StartingContext HttpMethod.Connect url

        // RFC 4918 (WebDAV) adds 7 methods
        // TODO

    [<AutoOpen>]
    module Header =

        let inline header name value (context: ^t) =
            (^t: (static member Header: ^t * string * string -> ^t) (context,name,value))

        /// Content-Types that are acceptable for the response
        let accept (contentType:string) (context:HeaderContext) =
            header "Accept" contentType context

        /// Character sets that are acceptable
        let acceptCharset (characterSets:string) (context:HeaderContext) =
            header "Accept-Charset" characterSets context

        /// Acceptable version in time
        let acceptDatetime (dateTime:DateTime) (context:HeaderContext) =
            header "Accept-Datetime" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context
        
        /// List of acceptable encodings. See HTTP compression.
        let acceptEncoding (encoding:string) (context:HeaderContext) =
            header "Accept-Encoding" encoding context
        
        /// List of acceptable human languages for response
        let acceptLanguage (language:string) (context:HeaderContext) =
            header "Accept-Language" language context
        
        /// The Allow header, which specifies the set of HTTP methods supported.
        let allow (methods: string) (context:HeaderContext) =
            header "Allow" methods context
        
        /// Authentication credentials for HTTP authentication
        let authorization (credentials: string) (context:HeaderContext) =
            header "Authorization" credentials context
        
        /// Authentication header using Basic Auth encoding
        let basicAuth (username: string) (password: string) (context:HeaderContext) =
            let s (context:HeaderContext) = sprintf "%s:%s" username password |> Helper.toBase64 |> sprintf "Basic %s"
            authorization (s context) context
        
        /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
        let cacheControl (control: string) (context:HeaderContext) =
            header "Cache-Control" control context
        
        /// What type of connection the user-agent would prefer
        let connection (connection: string) (context:HeaderContext) =
            header "Connection" connection context
        
        /// Describes the placement of the content. Valid dispositions are: inline, attachment, form-data
        let contentDisposition (placement: string) (name: string option) (fileName: string option) (context:HeaderContext) =
            let namePart = match name with Some n -> sprintf "; name=\"%s\"" n | None -> ""
            let fileNamePart = match fileName with Some n -> sprintf "; filename=\"%s\"" n | None -> ""
            header "Content-Disposition" (sprintf "%s%s%s" placement namePart fileNamePart) context
        
        /// The type of encoding used on the data
        let contentEncoding (encoding: string) (context:HeaderContext) =
            header "Content-Encoding" encoding context
        
        /// The language the content is in
        let contentLanguage (language: string) (context:HeaderContext) =
            header "Content-Language" language context
        
        /// An alternate location for the returned data
        let contentLocation (location: string) (context:HeaderContext) =
            header "Content-Location" location context
        
        /// A Base64-encoded binary MD5 sum of the content of the request body
        let contentMD5 (md5sum: string) (context:HeaderContext) =
            header "Content-MD5" md5sum context
        
        /// Where in a full body message this partial message belongs
        let contentRange (range: string) (context:HeaderContext) =
            header "Content-Range" range context

        // this is a property of the body.        
        // // /// The MIME type of the body of the request (used with POST and PUT requests)
        // // [<CustomOperation("ContentType")>]
        // // let ContentType (context: HeaderContext, contentType: string) =
        // //     this.header "Content-Type" contentType)
        /////// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        ////let ContentTypeWithEncoding (context: HeaderContext, contentType, charset:Encoding) =
        ////    this.header "Content-Type" sprintf "%s; charset=%s" contentType (charset.WebName))
        
        /// The date and time that the message was sent
        let date (date:DateTime) (context:HeaderContext) =
            header "Date" (date.ToString("R", CultureInfo.InvariantCulture)) context
        
        /// Indicates that particular server behaviors are required by the client
        let expect (behaviors: string) (context:HeaderContext) =
            header "Expect" behaviors context
        
        /// Gives the date/time after which the response is considered stale
        let expires (dateTime:DateTime) (context:HeaderContext) =
            header "Expires" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context
        
        /// The email address of the user making the request
        let from (email: string) (context:HeaderContext) =
            header "From" email context
        
        /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
        /// The port number may be omitted if the port is the standard port for the service requested.
        let host (host: string) (context:HeaderContext) =
            header "Host" host context
        
        /// Only perform the action if the client supplied entity matches the same entity on the server.
        /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
        let ifMatch (entity: string) (context:HeaderContext) =
            header "If-Match" entity context
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        let ifModifiedSince (dateTime:DateTime) (context:HeaderContext) =
            header "If-Modified-Since" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        let ifNoneMatch (etag: string) (context:HeaderContext) =
            header "If-None-Match" etag context
        
        /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
        let ifRange (range: string) (context:HeaderContext) =
            header "If-Range" range context
        
        /// Only send the response if the entity has not been modified since a specific time
        let ifUnmodifiedSince (dateTime:DateTime) (context:HeaderContext) =
            header "If-Unmodified-Since" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context
        
        /// Specifies a parameter used into order to maintain a persistent connection
        let keepAlive (keepAlive: string) (context:HeaderContext) =
            header "Keep-Alive" keepAlive context
        
        /// Specifies the date and time at which the accompanying body data was last modified
        let lastModified (dateTime:DateTime) (context:HeaderContext) =
            header "Last-Modified" (dateTime.ToString("R", CultureInfo.InvariantCulture)) context
        
        /// Limit the number of times the message can be forwarded through proxies or gateways
        let maxForwards (count:int) (context:HeaderContext) =
            header "Max-Forwards" (count.ToString()) context
        
        /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
        let origin (origin: string) (context:HeaderContext) =
            header "Origin" origin context
        
        /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
        let pragma (pragma: string) (context:HeaderContext) =
            header "Pragma" pragma context
        
        /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
        let prefer (prefer: string) (context:HeaderContext) =
            header "Prefer" prefer context
        
        /// Authorization credentials for connecting to a proxy.
        let proxyAuthorization (credentials: string) (context:HeaderContext) =
            header "Proxy-Authorization" credentials context
        
        /// Request only part of an entity. Bytes are numbered from 0
        let range (start:int64) (finish:int64) (context:HeaderContext) =
            header "Range" (sprintf "bytes=%d-%d" start finish) context
        
        /// This is the address of the previous web page from which a link to the currently requested page was followed. (The word "referrer" is misspelled in the RFC as well as in most implementations.)
        let referer (referer: string) (context:HeaderContext) =
            header "Referer" referer context
        
        /// The transfer encodings the user agent is willing to accept: the same values as for the response header
        /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
        /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
        let te (te: string) (context:HeaderContext) =
            header "TE" te context
        
        /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
        let trailer (trailer: string) (context:HeaderContext) =
            header "Trailer" trailer context
        
        /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.  The valid directives are one of: chunked, compress, deflate, gzip, or identity.
        let transferEncoding (directive: string) (context:HeaderContext) =
            header "Transfer-Encoding" directive context
        
        /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
        let translate (translate: string) (context:HeaderContext) =
            header "Translate" translate context
        
        /// Specifies additional communications protocols that the client supports.
        let upgrade (upgrade: string) (context:HeaderContext) =
            header "Upgrade" upgrade context
        
        /// The user agent string of the user agent
        let userAgent (userAgent: string) (context:HeaderContext) =
            header "User-Agent" userAgent context
        
        /// Informs the server of proxies through which the request was sent
        let via (server: string) (context:HeaderContext) =
            header "Via" server context
        
        /// A general warning about possible problems with the entity body
        let warning (message: string) (context:HeaderContext) =
            header "Warning" message context
        
        /// Override HTTP method.
        let xhttpMethodOverride (httpMethod: string) (context:HeaderContext) =
            header "X-HTTP-Method-Override" httpMethod context

    [<AutoOpen>]
    module Body =

        let body (headerContext: HeaderContext) : BodyContext = {
            request = headerContext.request;
            content = { content=""; contentType=""; headers=[] }
        }

        let private getContentTypeOrDefault (defaultValue:string) (context:BodyContext) =
            if String.IsNullOrEmpty(context.content.contentType) then
                defaultValue
            else 
                context.content.contentType

        // TODO: Binary
        // TODO: Base64
        
        // TODO
        // // [<CustomOperation("binary")>]
        // // let Binary(context: BodyContext, data: byte[]) =
        // //     let content = context.content
        // //     let contentType =
        // //         if context.content.contentType = null then
        // //             "text/plain" 
        // //         else 
        // //             context.content.contentType
        // //     { context with
        // //         content = { content with content=text; contentType=contentType;  }
        // //     }
        
        let text (text: string) (context: BodyContext) =
            let content = context.content
            let contentType = getContentTypeOrDefault "text/plain" context
            { context with
                content = { content with content=text; contentType=contentType;  }
            }

        let json (json: string) (context: BodyContext) =
            let content = context.content
            let contentType = getContentTypeOrDefault "application/json" context
            { context with
                content = { content with content=json; contentType=contentType;  }
            }

        let formUrlEncoded (data: (string*string) list) (context: BodyContext) =
            let content = context.content
            let contentType = getContentTypeOrDefault "application/x-www-form-urlencoded" context
            let contentString = String.Join("&", data |> List.map (fun (key,value) -> key + "=" + value))
            { context with
                content = { content with content=contentString; contentType=contentType;  }
            }

        /// The MIME type of the body of the request (used with POST and PUT requests)
        let contentType (contentType: string) (context: BodyContext) =
            let content = context.content
            { context with
                content = { content with contentType=contentType;  }
            }

        /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        let contentTypeWithEncoding (contentTypeString) (charset:Encoding) (context: BodyContext) =
            contentType (sprintf "%s; charset=%s" contentTypeString (charset.WebName)) context

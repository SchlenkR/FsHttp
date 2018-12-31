
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

    module Requests =
        
        let request (context: StartingContext) (method: HttpMethod) (url: string) =

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

        // RFC 2626 specifies 8 methods
        
        let get StartingContext (url:string) =
            request StartingContext HttpMethod.Get url
        
        let put StartingContext (url:string) =
            request StartingContext HttpMethod.Put url
        
        let post StartingContext (url:string) =
            request StartingContext HttpMethod.Post url
        
        let delete StartingContext (url:string) =
            request StartingContext HttpMethod.Delete url
        
        let options StartingContext (url:string) =
            request StartingContext HttpMethod.Options url
        
        let head StartingContext (url:string) =
            request StartingContext HttpMethod.Head url
        
        let trace StartingContext (url:string) =
            request StartingContext HttpMethod.Trace url

        // TODO: Connect
        // [<CustomOperation("CONNECT")>]
        // let Post StartingContext (url:string) =
        //     this.CreateRequest StartingContext HttpMethod.Connect url

        // RFC 4918 (WebDAV) adds 7 methods
        // TODO

    module Headers =

        let inline header (context: ^t) name value =
            (^t: (static member Header: ^t * string * string -> ^t) (context,name,value))

        /// Content-Types that are acceptable for the response
        let accept (context:HeaderContext) (contentType:string) =
            header context "Accept" contentType

        /// Character sets that are acceptable
        let acceptCharset (context:HeaderContext) (characterSets:string) =
            header context "Accept-Charset" characterSets

        /// Acceptable version in time
        let acceptDatetime (context:HeaderContext) (dateTime:DateTime) =
            header context "Accept-Datetime" (dateTime.ToString("R", CultureInfo.InvariantCulture))
        
        /// List of acceptable encodings. See HTTP compression.
        let acceptEncoding (context:HeaderContext) (encoding:string) =
            header context "Accept-Encoding" encoding
        
        /// List of acceptable human languages for response
        let acceptLanguage (context:HeaderContext) (language:string) =
            header context "Accept-Language" language
        
        /// The Allow header, which specifies the set of HTTP methods supported.
        let allow (context:HeaderContext) (methods: string) =
            header context "Allow" methods
        
        /// Authentication credentials for HTTP authentication
        let authorization (context: HeaderContext) (credentials: string) =
            header context "Authorization" credentials
        
        /// Authentication header using Basic Auth encoding
        let basicAuth (context: HeaderContext) (username: string) (password: string) =
            let s = sprintf "%s:%s" username password |> Helper.toBase64 |> sprintf "Basic %s"
            authorization context s
        
        /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
        let cacheControl (context: HeaderContext) (control: string) =
            header context "Cache-Control" control
        
        /// What type of connection the user-agent would prefer
        let connection (context: HeaderContext) (connection: string) =
            header context "Connection" connection
        
        /// Describes the placement of the content. Valid dispositions are: inline, attachment, form-data
        let contentDisposition (context: HeaderContext) (placement: string) (name: string option) (fileName: string option) =
            let namePart = match name with Some n -> sprintf "; name=\"%s\"" n | None -> ""
            let fileNamePart = match fileName with Some n -> sprintf "; filename=\"%s\"" n | None -> ""
            header context "Content-Disposition" (sprintf "%s%s%s" placement namePart fileNamePart)
        
        /// The type of encoding used on the data
        let contentEncoding (context: HeaderContext) (encoding: string) =
            header context "Content-Encoding" encoding
        
        /// The language the content is in
        let contentLanguage (context: HeaderContext) (language: string) =
            header context "Content-Language" language
        
        /// An alternate location for the returned data
        let contentLocation (context: HeaderContext) (location: string) =
            header context "Content-Location" location
        
        /// A Base64-encoded binary MD5 sum of the content of the request body
        let contentMD5 (context: HeaderContext) (md5sum: string) =
            header context "Content-MD5" md5sum
        
        /// Where in a full body message this partial message belongs
        let contentRange (context: HeaderContext) (range: string) =
            header context "Content-Range" range

        // this is a property of the body.        
        // // /// The MIME type of the body of the request (used with POST and PUT requests)
        // // [<CustomOperation("ContentType")>]
        // // let ContentType (context: HeaderContext, contentType: string) =
        // //     this.header context "Content-Type" contentType)
        /////// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        ////let ContentTypeWithEncoding (context: HeaderContext, contentType, charset:Encoding) =
        ////    this.header context "Content-Type" sprintf "%s; charset=%s" contentType (charset.WebName))
        
        /// The date and time that the message was sent
        let date (context: HeaderContext) (date:DateTime) =
            header context "Date" (date.ToString("R", CultureInfo.InvariantCulture))
        
        /// Indicates that particular server behaviors are required by the client
        let expect (context: HeaderContext) (behaviors: string) =
            header context "Expect" behaviors
        
        /// Gives the date/time after which the response is considered stale
        let expires (context: HeaderContext) (dateTime:DateTime) =
            header context "Expires" (dateTime.ToString("R", CultureInfo.InvariantCulture))
        
        /// The email address of the user making the request
        let from (context: HeaderContext) (email: string) =
            header context "From" email
        
        /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
        /// The port number may be omitted if the port is the standard port for the service requested.
        let host (context: HeaderContext) (host: string) =
            header context "Host" host
        
        /// Only perform the action if the client supplied entity matches the same entity on the server.
        /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
        let ifMatch (context: HeaderContext) (entity: string) =
            header context "If-Match" entity
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        let ifModifiedSince (context: HeaderContext) (dateTime:DateTime) =
            header context "If-Modified-Since" (dateTime.ToString("R", CultureInfo.InvariantCulture))
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        let ifNoneMatch (context: HeaderContext) (etag: string) =
            header context "If-None-Match" etag
        
        /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
        let ifRange (context: HeaderContext) (range: string) =
            header context "If-Range" range
        
        /// Only send the response if the entity has not been modified since a specific time
        let ifUnmodifiedSince (context: HeaderContext) (dateTime:DateTime) =
            header context "If-Unmodified-Since" (dateTime.ToString("R", CultureInfo.InvariantCulture))
        
        /// Specifies a parameter used into order to maintain a persistent connection
        let keepAlive (context: HeaderContext) (keepAlive: string) =
            header context "Keep-Alive" keepAlive
        
        /// Specifies the date and time at which the accompanying body data was last modified
        let lastModified (context: HeaderContext) (dateTime:DateTime) =
            header context "Last-Modified" (dateTime.ToString("R", CultureInfo.InvariantCulture))
        
        /// Limit the number of times the message can be forwarded through proxies or gateways
        let maxForwards (context: HeaderContext) (count:int) =
            header context "Max-Forwards" (count.ToString())
        
        /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
        let origin (context: HeaderContext) (origin: string) =
            header context "Origin" origin
        
        /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
        let pragma (context: HeaderContext) (pragma: string) =
            header context "Pragma" pragma
        
        /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
        let prefer (context: HeaderContext) (prefer: string) =
            header context "Prefer" prefer
        
        /// Authorization credentials for connecting to a proxy.
        let proxyAuthorization (context: HeaderContext) (credentials: string) =
            header context "Proxy-Authorization" credentials
        
        /// Request only part of an entity. Bytes are numbered from 0
        let range (context: HeaderContext) (start:int64) (finish:int64) =
            header context "Range" (sprintf "bytes=%d-%d" start finish)
        
        /// This is the address of the previous web page from which a link to the currently requested page was followed. (The word "referrer" is misspelled in the RFC as well as in most implementations.)
        let referer (context: HeaderContext) (referer: string) =
            header context "Referer" referer
        
        /// The transfer encodings the user agent is willing to accept: the same values as for the response header
        /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
        /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
        let te (context: HeaderContext) (te: string) =
            header context "TE" te
        
        /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
        let trailer (context: HeaderContext) (trailer: string) =
            header context "Trailer" trailer
        
        /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.  The valid directives are one of: chunked, compress, deflate, gzip, or identity.
        let transferEncoding (context: HeaderContext) (directive: string) =
            header context "Transfer-Encoding" directive
        
        /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
        let translate (context: HeaderContext) (translate: string) =
            header context "Translate" translate
        
        /// Specifies additional communications protocols that the client supports.
        let upgrade (context: HeaderContext) (upgrade: string) =
            header context "Upgrade" upgrade
        
        /// The user agent string of the user agent
        let userAgent (context: HeaderContext) (userAgent: string) =
            header context "User-Agent" userAgent
        
        /// Informs the server of proxies through which the request was sent
        let via (context: HeaderContext) (server: string) =
            header context "Via" server
        
        /// A general warning about possible problems with the entity body
        let warning (context: HeaderContext) (message: string) =
            header context "Warning" message
        
        /// Override HTTP method.
        let xhttpMethodOverride (context: HeaderContext) (httpMethod: string) =
            header context "X-HTTP-Method-Override" httpMethod

    module Body =

        let private getContentTypeOrDefault (context:BodyContext) (defaultValue:string) =
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
        
        let text (context: BodyContext) (text: string) =
            let content = context.content
            let contentType = getContentTypeOrDefault context "text/plain"
            { context with
                content = { content with content=text; contentType=contentType;  }
            }

        let json (context: BodyContext) (json: string) =
            let content = context.content
            let contentType = getContentTypeOrDefault context "application/json"
            { context with
                content = { content with content=json; contentType=contentType;  }
            }

        let formUrlEncoded (context: BodyContext) (data: (string*string) list) =
            let content = context.content
            let contentType = getContentTypeOrDefault context "application/x-www-form-urlencoded"
            let contentString = String.Join("&", data |> List.map (fun (key,value) -> key + "=" + value))
            { context with
                content = { content with content=contentString; contentType=contentType;  }
            }

        /// The MIME type of the body of the request (used with POST and PUT requests)
        let contentType (context: BodyContext) (contentType: string) =
            let content = context.content
            { context with
                content = { content with contentType=contentType;  }
            }

        /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        let contentTypeWithEncoding (context: BodyContext) (contentTypeString) (charset:Encoding) =
            contentType context (sprintf "%s; charset=%s" contentTypeString (charset.WebName))

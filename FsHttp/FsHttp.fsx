#if INTERACTIVE

#r "netstandard"
#r "System.Net.Http"

#else

module FsHttp

#endif

open System
open System.Linq
open System.Net.Http
open System.Text
open System.Globalization


[<AutoOpen>]
module Helper =

    let urlEncode (s: string) = System.Web.HttpUtility.UrlEncode(s)

    let toBase64 (s: string) =
        let utf8Bytes = Encoding.UTF8.GetBytes(s)
        Convert.ToBase64String(utf8Bytes)

    let fromBase64 (s: string) =
        let base64Bytes = Convert.FromBase64String(s)
        Encoding.UTF8.GetString(base64Bytes)

    let formatUrl (url: string) =
        let segments =
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
        segments


type Header = {
    url: string;
    method: HttpMethod;
    headers: (string*string) list;
}

type Content = {
    content: string;
    contentType: string;
    headers: (string*string) list;
} 


type StartingContext = StartingContext

type FinalContext = {
    request: Header;
    content: Content option;
}
with
    member this.invoke () =
        let request = this.request
        let requestMessage = new HttpRequestMessage(request.method, request.url)
        
        requestMessage.Content <-
            match this.content with
            | Some c -> 
                let stringContent = new StringContent(c.content, System.Text.Encoding.UTF8, c.contentType)
                for name,value in c.headers do
                    stringContent.Headers.TryAddWithoutValidation(name, value) |> ignore
                stringContent
            | _ -> null
        
        for name,value in request.headers do
            requestMessage.Headers.TryAddWithoutValidation(name, value) |> ignore

        // TODO: dispose        
        let client = new HttpClient()
        client.SendAsync(requestMessage)

type HeaderContext = { request: Header }
with
    static member header (this:HeaderContext, name:string, value: string) =
        { this with request = { this.request with headers = this.request.headers @ [name,value] } }
    static member finalize (this:HeaderContext) =
        let finalContext = { request=this.request; content=None }
        finalContext

type BodyContext = {
    request: Header;
    content: Content;
}
with
    static member header (this:BodyContext, name:string, value: string) =
        { this with request = { this.request with headers = this.request.headers @ [name,value] } }
    static member finalize (this:BodyContext) =
        let finalContext:FinalContext = { request=this.request; content=Some this.content }
        finalContext


module Builder =
    type HttpBuilder() =
        member this.Bind(m, f) = f m
        member this.Return(x) = x
        member this.Yield(x) = StartingContext
        member this.For(m, f) = this.Bind m f

    // Request methods
    type HttpBuilder with

        [<CustomOperation("Request")>]
        member this.Method (context:StartingContext) (method:HttpMethod) (url: string) =
            let formattedUrl = formatUrl url
            {
                request = { url=formattedUrl; method=method; headers=[] }
            }

        // RFC 2626 specifies 8 methods
        [<CustomOperation("GET")>]
        member this.Get(StartingContext, url: string) =
            this.Method StartingContext HttpMethod.Get url
        [<CustomOperation("PUT")>]
        member this.Put(StartingContext, url: string) =
            this.Method StartingContext HttpMethod.Put url
        [<CustomOperation("POST")>]
        member this.Post(StartingContext, url: string) =
            this.Method StartingContext HttpMethod.Post url
        [<CustomOperation("DELETE")>]
        member this.Delete(StartingContext, url: string) =
            this.Method StartingContext HttpMethod.Delete url
        [<CustomOperation("OPTIONS")>]
        member this.Options(StartingContext, url: string) =
            this.Method StartingContext HttpMethod.Options url
        [<CustomOperation("HEAD")>]
        member this.Head(StartingContext, url: string) =
            this.Method StartingContext HttpMethod.Head url
        [<CustomOperation("TRACE")>]
        member this.Trace(StartingContext, url: string) =
            this.Method StartingContext HttpMethod.Trace url
        // TODO: Connect
        // [<CustomOperation("CONNECT")>]
        // member this.Post(StartingContext, url: string) =
        //     this.CreateRequest StartingContext HttpMethod.Connect url

        // RFC 4918 (WebDAV) adds 7 methods
        // TODO

    // Header + Body
    type HttpBuilder with

        [<CustomOperation("header")>]
        member inline this.Header(context: ^t, name, value) =
            (^t: (static member header: ^t * string * string -> ^t) (context,name,value))

    // HTTP request headers
    type HttpBuilder with

        /// Content-Types that are acceptable for the response
        [<CustomOperation("Accept")>]
        member this.Accept (context: HeaderContext, contentType: string) =
            this.Header(context, "Accept", contentType)

        /// Character sets that are acceptable
        [<CustomOperation("AcceptCharset")>]
        member this.AcceptCharset (context: HeaderContext, characterSets: string) =
            this.Header(context, "Accept-Charset", characterSets)

        /// Acceptable version in time
        [<CustomOperation("AcceptDatetime")>]
        member this.AcceptDatetime (context: HeaderContext, dateTime: DateTime) =
            this.Header(context, "Accept-Datetime", dateTime.ToString("R", CultureInfo.InvariantCulture))
        
        /// List of acceptable encodings. See HTTP compression.
        [<CustomOperation("AcceptEncoding")>]
        member this.AcceptEncoding (context: HeaderContext, encoding: string) =
            this.Header(context, "Accept-Encoding", encoding)
        
        /// List of acceptable human languages for response
        [<CustomOperation("AcceptLanguage")>]
        member this.AcceptLanguage (context: HeaderContext, language: string) =
            this.Header(context, "Accept-Language", language)
        
        /// The Allow header, which specifies the set of HTTP methods supported.
        [<CustomOperation("Allow")>]
        member this.Allow (context: HeaderContext, methods: string) =
            this.Header(context, "Allow", methods)
        
        /// Authentication credentials for HTTP authentication
        [<CustomOperation("Authorization")>]
        member this.Authorization (context: HeaderContext, credentials: string) =
            this.Header(context, "Authorization", credentials)
        
        /// Authentication header using Basic Auth encoding
        [<CustomOperation("BasicAuth")>]
        member this.BasicAuth (context: HeaderContext, username: string, password: string) =
            let s = sprintf "%s:%s" username password |> toBase64 |> sprintf "Basic %s"
            this.Authorization(context, s)
        
        /// Used to specify directives that MUST be obeyed by all caching mechanisms along the request/response chain
        [<CustomOperation("CacheControl")>]
        member this.CacheControl (context: HeaderContext, control: string) =
            this.Header(context, "Cache-Control", control)
        
        /// What type of connection the user-agent would prefer
        [<CustomOperation("Connection")>]
        member this.Connection (context: HeaderContext, connection: string) =
            this.Header(context, "Connection", connection)
        
        /// Describes the placement of the content. Valid dispositions are: inline, attachment, form-data
        [<CustomOperation("ContentDisposition")>]
        member this.ContentDisposition (context: HeaderContext, placement: string, name: string option, fileName: string option) =
            let namePart = match name with Some n -> sprintf "; name=\"%s\"" n | None -> ""
            let fileNamePart = match fileName with Some n -> sprintf "; filename=\"%s\"" n | None -> ""
            this.Header(context, "Content-Disposition", sprintf "%s%s%s" placement namePart fileNamePart)
        
        /// The type of encoding used on the data
        [<CustomOperation("ContentEncoding")>]
        member this.ContentEncoding (context: HeaderContext, encoding: string) =
            this.Header(context, "Content-Encoding", encoding)
        
        /// The language the content is in
        [<CustomOperation("ContentLanguage")>]
        member this.ContentLanguage (context: HeaderContext, language: string) =
            this.Header(context, "Content-Language", language)
        
        /// An alternate location for the returned data
        [<CustomOperation("ContentLocation")>]
        member this.ContentLocation (context: HeaderContext, location: string) =
            this.Header(context, "Content-Location", location)
        
        /// A Base64-encoded binary MD5 sum of the content of the request body
        [<CustomOperation("ContentMD5")>]
        member this.ContentMD5 (context: HeaderContext, md5sum: string) =
            this.Header(context, "Content-MD5", md5sum)
        
        /// Where in a full body message this partial message belongs
        [<CustomOperation("ContentRange")>]
        member this.ContentRange (context: HeaderContext, range: string) =
            this.Header(context, "Content-Range", range)
        
        /// The MIME type of the body of the request (used with POST and PUT requests)
        [<CustomOperation("ContentType")>]
        member this.ContentType (context: HeaderContext, contentType: string) =
            this.Header(context, "Content-Type", contentType)
        
        /// The MIME type of the body of the request (used with POST and PUT requests) with an explicit encoding
        [<CustomOperation("ContentTypeWithEncoding")>]
        member this.ContentTypeWithEncoding (context: HeaderContext, contentType, charset:Encoding) =
            this.Header(context, "Content-Type", sprintf "%s; charset=%s" contentType (charset.WebName))
        
        /// The date and time that the message was sent
        [<CustomOperation("Date")>]
        member this.Date (context: HeaderContext, date:DateTime) =
            this.Header(context, "Date", date.ToString("R", CultureInfo.InvariantCulture))
        
        /// Indicates that particular server behaviors are required by the client
        [<CustomOperation("Expect")>]
        member this.Expect (context: HeaderContext, behaviors: string) =
            this.Header(context, "Expect", behaviors)
        
        /// Gives the date/time after which the response is considered stale
        [<CustomOperation("Expires")>]
        member this.Expires (context: HeaderContext, dateTime:DateTime) =
            this.Header(context, "Expires", dateTime.ToString("R", CultureInfo.InvariantCulture))
        
        /// The email address of the user making the request
        [<CustomOperation("From")>]
        member this.From (context: HeaderContext, email: string) =
            this.Header(context, "From", email)
        
        /// The domain name of the server (for virtual hosting), and the TCP port number on which the server is listening.
        /// The port number may be omitted if the port is the standard port for the service requested.
        [<CustomOperation("Host")>]
        member this.Host (context: HeaderContext, host: string) =
            this.Header(context, "Host", host)
        
        /// Only perform the action if the client supplied entity matches the same entity on the server.
        /// This is mainly for methods like PUT to only update a resource if it has not been modified since the user last updated it. If-Match: "737060cd8c284d8af7ad3082f209582d" Permanent
        [<CustomOperation("IfMatch")>]
        member this.IfMatch (context: HeaderContext, entity: string) =
            this.Header(context, "If-Match", entity)
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        [<CustomOperation("IfModifiedSince")>]
        member this.IfModifiedSince (context: HeaderContext, dateTime:DateTime) =
            this.Header(context, "If-Modified-Since", dateTime.ToString("R", CultureInfo.InvariantCulture))
        
        /// Allows a 304 Not Modified to be returned if content is unchanged
        [<CustomOperation("IfNoneMatch")>]
        member this.IfNoneMatch (context: HeaderContext, etag: string) =
            this.Header(context, "If-None-Match", etag)
        
        /// If the entity is unchanged, send me the part(s) that I am missing; otherwise, send me the entire new entity
        [<CustomOperation("IfRange")>]
        member this.IfRange (context: HeaderContext, range: string) =
            this.Header(context, "If-Range", range)
        
        /// Only send the response if the entity has not been modified since a specific time
        [<CustomOperation("IfUnmodifiedSince")>]
        member this.IfUnmodifiedSince (context: HeaderContext, dateTime:DateTime) =
            this.Header(context, "If-Unmodified-Since", dateTime.ToString("R", CultureInfo.InvariantCulture))
        
        /// Specifies a parameter used into order to maintain a persistent connection
        [<CustomOperation("KeepAlive")>]
        member this.KeepAlive (context: HeaderContext, keepAlive: string) =
            this.Header(context, "Keep-Alive", keepAlive)
        
        /// Specifies the date and time at which the accompanying body data was last modified
        [<CustomOperation("LastModified")>]
        member this.LastModified (context: HeaderContext, dateTime:DateTime) =
            this.Header(context, "Last-Modified", dateTime.ToString("R", CultureInfo.InvariantCulture))
        
        /// Limit the number of times the message can be forwarded through proxies or gateways
        [<CustomOperation("MaxForwards")>]
        member this.MaxForwards (context: HeaderContext, count:int) =
            this.Header(context, "Max-Forwards", count.ToString())
        
        /// Initiates a request for cross-origin resource sharing (asks server for an 'Access-Control-Allow-Origin' response header)
        [<CustomOperation("Origin")>]
        member this.Origin (context: HeaderContext, origin: string) =
            this.Header(context, "Origin", origin)
        
        /// Implementation-specific headers that may have various effects anywhere along the request-response chain.
        [<CustomOperation("Pragma")>]
        member this.Pragma (context: HeaderContext, pragma: string) =
            this.Header(context, "Pragma", pragma)
        
        /// Optional instructions to the server to control request processing. See RFC https://tools.ietf.org/html/rfc7240 for more details
        [<CustomOperation("Prefer")>]
        member this.Prefer (context: HeaderContext, prefer: string) =
            this.Header(context, "Prefer", prefer)
        
        /// Authorization credentials for connecting to a proxy.
        [<CustomOperation("ProxyAuthorization")>]
        member this.ProxyAuthorization (context: HeaderContext, credentials: string) =
            this.Header(context, "Proxy-Authorization", credentials)
        
        /// Request only part of an entity. Bytes are numbered from 0
        [<CustomOperation("Range")>]
        member this.Range (context: HeaderContext, start:int64, finish:int64) =
            this.Header(context, "Range", sprintf "bytes=%d-%d" start finish)
        
        /// This is the address of the previous web page from which a link to the currently requested page was followed. (The word "referrer" is misspelled in the RFC as well as in most implementations.)
        [<CustomOperation("Referer")>]
        member this.Referer (context: HeaderContext, referer: string) =
            this.Header(context, "Referer", referer)
        
        /// The transfer encodings the user agent is willing to accept: the same values as for the response header
        /// Transfer-Encoding can be used, plus the "trailers" value (related to the "chunked" transfer method) to
        /// notify the server it expects to receive additional headers (the trailers) after the last, zero-sized, chunk.
        [<CustomOperation("TE")>]
        member this.TE (context: HeaderContext, te: string) =
            this.Header(context, "TE", te)
        
        /// The Trailer general field value indicates that the given set of header fields is present in the trailer of a message encoded with chunked transfer-coding
        [<CustomOperation("Trailer")>]
        member this.Trailer (context: HeaderContext, trailer: string) =
            this.Header(context, "Trailer", trailer)
        
        /// The TransferEncoding header indicates the form of encoding used to safely transfer the entity to the user.  The valid directives are one of: chunked, compress, deflate, gzip, or identity.
        [<CustomOperation("TransferEncoding")>]
        member this.TransferEncoding (context: HeaderContext, directive: string) =
            this.Header(context, "Transfer-Encoding", directive)
        
        /// Microsoft extension to the HTTP specification used in conjunction with WebDAV functionality.
        [<CustomOperation("Translate")>]
        member this.Translate (context: HeaderContext, translate: string) =
            this.Header(context, "Translate", translate)
        
        /// Specifies additional communications protocols that the client supports.
        [<CustomOperation("Upgrade")>]
        member this.Upgrade (context: HeaderContext, upgrade: string) =
            this.Header(context, "Upgrade", upgrade)
        
        /// The user agent string of the user agent
        [<CustomOperation("UserAgent")>]
        member this.UserAgent (context: HeaderContext, userAgent: string) =
            this.Header(context, "User-Agent", userAgent)
        
        /// Informs the server of proxies through which the request was sent
        [<CustomOperation("Via")>]
        member this.Via (context: HeaderContext, server: string) =
            this.Header(context, "Via", server)
        
        /// A general warning about possible problems with the entity body
        [<CustomOperation("Warning")>]
        member this.Warning (context: HeaderContext, message: string) =
            this.Header(context, "Warning", message)
        
        /// Override HTTP method.
        [<CustomOperation("XHTTPMethodOverride")>]
        member this.XHTTPMethodOverride (context: HeaderContext, httpMethod: string) =
            this.Header(context, "X-HTTP-Method-Override", httpMethod)

    // Body
    type HttpBuilder with

        [<CustomOperation("body")>]
        member this.Body(context: HeaderContext) : BodyContext =
            {
                request = context.request;
                content = { content=""; contentType=""; headers=[] }
            }
        
        [<CustomOperation("text")>]
        member this.Text(context: BodyContext, text: string) =
            let content = context.content
            { context with
                content = { content with content=text; contentType="text/plain";  }
            }
        
        [<CustomOperation("json")>]
        member this.Json(context: BodyContext, json: string) =
            let content = context.content
            { context with
                content = { content with content=json; contentType="application/json";  }
            }

let http = Builder.HttpBuilder()


type PrintHint =
    | Header 
    | Preview of maxLength: int
    | Expand

type Response = {
    content: HttpContent;
    headers: Headers.HttpResponseHeaders;
    reasonPhrase: string;
    statusCode: System.Net.HttpStatusCode;
    requestMessage: HttpRequestMessage;
    version: Version;
    printHint: PrintHint
}


let inline sendAsync (context: ^t) =
    let finalContext = (^t: (static member finalize: ^t -> FinalContext) (context))
    async {
        let! response = finalContext.invoke() |> Async.AwaitTask
        return
            {
                content = response.Content;
                headers = response.Headers;
                reasonPhrase = response.ReasonPhrase;
                statusCode = response.StatusCode;
                requestMessage = response.RequestMessage;
                version = response.Version;
                printHint = Header
            }
    }
let inline send (context: ^t) = (sendAsync context) |> Async.RunSynchronously

let headerString (r: Response) =
    let sb = StringBuilder()
    let printHeader (headers: Headers.HttpHeaders) =
        // TODO: Table formatting
        for h in headers do
            let values = String.Join(", ", h.Value)
            sb.AppendLine (sprintf "%s: %s" h.Key values) |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine (sprintf "HTTP/%s %d %s" (r.version.ToString()) (int r.statusCode) (string r.statusCode)) |> ignore
    printHeader r.headers
    sb.AppendLine("---") |> ignore
    printHeader r.content.Headers
    sb.ToString()

let contentAsStringAsync (r: Response) =
    r.content.ReadAsStringAsync() |> Async.AwaitTask
let contentAsString (r: Response) =
    (contentAsStringAsync r) |> Async.RunSynchronously

let contentAsStringPreviewAsync maxLength (r: Response) =
    let getTrimChars (s: string) =
        match s.Length with
        | l when l > maxLength -> "\n..."
        | _ -> ""
    async {
        let! content = contentAsStringAsync r
        return
            System.String(content.Take(maxLength).ToArray())
            + (getTrimChars content)
    }
let contentAsStringPreview maxLength (r: Response) =
    (contentAsStringPreviewAsync maxLength r) |> Async.RunSynchronously

let headerOnly r = { r with printHint = Header }
let preview maxLength r = { r with printHint = Preview maxLength }
let expand r = { r with printHint = Expand }


// TODO: required FSharp.Data
// let toJson (r: FsHttp.Response) =  r |> FsHttp.contentAsString |> JsonValue.Parse


#if INTERACTIVE

fsi.AddPrinter
    (fun (r: Response) ->
        let content =
            match r.printHint with
            | Preview maxLength -> contentAsStringPreview maxLength r
            | Expand -> contentAsString r
            | Header -> contentAsStringPreview 500 r
        sprintf "%s\n%s" (headerString r) content
    )

#endif

// TODO:
// Multipart
// mime types
// content types
// body: text, binary, json, etc.
// setHeaders anschauen
// Manche Funktionen sind Synchron.
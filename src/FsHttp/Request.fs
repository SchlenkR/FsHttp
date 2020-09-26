
[<RequireQualifiedAccess>]
module FsHttp.Request

open System
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Threading

open Domain

[<Literal>]
let TimeoutPropertyName = "RequestTimeout"

/// Transforms a Request into a System.Net.Http.HttpRequestMessage.
let toMessage (request: Request): HttpRequestMessage =

    let header = request.header

    let requestMessage = new HttpRequestMessage(header.method, header.url)

    requestMessage.Properties.[TimeoutPropertyName] <- request.config.timeout

    let buildDotnetContent (part: ContentData) (contentType: string option) (name: string option) =
        let dotnetContent =
            match part with
            | StringContent s ->
                // TODO: Encoding is set hard to UTF8 - but the HTTP request has it's own encoding header.
                new StringContent(s) :> HttpContent
            | ByteArrayContent data -> new ByteArrayContent(data) :> HttpContent
            | StreamContent s -> new StreamContent(s) :> HttpContent
            | FormUrlEncodedContent data ->
                let kvps =
                    data |> List.map (fun (k, v) -> KeyValuePair<string, string>(k, v))

                new FormUrlEncodedContent(kvps) :> HttpContent
            | FileContent path ->
                let content =
                    let fs = System.IO.File.OpenRead path
                    new StreamContent(fs)

                let contentDispoHeaderValue =
                    ContentDispositionHeaderValue("form-data")

                match name with
                | Some v -> contentDispoHeaderValue.Name <- v
                | None -> ()

                contentDispoHeaderValue.FileName <- path
                content.Headers.ContentDisposition <- contentDispoHeaderValue

                content :> HttpContent

        if contentType.IsSome then dotnetContent.Headers.ContentType <- Headers.MediaTypeHeaderValue contentType.Value

        dotnetContent

    requestMessage.Content <-
        match request.content with
        | Empty -> null
        | Single bodyContent -> buildDotnetContent bodyContent.contentData bodyContent.contentType None
        | Multi multipartContent ->
            match multipartContent.contentData with
            | [] -> null
            | multi ->
                let multipartContent = new MultipartFormDataContent()
                do multi
                   |> List.map (fun x -> x.name, buildDotnetContent x.content x.contentType (Some x.name))
                   |> List.iter (fun (name, dotnetContent) -> multipartContent.Add(dotnetContent, name))
                multipartContent :> HttpContent

    for name, value in header.headers do
        requestMessage.Headers.TryAddWithoutValidation(name, value) |> ignore

    requestMessage

let httpClient =
    let timeoutHandler innerHandler =
        { new DelegatingHandler(InnerHandler = innerHandler) with
            member _.SendAsync(request: HttpRequestMessage, cancellationToken: CancellationToken) =
                let cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)

                cts.CancelAfter(request.Properties.[TimeoutPropertyName] :?> TimeSpan)
                base.SendAsync(request, cts.Token) }

    fun (config: Config) ->
        match config.httpClient with
        | Some client -> client
        | None ->
#if NETSTANDARD_2
            let handler = new HttpClientHandler()
#else
            let handler = new SocketsHttpHandler(UseCookies = false, PooledConnectionLifetime = TimeSpan.FromMinutes 5.0)
#endif
            match config.proxy with
            | Some proxy ->
                let webProxy = WebProxy(proxy.url)
                match proxy.credentials with
                | Some cred ->
                    webProxy.UseDefaultCredentials <- false
                    webProxy.Credentials <- cred
                | None -> webProxy.UseDefaultCredentials <- true
                handler.Proxy <- webProxy
            | None -> ()

            new HttpClient(timeoutHandler handler, Timeout = Timeout.InfiniteTimeSpan)

/// Sends a context asynchronously.
let sendAsync (context: IContext) =

    let request = context.ToRequest()

    let requestMessage = toMessage request

    let finalRequestMessage =
        match request.config.httpMessageTransformer with
        | None -> requestMessage
        | Some map -> map requestMessage

    let invoke (ctok: CancellationToken) =
        let client = httpClient request.config

        let cookies =
            request.header.cookies
            |> List.map string
            |> String.concat "; "

        finalRequestMessage.Headers.Add("Cookie", cookies)

        let finalClient =
            match request.config.httpClientTransformer with
            | None -> client
            | Some map -> map client

        finalClient.SendAsync(finalRequestMessage, ctok)

    async {
        let! ctok = Async.CancellationToken
        let! response = invoke ctok |> Async.AwaitTask
        return { requestContext = request
                 content = response.Content
                 headers = response.Headers
                 reasonPhrase = response.ReasonPhrase
                 statusCode = response.StatusCode
                 requestMessage = response.RequestMessage
                 version = response.Version
                 printHint = request.config.printHint }
    }

/// Sends a context synchronously.
let inline send context =
    context
    |> sendAsync
    |> Async.RunSynchronously

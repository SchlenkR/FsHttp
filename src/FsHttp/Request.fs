module FsHttp.Request

open System
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Threading

open FsHttp.Domain

[<Literal>]
let private TimeoutPropertyName = "RequestTimeout"

/// Transforms a Request into a System.Net.Http.HttpRequestMessage.
let toMessage (request: Request): HttpRequestMessage =

    let header = request.header

    let url = FsHttpUrl.toUriString header.url
    let requestMessage = new HttpRequestMessage(header.method, url)

    do requestMessage.Properties.[TimeoutPropertyName] <- request.config.timeout

    let buildDotnetContent (part: ContentData) (contentType: string option) (name: string option) =
        let dotnetContent =
            match part with
            | StringContent s ->
                // TODO: Encoding is set hard to UTF8 - but the HTTP request has it's own encoding header.
                new StringContent(s) :> HttpContent
            | ByteArrayContent data -> new ByteArrayContent(data) :> HttpContent
            | StreamContent s -> new StreamContent(s) :> HttpContent
            | FormUrlEncodedContent data ->
                new FormUrlEncodedContent(data) :> HttpContent
            | FileContent path ->
                let content =
                    let fs = System.IO.File.OpenRead path
                    new StreamContent(fs)
                let contentDispoHeaderValue =
                    ContentDispositionHeaderValue("form-data")
                match name with
                | Some v -> contentDispoHeaderValue.Name <- v
                | None -> ()
                do
                    contentDispoHeaderValue.FileName <- path
                    content.Headers.ContentDisposition <- contentDispoHeaderValue
                content :> HttpContent

        if contentType.IsSome then dotnetContent.Headers.ContentType <- Headers.MediaTypeHeaderValue contentType.Value

        dotnetContent

    do requestMessage.Content <-
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
    
    for kvp in header.headers do
        requestMessage.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value) |> ignore

    requestMessage

let private getHttpClient =
    let timeoutHandler innerHandler printDebugMessages =
        { new DelegatingHandler(InnerHandler = innerHandler) with
            member _.SendAsync(request: HttpRequestMessage, cancellationToken: CancellationToken) =
                let cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)

                cts.CancelAfter(request.Properties.[TimeoutPropertyName] :?> TimeSpan)
                base.SendAsync(request, cts.Token) }

    fun (config: Config) ->
        match config.httpClientFactory with
        | Some clientFactory -> clientFactory()
        | None ->
            let transformHandler = Option.defaultValue id config.httpClientHandlerTransformer
            let handler =
                transformHandler <|
#if NETSTANDARD_2
                    new HttpClientHandler()
#else
                    new SocketsHttpHandler(UseCookies = false, PooledConnectionLifetime = TimeSpan.FromMinutes 5.0)
#endif

            match config.certErrorStrategy with
            | Default -> ()
            | AlwaysAccept ->
#if NETSTANDARD_2
                handler.ServerCertificateCustomValidationCallback <- (fun msg cert chain errors -> true)
#else
                handler.SslOptions <-
                    let options = Security.SslClientAuthenticationOptions()
                    options.RemoteCertificateValidationCallback <-
                        Security.RemoteCertificateValidationCallback(fun sender cert chain errors -> true)
                    options
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

            new HttpClient(timeoutHandler handler config.printDebugMessages, Timeout = Timeout.InfiniteTimeSpan)

/// Builds an asynchronous request, without sending it.
let buildAsync (context: IContext) =
    async {
        let request = context.ToRequest()
        let requestMessage = toMessage request
        let finalRequestMessage = requestMessage |> Option.defaultValue id request.config.httpMessageTransformer
        let invoke (ctok: CancellationToken) =
            let client = getHttpClient request.config
            let cookies =
                request.header.cookies
                |> List.map string
                |> String.concat "; "
            finalRequestMessage.Headers.Add("Cookie", cookies)
            let finalClient = client |> Option.defaultValue id request.config.httpClientTransformer
            finalClient.SendAsync(finalRequestMessage, request.config.httpCompletionOption, ctok)
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

/// Sends a context asynchronously.
let sendAsync (context: IContext) =
    context
    |> buildAsync
    |> Async.StartChild
    |> Async.RunSynchronously

/// Sends a context synchronously.
let inline send context =
    context
    |> buildAsync
    |> Async.RunSynchronously

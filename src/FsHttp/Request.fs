module FsHttp.Request

open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Threading

open FsHttp

let private TimeoutPropertyName = "RequestTimeout"

let private setRequestMessageProp (requestMessage: HttpRequestMessage) (propName: string) (value: 'a) =
#if NETSTANDARD2_0 || NETSTANDARD2_1
    do requestMessage.Properties.[propName] <- value
#else
    do requestMessage.Options.Set(HttpRequestOptionsKey propName, value)
#endif

let private getRequestMessageProp<'a> (requestMessage: HttpRequestMessage) (propName: string) =
#if NETSTANDARD2_0 || NETSTANDARD2_1
    requestMessage.Properties.[propName] :?> 'a
#else
    match requestMessage.Options.TryGetValue<'a>(HttpRequestOptionsKey propName) with
    | true,value -> value
    | false,_ -> failwith $"HttpRequestOptionsKey '{propName}' not found."
#endif


/// Transforms a Request into a System.Net.Http.HttpRequestMessage.
let toRequestAndMessage (request: IToRequest): Request * HttpRequestMessage =
    let request = request.Transform()
    let header = request.header
    let requestMessage = new HttpRequestMessage(header.method, header.url.ToUriString())
    do setRequestMessageProp requestMessage TimeoutPropertyName request.config.timeout
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
        if contentType.IsSome then
            dotnetContent.Headers.ContentType <- MediaTypeHeaderValue contentType.Value
        dotnetContent
    let assignContentHeaders (target: HttpHeaders) (headers: Map<string, string>) =
        for kvp in headers do
            target.Add(kvp.Key, kvp.Value)
    let dotnetContent =
        match request.content with
        | Empty -> null
        | Single bodyContent -> 
            let dotnetBodyContent = buildDotnetContent bodyContent.contentData bodyContent.contentType None
            do assignContentHeaders dotnetBodyContent.Headers bodyContent.headers
            dotnetBodyContent
        | Multi multipartContent ->
            let dotnetMultipartContent =
                match multipartContent.contentData with
                | [] -> null
                | contentPart ->
                    let dotnetPart = new MultipartFormDataContent()
                    for x in contentPart do
                        let dotnetContent = buildDotnetContent x.content x.contentType (Some x.name)
                        dotnetPart.Add(dotnetContent, x.name)
                    dotnetPart :> HttpContent
            do assignContentHeaders dotnetMultipartContent.Headers multipartContent.headers
            dotnetMultipartContent
    do
        requestMessage.Content <- dotnetContent
        assignContentHeaders requestMessage.Headers header.headers
    request,requestMessage

let toRequest request = request |> toRequestAndMessage |> fst
let toHttpRequestMessage request = request |> toRequestAndMessage |> snd

let private getHttpClient =
    let timeoutHandler innerHandler =
        { new DelegatingHandler(InnerHandler = innerHandler) with
            member _.SendAsync(request: HttpRequestMessage, cancellationToken: CancellationToken) =
                let cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
                do cts.CancelAfter(getRequestMessageProp<TimeSpan> request TimeoutPropertyName)
                base.SendAsync(request, cts.Token)
        }

#if NETSTANDARD2_0 || NETSTANDARD2_1
    let getHandler ignoreSslIssues =
        let handler = new HttpClientHandler()
        if ignoreSslIssues then
            handler.ServerCertificateCustomValidationCallback <- (fun msg cert chain errors -> true)
        handler
#else
    let getHandler ignoreSslIssues =
        let handler = new SocketsHttpHandler(UseCookies = false, PooledConnectionLifetime = TimeSpan.FromMinutes 5.0)
        if ignoreSslIssues then
            handler.SslOptions <-
                let options = Security.SslClientAuthenticationOptions()
                let callback = Security.RemoteCertificateValidationCallback(fun sender cert chain errors -> true)
                do options.RemoteCertificateValidationCallback <- callback
                options
        handler
#endif

    fun (config: Config) ->
        match config.httpClientFactory with
        | Some clientFactory -> clientFactory()
        | None ->
            let transformHandler = Option.defaultValue id config.httpClientHandlerTransformer
            let ignoreSslIssues =
                match config.certErrorStrategy with
                | Default -> false
                | AlwaysAccept -> true
            let handler = getHandler ignoreSslIssues |> transformHandler
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

            new HttpClient(handler |> timeoutHandler, Timeout = Timeout.InfiniteTimeSpan)

/// Builds an asynchronous request, without sending it.
let toAsync (context: IToRequest) =
    async {
        let request,requestMessage = toRequestAndMessage context
        if request.config.printHint.printDebugMessages then
            printfn $"Sending request {request.header.method} {request.header.url.ToUriString()} ..."
        use finalRequestMessage = 
            let httpMessageTransformer = Option.defaultValue id request.config.httpMessageTransformer
            httpMessageTransformer requestMessage
        let! ctok = Async.CancellationToken
        let client = getHttpClient request.config
        let cookies =
            request.header.cookies
            |> List.map string
            |> String.concat "; "
        do finalRequestMessage.Headers.Add("Cookie", cookies)
        let finalClient = 
            let httpClientTransformer = Option.defaultValue id request.config.httpClientTransformer
            httpClientTransformer client
        let! response =
            finalClient.SendAsync(finalRequestMessage, request.config.httpCompletionOption, ctok)
            |> Async.AwaitTask
        if request.config.bufferResponseContent then
            // Task is started immediately, but must not be awaited when running in background.
            response.Content.LoadIntoBufferAsync() |> ignore
        if request.config.printHint.printDebugMessages then
            printfn $"{response.StatusCode |> int} ({response.StatusCode}) ({request.header.method} {request.header.url.ToUriString()})"
        let dispose () =
            do finalClient.Dispose()
            do response.Dispose()
            do requestMessage.Dispose()
        return { request = request
                 content = response.Content
                 headers = response.Headers
                 reasonPhrase = response.ReasonPhrase
                 statusCode = response.StatusCode
                 requestMessage = response.RequestMessage
                 version = response.Version
                 originalHttpRequestMessage = requestMessage
                 originalHttpResponseMessage = response
                 dispose = dispose }
    }

/// Sends a request asynchronously.
let sendTAsync (context: IToRequest) =
    context |> toAsync |> Async.StartAsTask

/// Sends a request asynchronously.
let sendAsync (context: IToRequest) =
    sendTAsync context |> Async.AwaitTask

/// Sends a request synchronously.
let inline send context =
    context |> toAsync |> Async.RunSynchronously

module FsHttp.Defaults

open System
open System.Net
open System.Net.Http

open FsHttp
open System.Text.Json

let defaultJsonDocumentOptions = JsonDocumentOptions()
let defaultJsonSerializerOptions = JsonSerializerOptions JsonSerializerDefaults.Web

let defaultHttpClientFactory (config: Config) =
    let getDefaultClientHandler ignoreSslIssues =
#if NETSTANDARD2_0 || NETSTANDARD2_1
        let handler = new HttpClientHandler()

        if ignoreSslIssues then
            handler.ServerCertificateCustomValidationCallback <- (fun msg cert chain errors -> true)

        handler
#else
        let handler =
            new SocketsHttpHandler(UseCookies = false, PooledConnectionLifetime = TimeSpan.FromMinutes 5.0)

        if ignoreSslIssues then
            handler.SslOptions <-
                let options = Security.SslClientAuthenticationOptions()

                let callback =
                    Security.RemoteCertificateValidationCallback(fun sender cert chain errors -> true)

                do options.RemoteCertificateValidationCallback <- callback
                options

        handler
#endif

    let ignoreSslIssues =
        match config.certErrorStrategy with
        | Default -> false
        | AlwaysAccept -> true

    let handler =
        let clientHandler = getDefaultClientHandler ignoreSslIssues

        clientHandler.AutomaticDecompression <-
            config.defaultDecompressionMethods
            |> List.fold (fun c n -> c ||| n) DecompressionMethods.None

        config.httpClientHandlerTransformers |> List.fold (fun c n -> n c) clientHandler

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

    let client = new HttpClient(handler)
    do config.timeout |> Option.iter (fun timeout -> client.Timeout <- timeout)
    client

let defaultHeadersAndBodyPrintMode = {
    format = true
    maxLength = Some 7000
}

let defaultDecompressionMethods = [
#if NETSTANDARD2_0 || NETSTANDARD2_1
    DecompressionMethods.Deflate
    DecompressionMethods.GZip
#else
    DecompressionMethods.All
#endif
]

let defaultConfig = {
    timeout = None
    defaultDecompressionMethods = defaultDecompressionMethods
    printHint = {
        requestPrintMode = HeadersAndBody(defaultHeadersAndBodyPrintMode)
        responsePrintMode = HeadersAndBody(defaultHeadersAndBodyPrintMode)
    }
    httpMessageTransformers = []
    httpClientHandlerTransformers = []
    httpClientTransformers = []
    httpClientFactory = defaultHttpClientFactory
    httpCompletionOption = HttpCompletionOption.ResponseHeadersRead
    proxy = None
    certErrorStrategy = Default
    bufferResponseContent = false
    cancellationToken = Threading.CancellationToken.None
}

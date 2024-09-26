module internal FsHttp.Defaults

open System
open System.Net
open System.Net.Http

open FsHttp
open System.Text.Json

let defaultJsonDocumentOptions = JsonDocumentOptions()
let defaultJsonSerializerOptions = JsonSerializerOptions JsonSerializerDefaults.Web

let defaultHttpClientFactory (config: Config) =
    let handler =
        new SocketsHttpHandler(
            UseCookies = false, 
            PooledConnectionLifetime = TimeSpan.FromMinutes 5.0)
    let ignoreSslIssues =
        match config.certErrorStrategy with
        | Default -> false
        | AlwaysAccept -> true
    if ignoreSslIssues then
        do handler.SslOptions <-
            let options = Security.SslClientAuthenticationOptions()
            let callback = Security.RemoteCertificateValidationCallback(fun sender cert chain errors -> true)
            do options.RemoteCertificateValidationCallback <- callback
            options
    do handler.AutomaticDecompression <-
        config.defaultDecompressionMethods
        |> List.fold (fun c n -> c ||| n) DecompressionMethods.None
    let handler = config.httpClientHandlerTransformers |> List.fold (fun c n -> n c) handler

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

let defaultDecompressionMethods = [ DecompressionMethods.All ]

let defaultConfig = 
    {
        timeout = None
        defaultDecompressionMethods = defaultDecompressionMethods
        headerTransformers = []
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

let defaultPrintHint = 
    {
        requestPrintMode = HeadersAndBody(defaultHeadersAndBodyPrintMode)
        responsePrintMode = HeadersAndBody(defaultHeadersAndBodyPrintMode)
    }

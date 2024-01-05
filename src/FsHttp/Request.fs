module FsHttp.Request

open System
open System.Net.Http
open System.Net.Http.Headers

open FsHttp
open FsHttp.Helper

// TODO: Remove this
let getAddressDefaults (request: Request) =
    let uri = request.header.target.ToUriStringWithDefault("")
    let method = request.header.target.method |> Option.defaultValue HttpMethod.Get
    uri, method

let addressToString (request: Request) =
    let uri, method = getAddressDefaults request
    $"{method} {uri}"

/// Transforms a Request into a System.Net.Http.HttpRequestMessage.
let toRequestAndMessage (request: IToRequest) : Request * HttpRequestMessage =
    let request = 
        let mutable request = request.ToRequest()
        for headerTransformer in request.config.headerTransformers do
            request <- { request with header = headerTransformer request.header }
        request

    // TODO: Try to encode URL / HTTP method presence or absence on type level.
    let uri, method = getAddressDefaults request

    let requestMessage = new HttpRequestMessage(method, uri)

    let buildDotnetContent
        (part: ContentData)
        (contentType: ContentType option)
        (name: string option)
        (fileName: string option)
        =
        let addDispoHeaderIfNeeded (content: HttpContent) =
            match request.content with
            | Multi _ ->
                let contentDispoHeaderValue = ContentDispositionHeaderValue("form-data")

                match name with
                | Some v -> contentDispoHeaderValue.Name <- v
                | None -> ()

                match fileName with
                | Some v -> contentDispoHeaderValue.FileName <- v
                | None -> ()

                content.Headers.ContentDisposition <- contentDispoHeaderValue
                ()
            | _ -> ()

        let dotnetContent =
            match part with
            | TextContent s ->
                // TODO: Encoding is set hard to UTF8 - but the HTTP request has it's own encoding header.
                let content = new StringContent(s) :> HttpContent
                addDispoHeaderIfNeeded content
                content
            | BinaryContent data ->
                let content = new ByteArrayContent(data) :> HttpContent
                addDispoHeaderIfNeeded content
                content
            | StreamContent s ->
                let content = new StreamContent(s) :> HttpContent
                addDispoHeaderIfNeeded content
                content
            | FormUrlEncodedContent data -> new FormUrlEncodedContent(data) :> HttpContent
            | FileContent path ->
                let content =
                    let fs = System.IO.File.OpenRead path
                    new StreamContent(fs)

                addDispoHeaderIfNeeded content
                content

        match contentType with
        | Some contentType -> do dotnetContent.Headers.ContentType <- contentType.ToMediaHeaderValue()
        | _ -> ()

        dotnetContent

    let assignContentHeaders (target: HttpHeaders) (headers: Map<string, string>) =
        for kvp in headers do
            target.Add(kvp.Key, kvp.Value)

    let dotnetContent =
        match request.content with
        | Empty -> null
        | Single bodyContent ->
            let dotnetBodyContent =
                buildDotnetContent
                    bodyContent.contentElement.contentData
                    bodyContent.contentElement.explicitContentType
                    None
                    None

            do assignContentHeaders dotnetBodyContent.Headers bodyContent.headers
            dotnetBodyContent
        | Multi multipartContent ->
            let dotnetMultipartContent =
                match multipartContent.partElements with
                | [] -> null
                | parts ->
                    let dotnetPart = new MultipartFormDataContent()

                    for part in parts do
                        let dotnetContent =
                            buildDotnetContent
                                part.content.contentData
                                part.content.explicitContentType
                                (Some part.name)
                                part.fileName

                        dotnetPart.Add(dotnetContent, part.name)

                    dotnetPart :> HttpContent

            do assignContentHeaders dotnetMultipartContent.Headers multipartContent.headers
            dotnetMultipartContent

    do
        requestMessage.Content <- dotnetContent
        assignContentHeaders requestMessage.Headers request.header.headers

    request, requestMessage

let toRequest request = request |> toRequestAndMessage |> fst
let toHttpRequestMessage request = request |> toRequestAndMessage |> snd

/// Builds an asynchronous request, without sending it.
let toAsync cancellationTokenOverride (context: IToRequest) =
    async {
        let request, requestMessage = toRequestAndMessage context
        do Fsi.logfn $"Sending request {addressToString request} ..."

        use finalRequestMessage =
            request.config.httpMessageTransformers
            |> List.fold (fun c n -> n c) requestMessage

        // cancellationTokenOverride: Because of C# interop (see Extensions)
        let ctok =
            match cancellationTokenOverride with
            | Some ctok -> ctok 
            | None -> request.config.cancellationToken
        let client = request.config.httpClientFactory request.config

        match request.header.cookies with
        | [] -> ()
        | cookies ->
            let cookies = cookies |> List.map string |> String.concat "; "
            do finalRequestMessage.Headers.Add("Cookie", cookies)

        let finalClient =
            request.config.httpClientTransformers |> List.fold (fun c n -> n c) client

        let! response =
            finalClient.SendAsync(finalRequestMessage, request.config.httpCompletionOption, ctok)
            |> Async.AwaitTask

        if request.config.bufferResponseContent then
            // Task is started immediately, but must not be awaited when running in background.
            response.Content.LoadIntoBufferAsync() |> ignore

        do Fsi.logfn $"{response.StatusCode |> int} ({response.StatusCode}) ({addressToString request})"

        let dispose () =
            do finalClient.Dispose()
            do response.Dispose()
            do requestMessage.Dispose()

        return {
            request = request
            content = response.Content
            headers = response.Headers
            reasonPhrase = response.ReasonPhrase
            statusCode = response.StatusCode
            requestMessage = response.RequestMessage
            version = response.Version
            printHint = request.printHint
            originalHttpRequestMessage = requestMessage
            originalHttpResponseMessage = response
            dispose = dispose
        }
    }

/// Sends a request asynchronously.
let sendTAsync (request: IToRequest) = request |> toAsync None |> Async.StartAsTask

/// Sends a request asynchronously.
let sendAsync (request: IToRequest) = request |> sendTAsync |> Async.AwaitTask

/// Sends a request synchronously.
let send request = request |> toAsync None |> Async.RunSynchronously

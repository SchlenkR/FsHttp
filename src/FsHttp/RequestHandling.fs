
[<AutoOpen>]
module FsHttp.RequestHandling

open System
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Text
open System.Threading

open Domain

/// Takes a context (HeaderContext or BodyContext) and transforms it into a
/// FinalContext that can be used for invocation.
let inline finalizeContext (context: ^t) =
    (^t: (static member Finalize: ^t -> FinalContext) (context))

/// Transforms a FinalContext into a System.Net.Http.HttpRequestMessage.
let toMessage (finalContext: FinalContext) : HttpRequestMessage =

    let request = finalContext.header

    let requestMessage = new HttpRequestMessage(request.method, request.url)

    let buildDotnetContent (part: ContentPart) =
        let dotnetContent =
            match part.contentData with
            | StringContent s ->
                // TODO: Encoding is set hard to UTF8 - but the HTTP request has it's own encoding header. 
                new StringContent(s, Encoding.UTF8) :> HttpContent
            | ByteArrayContent data ->
                new ByteArrayContent(data) :> HttpContent
            | StreamContent s ->
                new StreamContent(s) :> HttpContent
            | FormUrlEncodedContent data ->
                let kvps = data |> List.map (fun (k,v) -> KeyValuePair<string, string>(k, v))
                new FormUrlEncodedContent(kvps) :> HttpContent
            | FileContent path ->
                let content =
                    let fs = System.IO.File.OpenRead path
                    new StreamContent(fs)

                let contentDispoHeaderValue = Headers.ContentDispositionHeaderValue("form-data")
                if part.name.IsSome then
                    contentDispoHeaderValue.Name <- part.name.Value
                contentDispoHeaderValue.FileName  <- path
                content.Headers.ContentDisposition <- contentDispoHeaderValue

                content :> HttpContent

        dotnetContent.Headers.ContentType <- Headers.MediaTypeHeaderValue part.contentType

        dotnetContent

    requestMessage.Content <-
        match finalContext.content with
        | [] -> null
        | [single] -> buildDotnetContent single
        | multi ->
            let multipartContent = new MultipartFormDataContent()
            do
                multi
                |> List.map buildDotnetContent
                |> List.iter (fun x -> multipartContent.Add(x))
            multipartContent :> HttpContent

    for name,value in request.headers do
        requestMessage.Headers.TryAddWithoutValidation(name, value) |> ignore

    requestMessage

/// Sends a context asynchronously.
let inline sendAsync context =
    let finalContext = finalizeContext context
    let invoke(ctok : CancellationToken) =
        let requestMessage = toMessage finalContext

        let finalRequestMessage =
            match finalContext.config.httpMessageTransformer with
            | None -> requestMessage
            | Some map -> map requestMessage

        let cookieContainer = CookieContainer()
        
        finalContext.header.cookies
        |> List.iter (fun cookie ->
            let domain =
                if String.IsNullOrWhiteSpace cookie.Domain
                then finalContext.header.url
                else cookie.Domain
            cookieContainer.Add(Uri(domain), cookie))

        // TODO: dispose
        let clientHandler = new HttpClientHandler()
        clientHandler.CookieContainer <- cookieContainer
        let client = new HttpClient(clientHandler, Timeout = finalContext.config.timeout)

        let finalClient =
            match finalContext.config.httpClientTransformer with
            | None -> client
            | Some map -> map client
        finalClient.SendAsync(finalRequestMessage, ctok)

    async {
        let! ctok = Async.CancellationToken
        let! response = invoke ctok |> Async.AwaitTask
        return { 
            requestContext = finalContext
            content = response.Content
            headers = response.Headers
            reasonPhrase = response.ReasonPhrase
            statusCode = response.StatusCode
            requestMessage = response.RequestMessage
            version = response.Version
            printHint = finalContext.config.printHint
        }
    }

/// Sends a context synchronously.
let inline send context =
    context |> sendAsync |> Async.RunSynchronously


[<AutoOpen>]
module FsHttp.RequestHandling

open System
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Text
open System.Threading

open Domain

/// Takes a context (HeaderContext or BodyContext) and transforms it into a
/// FinalContext that can be used for invocation.
let inline finalizeContext (context: ^t) =
    (^t: (member Finalize: unit -> FinalContext) (context))

/// Transforms a FinalContext into a System.Net.Http.HttpRequestMessage.
let toMessage (finalContext: FinalContext) : HttpRequestMessage =

    let request = finalContext.header

    let requestMessage = new HttpRequestMessage(request.method, request.url)

    let buildDotnetContent (part: ContentData) (contentType: string option) (name: string option) =
        let dotnetContent =
            match part with
            | StringContent s ->
                // TODO: Encoding is set hard to UTF8 - but the HTTP request has it's own encoding header. 
                new StringContent(s) :> HttpContent
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

                let contentDispoHeaderValue = ContentDispositionHeaderValue("form-data")
                
                match name with
                | Some v ->  contentDispoHeaderValue.Name <- v
                | None -> ()
                
                contentDispoHeaderValue.FileName  <- path
                content.Headers.ContentDisposition <- contentDispoHeaderValue

                content :> HttpContent
        
        if contentType.IsSome then
            dotnetContent.Headers.ContentType <- Headers.MediaTypeHeaderValue contentType.Value

        dotnetContent

    requestMessage.Content <-
        match finalContext.content with
        | Empty -> null
        | Single bodyContent ->
            buildDotnetContent bodyContent.contentData (Some bodyContent.contentType) None
        | Multi multipartContent ->
            match multipartContent.contentData with
            | [] -> null
            | multi ->
                let multipartContent = new MultipartFormDataContent()
                do
                    multi
                    |> List.map (fun x -> x.name, buildDotnetContent x.content x.contentType (Some x.name))
                    |> List.iter (fun (name, dotnetContent) -> multipartContent.Add(dotnetContent, name))
                multipartContent :> HttpContent

    for name,value in request.headers do
        requestMessage.Headers.TryAddWithoutValidation(name, value) |> ignore

    requestMessage

/// Sends a context asynchronously.
let inline sendAsync context =
    
    let finalContext = finalizeContext context

    let requestMessage = toMessage finalContext

    let finalRequestMessage =
        match finalContext.config.httpMessageTransformer with
        | None -> requestMessage
        | Some map -> map requestMessage

    let invoke(ctok : CancellationToken) =

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
            requestMessage = requestMessage
            version = response.Version
            printHint = finalContext.config.printHint
        }
    }

/// Sends a context synchronously.
let inline send context =
    context |> sendAsync |> Async.RunSynchronously

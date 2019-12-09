
namespace FsHttp

open System
open System.Net
open System.Net.Http
open System.Text

[<AutoOpen>]
module RequestHandling =

    /// Takes a context (HeaderContext or BodyContext) and transforms it into a
    /// FinalContext that can be used for invocation.
    let inline finalizeContext (context: ^t) =
        (^t: (static member Finalize: ^t -> FinalContext) (context))

    /// Transforms a FinalContext into a System.Net.Http.HttpRequestMessage.
    let toMessage (finalContext: FinalContext) : HttpRequestMessage =
        let request = finalContext.header
        let requestMessage = new HttpRequestMessage(request.method, request.url)
    
        requestMessage.Content <-
            match finalContext.content with
            | Some c ->
                let dotnetContent =
                    match c.contentData with
                    | StringContent s ->
                        // TODO: Encoding is set hard to UTF8 - but the HTTP request has it's own encoding header. 
                        new System.Net.Http.StringContent(s, Encoding.UTF8, c.contentType) :> HttpContent
                    | ByteArrayContent data -> new System.Net.Http.ByteArrayContent(data) :> HttpContent
                    | StreamContent s -> new System.Net.Http.StreamContent(s) :> HttpContent
                    | FormUrlEncodedContent data ->
                        let kvps = data |> List.map (fun (k,v) -> System.Collections.Generic.KeyValuePair<string, string>(k, v))
                        new System.Net.Http.FormUrlEncodedContent(kvps) :> HttpContent
                for name,value in c.headers do
                    dotnetContent.Headers.TryAddWithoutValidation(name, value) |> ignore
                dotnetContent
            | _ -> null
    
        for name,value in request.headers do
            requestMessage.Headers.TryAddWithoutValidation(name, value) |> ignore

        requestMessage
 
    /// Sends a context asynchronously.
    let inline sendAsync context =
        let finalContext = finalizeContext context
        let invoke() =
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
            finalClient.SendAsync(finalRequestMessage)

        async {
            let! response = invoke() |> Async.AwaitTask
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

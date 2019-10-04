
namespace FsHttp

open System
open System.Net
open System.Net.Http
open System.Text
open System.Threading

[<AutoOpen>]
module RequestHandling =

    let inline finalizeContext (context: ^t) =
        (^t: (static member Finalize: ^t -> FinalContext) (context))

    let toMessage (finalContext: FinalContext) : HttpRequestMessage =
        let request = finalContext.header
        let requestMessage = new HttpRequestMessage(request.method, request.url)
    
        requestMessage.Content <-
            match finalContext.content with
            | Some c -> 
                let stringContent = new StringContent(c.content, Encoding.UTF8, c.contentType)
                for name,value in c.headers do
                    stringContent.Headers.TryAddWithoutValidation(name, value) |> ignore
                stringContent
            | _ -> null
    
        for name,value in request.headers do
            requestMessage.Headers.TryAddWithoutValidation(name, value) |> ignore

        requestMessage
    
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
                requestContext = finalContext;
                content = response.Content;
                headers = response.Headers;
                reasonPhrase = response.ReasonPhrase;
                statusCode = response.StatusCode;
                requestMessage = response.RequestMessage;
                version = response.Version;
                printHint = defaultPrintHint
            }
        }

    let inline send context =
        context |> sendAsync |> Async.RunSynchronously

    [<AutoOpen>]
    module Operators =

        /// synchronous request invocation
        let inline ( .> ) context f = send context |>  f

        /// asynchronous request invocation
        let inline ( >. ) context f = sendAsync |> context f

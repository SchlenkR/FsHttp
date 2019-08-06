
namespace FsHttp

open System.Net.Http
open System.Text

[<AutoOpen>]
module RequestHandling =

    let inline finalizeContext (context: ^t) =
        (^t: (static member Finalize: ^t -> FinalContext) (context))

    let toMessage (finalContext: FinalContext) : HttpRequestMessage =
        let request = finalContext.request
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
        let invoke() =
            let requestMessage = toMessage finalContext

            let finalRequestMessage =
                match finalContext.config.httpMessageTransformer with
                | None -> requestMessage
                | Some map -> map requestMessage

            // TODO: dispose
            let clientHandler = new HttpClientHandler()
            let client = new HttpClient(clientHandler, Timeout = finalContext.config.timeout)

            let finalClient =
                match finalContext.config.httpClientTransformer with
                | None -> client
                | Some map -> map client
            finalClient.SendAsync(finalRequestMessage)

        async {
            let! response = invoke() |> Async.AwaitTask
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

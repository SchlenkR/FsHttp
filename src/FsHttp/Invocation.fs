
namespace FsHttp

open System.Net.Http
open System.Text

open FsHttp

[<AutoOpen>]
module Invocation =

    let inline internal finalizeContext (context: ^t) =
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
    
    let inline sendAsync (finalContext:FinalContext) =
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

    let send (context:FinalContext) =
        context |> sendAsync |> Async.RunSynchronously

    /// synchronous request invocation
    let inline (.>) context f = finalizeContext context |> send |> f

    /// asynchronous request invocation
    let inline (>.) context f =
        async {
            let! response = finalizeContext context |> sendAsync
            return f response
        }

    // let run map (response:Response) =
    //     map response |> Async.RunSynchronously
    
    // /// run operator for applying an async map functions to a response and receiving the pure result.
    // let (|>>) (response:Response) map = run map response

    // TODO: All Async->Sync functions shouldn't handle explicitly the f parameters
    // let makeSync f = ?

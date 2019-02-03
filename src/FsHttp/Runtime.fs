
namespace FsHttp

open System
open System.Net.Http
open System.Text
open FsHttp
open FSharp.Data
open System.Xml.Linq

[<AutoOpen>]
module Runtime =

    let inline internal finalizeContext (context: ^t) =
        (^t: (static member Finalize: ^t -> FinalContext) (context))

    let inline sendAsync (finalContext:FinalContext) =
        let invoke() =
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

            // TODO: dispose
            let clientHandler = new HttpClientHandler()
            let client = new HttpClient(clientHandler, Timeout = finalContext.config.timeout)
            client.SendAsync(requestMessage)

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
    
    let toStreamAsync (r:Response) = r.content.ReadAsStreamAsync() |> Async.AwaitTask
    let toStream (r:Response) = toStreamAsync r |> Async.RunSynchronously
    
    let toBytesAsync (r:Response) = r.content.ReadAsByteArrayAsync() |> Async.AwaitTask
    let toBytes (r:Response) = toBytesAsync r |> Async.RunSynchronously

    // TODO: Don't read the whole response; read only requested chars.
    let toStringAsync maxLength (r: Response) =
        let getTrimChars (s: string) =
            match s.Length with
            | l when l > maxLength -> "\n..."
            | _ -> ""
        async {
            let! content = r.content.ReadAsStringAsync() |> Async.AwaitTask
            return (Helper.substring content maxLength) + (getTrimChars content)
        }
    let toString maxLength response = toStringAsync maxLength response |> Async.RunSynchronously

    let toTextAsync (r:Response) = toStringAsync Int32.MaxValue r
    let toText (r:Response) = toTextAsync r |> Async.RunSynchronously
    
    let toJsonAsync (r:Response) = async {
        let! s = toTextAsync r 
        return JsonValue.Parse s
    }
    let toJson (r:Response) = toJsonAsync r |> Async.RunSynchronously

    let toXmlAsync (r:Response) = async {
        let! s = toTextAsync r 
        return XDocument.Parse s
    }
    let toXml (r:Response) = toXmlAsync r |> Async.RunSynchronously

    // TODO: toHtml

    /// Tries to convert the response content according to it's type to a formatted string.
    let toFormattedTextAsync (r:Response) =
        async {
            let mediaType = try r.content.Headers.ContentType.MediaType with _ -> ""
            if mediaType.Contains("application/json") then
                let! json = toJsonAsync r
                use tw = new System.IO.StringWriter()
                json.WriteTo (tw, JsonSaveOptions.None)
                return tw.ToString()
            else if mediaType.Contains("application/xml") then
                let! xml = toXmlAsync r
                return xml.ToString(SaveOptions.None)                
            else 
                let! s = toTextAsync r
                return s
        }
    let toFormattedText (r:Response) = toFormattedTextAsync r |> Async.RunSynchronously

    // TODO:
    // Multipart
    // mime types
    // content types
    // body: text, binary, json, etc.

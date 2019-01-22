
namespace FsHttp

open System
open System.Linq
open System.Net.Http
open System.Text
open FsHttp
open FSharp.Data
open System.Xml
open System.Xml.Linq

[<AutoOpen>]
module Runtime =

    let previewPrintLength = 250

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
            let client = new HttpClient()
            client.SendAsync(requestMessage)

        async {
            let! response = invoke() |> Async.AwaitTask
            return
                {
                    content = response.Content;
                    headers = response.Headers;
                    reasonPhrase = response.ReasonPhrase;
                    statusCode = response.StatusCode;
                    requestMessage = response.RequestMessage;
                    version = response.Version;
                    printHint = Show previewPrintLength
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
    
    // TODO: Don't read the whole response; read only requested chars.
    let toStringAsync maxLength (r: Response) =
        let getTrimChars (s: string) =
            match s.Length with
            | l when l > maxLength -> "\n..."
            | _ -> ""
        async {
            let! content = r.content.ReadAsStringAsync() |> Async.AwaitTask
            return
                System.String(content.Take(maxLength).ToArray())
                + (getTrimChars content)
        }
    let toString maxLength (r: Response) =
        (toStringAsync maxLength r) |> Async.RunSynchronously

    let toText (r: Response) =  toString Int32.MaxValue r
    
    /// Tries to convert the response content according to it's type to a formatted string.
    let toFormattedText (r: Response) = 
        
        if r.content.Headers.ContentLength = 0 then ""
        else
            let s = toText r
            if r.content.Headers.ContentType.MediaType.Contains("application/json") then
                let json = JsonValue.Parse s
                use tw = new System.IO.StringWriter()
                json.WriteTo (tw, JsonSaveOptions.None)
                tw.ToString()
            else if r.content.Headers.ContentType.MediaType.Contains("application/xml") then
                let xml = XDocument.Parse s
                xml.ToString(SaveOptions.None)                
            else 
                s


    let toJson (r: Response) =  toText r |> JsonValue.Parse
    let toJsonArray (r: Response) =  ((toString Int32.MaxValue r) |> JsonValue.Parse).AsArray()

    let raw (r: Response) = r
    let go = raw
    let preview = raw
    let show maxLength r = { r with printHint = Show maxLength }
    let expand r = { r with printHint = Expand }


    // TODO:
    // Multipart
    // mime types
    // content types
    // body: text, binary, json, etc.

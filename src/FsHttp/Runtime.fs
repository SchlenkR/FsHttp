
namespace FsHttp

open System
open System.Net.Http
open System.Text
open FsHttp
open FSharp.Data
open System.Xml
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
            let client = new HttpClient()
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
                printHint = {
                    requestPrintHint = {
                        enabled = true;
                        printHeader = true;
                    };
                    responsePrintHint = {
                        enabled = true;
                        printHeader = true;
                        printContent = {
                            enabled = true;
                            format = true;
                            maxLength = 250
                        }
                    }
                }
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
    let toString maxLength (r: Response) =
        let getTrimChars (s: string) =
            match s.Length with
            | l when l > maxLength -> "\n..."
            | _ -> ""
        async {
            let! content = r.content.ReadAsStringAsync() |> Async.AwaitTask
            return (Helper.substring content maxLength) + (getTrimChars content)
        }

    let toText (r:Response) = toString Int32.MaxValue r
    
    let toJson (r:Response) = async {
        let! s = toText r 
        return JsonValue.Parse s
    }

    let toJsonArray (r:Response) = async {
        let! json = toJson r
        return json.AsArray()
    } 

    let toXml (r:Response) = async {
        let! s = toText r 
        return XDocument.Parse s
    }

    // TODO: toHtml

    /// Tries to convert the response content according to it's type to a formatted string.
    let toFormattedText (r:Response) =
        async {
            let mediaType = try r.content.Headers.ContentType.MediaType with _ -> ""
            if mediaType.Contains("application/json") then
                let! json = toJson r
                use tw = new System.IO.StringWriter()
                json.WriteTo (tw, JsonSaveOptions.None)
                return tw.ToString()
            else if mediaType.Contains("application/xml") then
                let! xml = toXml r
                return xml.ToString(SaveOptions.None)                
            else 
                let! s = toText r 
                return s
        }

    [<AutoOpen>]
    module PrintModifier =
        let noRequest printHint = { printHint with requestPrintHint = { printHint.requestPrintHint with enabled = false } }
        let noRequestHeader printHint = { printHint with requestPrintHint = { printHint.requestPrintHint with printHeader = false } }
        let noResponse printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with enabled = false } }
        let noResponseHeader printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with printHeader = false } }
        let noResponseContent printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with printContent = { printHint.responsePrintHint.printContent with enabled = false } } }
        let noResponseContentFormatting printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with printContent = { printHint.responsePrintHint.printContent with format = false } } }
        let withResponseContentMaxLength maxLength printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with printContent = { printHint.responsePrintHint.printContent with maxLength = maxLength } } }

    // Printing (Response -> Response)
    let go (r:Response) = r
    let print f r =
        let response = go r
        { response with printHint = f response.printHint }

    let show maxLength = withResponseContentMaxLength maxLength |> print
    let preview = print id // same as go
    let expand = withResponseContentMaxLength Int32.MaxValue |> print
    let raw = noResponseContentFormatting |> print

    // TODO:
    // Multipart
    // mime types
    // content types
    // body: text, binary, json, etc.

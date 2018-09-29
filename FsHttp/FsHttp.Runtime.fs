
namespace FsHttp

[<AutoOpen>]
module Runtime =

    open System
    open System.Linq
    open System.Net.Http
    open System.Text
    open FsHttp
    open FSharp.Data


    type PrintHint =
        | Header 
        | Preview of maxLength: int
        | Expand

    type Response = {
        content: HttpContent;
        headers: Headers.HttpResponseHeaders;
        reasonPhrase: string;
        statusCode: System.Net.HttpStatusCode;
        requestMessage: HttpRequestMessage;
        version: Version;
        printHint: PrintHint
    }

    let sendAsync (finalContext: FinalContext) =
    
        let invoke() =
            let request = finalContext.request
            let requestMessage = new HttpRequestMessage(request.method, request.url)
        
            requestMessage.Content <-
                match finalContext.content with
                | Some c -> 
                    let stringContent = new StringContent(c.content, System.Text.Encoding.UTF8, c.contentType)
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
                    printHint = Header
                }
        }
    let send (finalContext: FinalContext) = finalContext |> sendAsync |> Async.RunSynchronously

    let headerToString (r: Response) =
        let sb = StringBuilder()
        let printHeader (headers: Headers.HttpHeaders) =
            // TODO: Table formatting
            for h in headers do
                let values = String.Join(", ", h.Value)
                sb.AppendLine (sprintf "%s: %s" h.Key values) |> ignore
        sb.AppendLine() |> ignore
        sb.AppendLine (sprintf "HTTP/%s %d %s" (r.version.ToString()) (int r.statusCode) (string r.statusCode)) |> ignore
        printHeader r.headers
        sb.AppendLine("---") |> ignore
        printHeader r.content.Headers
        sb.ToString()

    let toStringAsync (r: Response) =
        r.content.ReadAsStringAsync() |> Async.AwaitTask
    let toString (r: Response) =
        (toStringAsync r) |> Async.RunSynchronously

    let toStringNAsync maxLength (r: Response) =
        let getTrimChars (s: string) =
            match s.Length with
            | l when l > maxLength -> "\n..."
            | _ -> ""
        async {
            let! content = toStringAsync r
            return
                System.String(content.Take(maxLength).ToArray())
                + (getTrimChars content)
        }
    let toStringN maxLength (r: Response) =
        (toStringNAsync maxLength r) |> Async.RunSynchronously

    let toJson (r: Response) =  toString r |> JsonValue.Parse
    let toJsonArray (r: Response) =  ((toString r) |> JsonValue.Parse).AsArray()

    let headerOnly r = { r with printHint = Header }
    let preview maxLength r = { r with printHint = Preview maxLength }
    let expand r = { r with printHint = Expand }


    let inline private finalizeContext (context: ^t) =
        (^t: (static member finalize: ^t -> FinalContext) (context))

    type HttpBuilderSync() =
        inherit HttpBuilder()
        member inline this.Delay(f: unit -> 'a) =
            f() |> finalizeContext |> send

    type HttpBuilderAsync() =
        inherit HttpBuilder()
        member inline this.Delay(f: unit -> 'a) =
            f() |> finalizeContext |> sendAsync

    let http = HttpBuilderSync()
    let httpAsync = HttpBuilderAsync()

    // TODO:
    // Multipart
    // mime types
    // content types
    // body: text, binary, json, etc.
    // setHeaders anschauen
    // Manche Funktionen sind Synchron.
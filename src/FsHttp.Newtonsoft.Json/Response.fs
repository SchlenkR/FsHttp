module FsHttp.Response

open System
open System.Collections.Generic
open System.Net
open System.Net.Http
open System.Text
open System.Text.Json
open System.Xml.Linq
open FsHttp.Helper
open FsHttp.Domain


// -----------
// Content transformation
// -----------

let maxContentLengthOnParseFail = 1000
let private tryParse text parserName parser =
    try parser text with
    | ex ->
        Exception("Could not parse " + parserName + ": " + (String.substring text maxContentLengthOnParseFail), ex)
        |> raise

let toStreamTAsync (r: Response) = r.content.ReadAsStreamAsync()
let toStreamAsync (r: Response) = toStreamTAsync r |> Async.AwaitTask
let toStream (r: Response) = (toStreamTAsync r).Result

let toBytesAsync (r: Response) = 
    async {
        let! stream = r |> toStreamAsync
        return! stream |> Stream.toBytesAsync
    }
let toBytesTAsync (r: Response) = toBytesAsync r |> Async.StartAsTask
let toBytes (r: Response) = toBytesAsync r |> Async.RunSynchronously

// TODO: Don't read the whole response; read only requested chars.
let toStringAsync maxLength (r: Response) =
    let getTrimChars (s: string) =
        match s.Length with
        | l when l > maxLength -> "\n..."
        | _ -> ""
    async {
        let! content = r.content.ReadAsStringAsync() |> Async.AwaitTask
        return (String.substring content maxLength) + (getTrimChars content)
    }
let toStringTAsync maxLength response =
    toStringAsync maxLength response |> Async.StartAsTask
let toString maxLength response =
    toStringAsync maxLength response |> Async.RunSynchronously

let toTextAsync (r: Response) = toStringAsync Int32.MaxValue r
let toTextTAsync (r: Response) = toTextAsync r |> Async.StartAsTask
let toText (r: Response) = toTextAsync r |> Async.RunSynchronously

let private parseJson text =
    tryParse text "JSON" JsonValue.Parse

let toJsonAsync (r: Response) = 
    async {
        let! s = toTextAsync r 
        return parseJson s
    }
let toJsonTAsync (r: Response) = toJsonAsync r |> Async.StartAsTask
let toJson (r: Response) = toJsonAsync r |> Async.RunSynchronously

let toJsonArrayAsync (r: Response) = 
    async {
        let! res = toJsonAsync r
        return res |> fun json -> json.AsArray()
    }
let toJsonArrayTAsync (r: Response) = toJsonArrayAsync r |> Async.StartAsTask
let toJsonArray (r: Response) = toJsonArrayAsync r |> Async.RunSynchronously

let private parseXml text = tryParse text "XML" XDocument.Parse

let toXmlAsync (r: Response) = 
    async {
        let! s = toTextAsync r 
        return parseXml s
    }
let toXmlTAsync (r: Response) = toXmlAsync r |> Async.StartAsTask
let toXml (r: Response) = toXmlAsync r |> Async.RunSynchronously

// TODO: toHtml

/// Tries to convert the response content according to it's type to a formatted string.
let toFormattedTextAsync (r: Response) =
    async {
        let mediaType = try r.content.Headers.ContentType.MediaType with _ -> ""
        let! s = toTextAsync r
        try
            if mediaType.Contains("/json") then
                let json = parseJson s
                use tw = new System.IO.StringWriter()
                json.WriteTo(tw, JsonSaveOptions.None)
                return tw.ToString()
            else if mediaType.Contains("/xml") then
                return (parseXml s).ToString(SaveOptions.None)
            else
                return s
        with _ ->
            return s
    }
let toFormattedTextTAsync (r: Response) = toFormattedTextAsync r |> Async.StartAsTask
let toFormattedText (r: Response) = toFormattedTextAsync r |> Async.RunSynchronously

let saveFileAsync (fileName: string) (r: Response) = 
    async {
        let! stream = r |> toStreamAsync 
        do! stream |> Stream.saveFileAsync fileName
    }
let saveFileTAsync (fileName: string) (r: Response) = saveFileAsync fileName r |> Async.StartAsTask
let saveFile (fileName: string) (r: Response) = saveFileAsync fileName r |> Async.RunSynchronously

let toResult (response: Response) =
    match int response.statusCode with
    | code when code >= 200 && code < 300 -> Ok response
    | _ -> Error response

let asOriginalHttpRequestMessage (response: Response) =
    response.originalHttpRequestMessage

let asOriginalHttpResponseMessage (response: Response) =
    response.originalHttpResponseMessage


// -----------
// Expect / Assert
// -----------

let expectHttpStatusCodes (codes: HttpStatusCode list) (r: Response) =
    match set codes |> Set.contains r.statusCode with
    | true -> Ok r
    //| false -> Error (sprintf $"Status code {HttpStatusCode.show r.statusCode} is not in expected [{codes}].")
    | false -> Error { expected = codes; actual = r.statusCode }
let expectHttpStatusCode (code: HttpStatusCode) = expectHttpStatusCodes [code]
let expectStatusCodes (codes: int list) = expectHttpStatusCodes (codes |> List.map LanguagePrimitives.EnumOfValue)
let expectStatusCode (code: int) = expectStatusCodes [code]

let assertHttpStatusCodes codes response = expectHttpStatusCodes codes response |> Result.getValueOrThrow StatusCodeExpectedxception
let assertHttpStatusCode code response = assertHttpStatusCodes [code] response
let assertStatusCodes codes response = expectStatusCodes codes response |> Result.getValueOrThrow StatusCodeExpectedxception
let assertStatusCode code response = assertStatusCodes [code] response

let assertOk response = assertStatusCode 200 response
let assertNoContent response = assertStatusCode 204 response
let assertBadRequest response = assertStatusCode 400 response
let assertUnauthorized response = assertStatusCode 401 response
let assertForbidden response = assertStatusCode 403 response
let assertNotFound response = assertStatusCode 404 response
let assert1xx response = assertStatusCodes [100..199] response
let assert2xx response = assertStatusCodes [200..299] response
let assert3xx response = assertStatusCodes [300..399] response
let assert4xx response = assertStatusCodes [400..499] response
let assert5xx response = assertStatusCodes [500..599] response
let assert9xx response = assertStatusCodes [900..999] response
// TODO: Some more explicit expectations


// -----------
// Printing / Show
// -----------

// TODO: Printer for Request
let print (response: Response) =
    let contentIndicator = "===content==="

    let printHeaderCollection (headers: KeyValuePair<string, string seq> seq) =
        let sb = StringBuilder()
        let maxHeaderKeyLength =
            let lengths = headers |> Seq.map (fun h -> h.Key.Length) |> Seq.toList
            match lengths with
            | [] -> 0
            | list -> list |> Seq.max
        for h in headers do
            let values = String.Join(", ", h.Value)
            do sb.appendLine (sprintf "%-*s: %s" (maxHeaderKeyLength + 3) h.Key values)
        sb.ToString()

    let printRequest() =
        let sb = StringBuilder()
        let requestPrintHint = response.request.config.printHint.requestPrintMode
    
        do sb.appendSection "REQUEST"
        do sb.appendLine $"{response.request.header.method} {response.request.header.url.ToUriString()} HTTP/{response.version}"

        let printRequestHeaders () =
            let contentHeaders,multipartHeaders =
                if not (isNull response.requestMessage.Content) then
                    let a = response.requestMessage.Content.Headers |> Seq.toList
                    let b =
                        match response.requestMessage.Content with
                        | :? MultipartFormDataContent as m ->
                            // TODO: After having the request invoked, the dotnet multiparts
                            // have no headers anymore...
                            m
                            |> Seq.collect (fun part -> part.Headers)
                            |> Seq.toList
                        | _ -> []
                    a,b
                else
                    [],[]
            sb.append <|
                printHeaderCollection (
                    (response.requestMessage.Headers |> Seq.toList)
                    @ contentHeaders
                    @ multipartHeaders)

        let printRequestBody () =
            let formatContentData contentData =
                match contentData with
                | StringContent s -> s
                | ByteArrayContent bytes ->
                    sprintf "::ByteArray (length = %d)" bytes.Length
                | StreamContent stream ->
                    sprintf "::Stream (length = %s)" (if stream.CanSeek then stream.Length.ToString() else "?")
                | FormUrlEncodedContent formDataList ->
                    [
                        yield "::FormUrlEncoded"
                        for kvp in formDataList do
                            yield sprintf "    %s = %s" kvp.Key kvp.Value
                    ]
                    |> String.concat "\n"
                | FileContent fileName ->
                    sprintf "::File (name = %s)" fileName

            sb.appendLine contentIndicator
            sb.appendLine <|
                match response.request.content with
                | Empty -> ""
                | Single bodyContent -> formatContentData bodyContent.contentData
                | Multi multipartContent ->
                    [
                        yield "::Multipart"
                        for contentData in multipartContent.contentData do
                            yield (sprintf "-------- %s" contentData.name)
                            yield sprintf "Part content type: %s" (match contentData.contentType with | Some v -> v | _ -> "")
                            yield formatContentData contentData.content
                    ]
                    |> String.concat "\n"
    
        // TODO: bodyConfig
        match requestPrintHint with
        | AsObject ->
            sb.appendLine (sprintf "%A" response.request)
        | HeadersOnly ->
            printRequestHeaders()
        | HeadersAndBody bodyConfig ->
            printRequestHeaders()
            printRequestBody()
    
        sb.newLine()
        sb.ToString()

    let printResponse() =
        let sb = StringBuilder()
        sb.appendSection "RESPONSE"
        sb.appendLine (sprintf "HTTP/%s %d %s" (response.version.ToString()) (int response.statusCode) (string response.statusCode))

        //if r.request.config.printHint.responsePrintMode.printHeader then
        let printResponseHeaders () =
            let allHeaders = (response.headers |> Seq.toList) @ (response.content.Headers |> Seq.toList)
            sb.appendLine (printHeaderCollection allHeaders)

        //if r.request.config.printHint.responsePrintMode.printContent.isEnabled then
        let printResponseBody (format: bool) (maxLength: int option) =
            let trimmedContentText =
                try
                    let contentText =
                        if format then toFormattedText response
                        else toText response
                    match maxLength with
                    | Some maxLength when contentText.Length > maxLength ->
                        (contentText.Substring (0,maxLength)) + $"{Environment.NewLine}..."
                    | _ -> contentText
                with ex -> sprintf "ERROR reading response content: %s" (ex.ToString())
            sb.appendLine contentIndicator
            sb.append trimmedContentText
        
        match response.request.config.printHint.responsePrintMode with
        | AsObject ->
            sb.appendLine (sprintf "%A" response)
        | HeadersOnly ->
            printResponseHeaders()
        | HeadersAndBody bodyConfig ->
            printResponseHeaders()
            printResponseBody bodyConfig.format bodyConfig.maxLength
        
        sb.newLine()
        sb.ToString()
    
    let sb = StringBuilder()
    sb.newLine ()
    sb.append (printRequest ())
    sb.append (printResponse ())

    sb.ToString()

// TODO:
// Multipart extraction
// mime types
// content types

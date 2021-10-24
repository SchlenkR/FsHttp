module FsHttp.Response

open System
open System.Net
open FsHttp.Helper
open FsHttp.Domain
open System.Xml.Linq
open FSharp.Data


/////////////////////////////////
// Content transformation
/////////////////////////////////


let maxContentLengthOnParseFail = 1000
let private tryParse text parserName parser =
    try parser text with
    | ex ->
        Exception("Could not parse " + parserName + ": " + (String.substring text maxContentLengthOnParseFail), ex)
        |> raise
    
let toStreamAsync (r: Response) =
    r.content.ReadAsStreamAsync() |> Async.AwaitTask
let toStream (r: Response) =
    toStreamAsync r |> Async.RunSynchronously

let toBytesAsync (r: Response) = async {
    let! stream = r |> toStreamAsync
    return! stream |> Stream.toBytesAsync
}
let toBytes (r: Response) =
    toBytesAsync r |> Async.RunSynchronously

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
let toString maxLength response =
    toStringAsync maxLength response |> Async.RunSynchronously

let toTextAsync (r: Response) =
    toStringAsync Int32.MaxValue r
let toText (r: Response) =
    toTextAsync r |> Async.RunSynchronously

let private parseJson text =
    tryParse text "JSON" JsonValue.Parse

let toJsonAsync (r: Response) : Async<JsonValue> = async {
    let! s = toTextAsync r 
    return parseJson s
}
let toJson (r: Response) : JsonValue =
    toJsonAsync r |> Async.RunSynchronously

let toJsonArrayAsync (r: Response) = async {
    let! res = toJsonAsync r
    return res |> fun json -> json.AsArray()
}
let toJsonArray (r: Response) =
    toJsonArrayAsync r |> Async.RunSynchronously

let private parseXml text = tryParse text "XML" XDocument.Parse

let toXmlAsync (r: Response) = async {
    let! s = toTextAsync r 
    return parseXml s
}
let toXml (r: Response) =
    toXmlAsync r |> Async.RunSynchronously

// TODO: toHtml

/// Tries to convert the response content according to it's type to a formatted string.
let toFormattedTextAsync (r: Response) =
    async {
        let mediaType = try r.content.Headers.ContentType.MediaType with _ -> ""
        let! s = toTextAsync r
        try
            if mediaType.Contains("application/json") then
                let json = parseJson s
                use tw = new System.IO.StringWriter()
                json.WriteTo(tw, JsonSaveOptions.None)
                return tw.ToString()
            else if mediaType.Contains("application/xml") then
                return (parseXml s).ToString(SaveOptions.None)
            else
                return s
        with _ ->
            return s
    }
let toFormattedText (r: Response) =
    toFormattedTextAsync r |> Async.RunSynchronously

let saveFileAsync (fileName: string) (r: Response) = async {
    let! stream = r |> toStreamAsync 
    do! stream |> Stream.saveFileAsync fileName
}
let saveFile (fileName: string) (r: Response) =
    saveFileAsync fileName r |> Async.RunSynchronously

let toResult (response: Response) =
    match int response.statusCode with
    | code when code >= 200 && code < 300 -> Ok response
    | _ -> Error response

let asOriginalHttpRequestMessage (response: Response) =
    response.originalHttpRequestMessage

let asOriginalHttpResponseMessage (response: Response) =
    response.originalHttpResponseMessage


/////////////////////////////////
// Expect
/////////////////////////////////
    
let expectHttpStatusCodes (codes: HttpStatusCode list) (r: Response) =
    let codes = set codes
    match codes |> Set.contains r.statusCode with
    | true -> Ok ()
    | false -> Error (sprintf $"Status code {HttpStatusCode.show r.statusCode} is not in expected [{codes}].")
let expectHttpStatusCode (code: HttpStatusCode) = 
    expectHttpStatusCodes [code]
let expectStatusCodes (codes: int list) =
    expectHttpStatusCodes (codes |> List.map LanguagePrimitives.EnumOfValue)
let expectStatusCode (code: int) = expectStatusCodes [code]

let assertHttpStatusCodes codes r =
    expectHttpStatusCodes codes r |> Result.raiseOnError
let assertHttpStatusCode code r = 
    expectHttpStatusCode code r |> Result.raiseOnError
let assertStatusCodes codes r =
    expectStatusCodes codes r  |> Result.raiseOnError
let assertStatusCode code r =
    expectStatusCode code r |> Result.raiseOnError

let assertOk = expectStatusCodes [200]
let assertNoContent = expectStatusCodes [204]
let assertBadRequest = expectStatusCodes [400]
let assertUnauthorized = expectStatusCodes [401]
let assertForbidden = expectStatusCodes [403]
let assertNotFound = expectStatusCodes [404]
let assert1xx = expectStatusCodes [100..199]
let assert2xx = expectStatusCodes [200..299]
let assert3xx = expectStatusCodes [300..399]
let assert4xx = expectStatusCodes [400..499]
let assert5xx = expectStatusCodes [500..599]
let assert9xx = expectStatusCodes [900..999]
// TODO: Some more explicit expectations


// TODO:
// Multipart extraction
// mime types
// content types

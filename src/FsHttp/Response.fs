module FsHttp.Response

open System
open System.Xml.Linq
open FSharp.Data
open Domain

let maxContentLengthOnParseFail = 1000
let private tryParse text parserName parser =
    try parser text with
    | ex ->
        Exception("Could not parse " + parserName + ": " + (String.substring text maxContentLengthOnParseFail), ex)
        |> raise
    
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
        return (String.substring content maxLength) + (getTrimChars content)
    }
let toString maxLength response = toStringAsync maxLength response |> Async.RunSynchronously

let toTextAsync (r:Response) = toStringAsync Int32.MaxValue r
let toText (r:Response) = toTextAsync r |> Async.RunSynchronously

let private parseJson text = tryParse text "JSON" JsonValue.Parse

let toJsonAsync (r:Response) = async {
    let! s = toTextAsync r 
    return parseJson s
}
let toJson (r:Response) = toJsonAsync r |> Async.RunSynchronously

let toJsonArrayAsync (r:Response) = async {
    let! res = toJsonAsync r
    return res |> fun json -> json.AsArray()
}
let toJsonArray (r:Response) = toJsonArrayAsync r |> Async.RunSynchronously

let private parseXml text = tryParse text "XML" XDocument.Parse

let toXmlAsync (r:Response) = async {
    let! s = toTextAsync r 
    return parseXml s
}
let toXml (r:Response) = toXmlAsync r |> Async.RunSynchronously

// TODO: toHtml

/// Tries to convert the response content according to it's type to a formatted string.
let toFormattedTextAsync (r:Response) =
    async {
        let mediaType = try r.content.Headers.ContentType.MediaType with _ -> ""
        let! s = toTextAsync r
        try
            if mediaType.Contains("application/json") then
                let json = parseJson s
                use tw = new System.IO.StringWriter()
                json.WriteTo (tw, JsonSaveOptions.None)
                return tw.ToString()
            else if mediaType.Contains("application/xml") then
                return (parseXml s).ToString(SaveOptions.None)
            else
                return s
        with _ ->
            return s
    }
let toFormattedText (r:Response) = toFormattedTextAsync r |> Async.RunSynchronously

let toResult (response: FsHttp.Domain.Response) =
    match int response.statusCode with
    | code when code >= 200 && code < 300 -> Ok response
    | _ -> Error response

// TODO:
// Multipart
// mime types
// content types

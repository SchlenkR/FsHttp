﻿[<AutoOpen>]
module FsHttp.Print

open System
open System.Collections.Generic
open System.Net.Http
open System.Text

open FsHttp
open FsHttp.Helper

// We try to get as close to the underlying representation (requestMessage) when printing!

let internal contentIndicator = "===content==="

let private printHeaderCollection (headers: KeyValuePair<string, string seq> seq) =
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

let private doPrintRequestOnly (request: Request) (requestMessage: HttpRequestMessage) =
    let sb = StringBuilder()
    let requestPrintHint = request.config.printHint.requestPrintMode
    
    do sb.appendSection "REQUEST"
    do sb.appendLine $"{requestMessage.Method.Method} {requestMessage.RequestUri} HTTP/{requestMessage.Version}"

    let printRequestHeaders () =
        let contentHeaders =
            if not (requestMessage.Content = null)
                then requestMessage.Content.Headers |> Seq.toList
                else []
        sb.append <| printHeaderCollection ((requestMessage.Headers |> Seq.toList) @ contentHeaders)

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
            match request.content with
            | Empty -> ""
            | Single bodyContent -> formatContentData bodyContent.contentData
            | Multi multipartContent ->
                [
                    for contentData in multipartContent.part do
                        yield $"-- {contentData.name}"
                        yield "Part content type: " + (match contentData.contentType with | Some v -> v | _ -> "")
                        yield formatContentData contentData.content
                ]
                |> String.concat "\n"
    
    // TODO: bodyConfig
    match requestPrintHint with
    | AsObject ->
        sb.appendLine (sprintf "%A" request)
    | HeadersOnly ->
        printRequestHeaders()
    | HeadersAndBody bodyConfig ->
        printRequestHeaders()
        printRequestBody()
    
    sb.newLine()
    sb.ToString()

let private printRequestOnly (request: IToRequest) =
    let request,requestMessage = request |> Request.toRequestAndMessage
    doPrintRequestOnly "?" request requestMessage

let private printResponseOnly (response: Response) =
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
                    if format then Response.toFormattedText response
                    else Response.toText response
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

let private printRequestAndResponse (response: Response) =
    let sb = StringBuilder()
    sb.newLine ()
    sb.append (doPrintRequestOnly (response.version.ToString()) response.request response.requestMessage)
    sb.append (printResponseOnly response)

    sb.ToString()

module Request =
    let print (request: IToRequest) = printRequestOnly request

module Response =
    let print (response: Response) = printRequestAndResponse response

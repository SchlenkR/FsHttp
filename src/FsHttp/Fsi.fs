
namespace FsHttp

module Fsi =

    open FsHttp

    open System

    let noCustomPrinting printHint = { printHint with isEnabled = false }
    let noRequest printHint = { printHint with requestPrintHint = { printHint.requestPrintHint with enabled = false } }
    let noRequestHeader printHint = { printHint with requestPrintHint = { printHint.requestPrintHint with printHeader = false } }
    let noResponse printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with enabled = false } }
    let noResponseHeader printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with printHeader = false } }
    let withResponseContent printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with printContent = { printHint.responsePrintHint.printContent with enabled = true } } }
    let noResponseContentFormatting printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with printContent = { printHint.responsePrintHint.printContent with format = false } } }
    let withResponseContentMaxLength maxLength printHint =
        { printHint with responsePrintHint = { printHint.responsePrintHint with printContent = { printHint.responsePrintHint.printContent with maxLength = maxLength } } } 
        |> withResponseContent

    // Printing (Response -> Response)
    let modifyPrinter f r = { r with printHint = f r.printHint }

    let raw = noCustomPrinting |> modifyPrinter
    let header = modifyPrinter id
    let show maxLength = (withResponseContentMaxLength maxLength >> withResponseContent) |> modifyPrinter
    let preview = withResponseContent |> modifyPrinter
    let prv = preview
    let go = preview
    let expand = (withResponseContentMaxLength Int32.MaxValue >> withResponseContent) |> modifyPrinter
    let exp = expand

module FsiPrinting =

    open FsHttp

    open System
    open System.Collections.Generic
    open System.Text
    // TODO: Printer for FinalContext

    let print (r: Response) =
        let sb = StringBuilder()

        let append (s:string) = sb.Append s |> ignore
        let appendLine s = sb.AppendLine s |> ignore
        let newLine() = appendLine ""
        let appendSection s =
            appendLine s
            String([0..s.Length] |> List.map (fun _ -> '-') |> List.toArray) |> appendLine
        
        let printHeaderCollection (headers: KeyValuePair<string, string seq> seq) =
            let maxHeaderKeyLength =
                let lengths = headers |> Seq.map (fun h -> h.Key.Length) |> Seq.toList
                match lengths with
                | [] -> 0
                | list -> list |> Seq.max

            for h in headers do
                let values = String.Join(", ", h.Value)
                appendLine (sprintf "%-*s: %s" (maxHeaderKeyLength + 3) h.Key values)

        let printRequest() =
            let requestPrintHint = r.printHint.requestPrintHint
            if requestPrintHint.enabled then
                appendSection "REQUEST"
                appendLine (sprintf "%s %s HTTP/%s" (r.requestContext.request.method.ToString()) r.requestContext.request.url (r.version.ToString()))

                if requestPrintHint.printHeader then
                    let contentHeader =
                        if not (isNull r.requestMessage.Content) 
                        then r.requestMessage.Content.Headers |> Seq.toList 
                        else []

                    printHeaderCollection ((r.requestMessage.Headers |> Seq.toList) @ contentHeader)
                
                newLine()

        let printResponse() =
            if r.printHint.responsePrintHint.enabled then
                appendSection "RESPONSE"
                appendLine (sprintf "HTTP/%s %d %s" (r.version.ToString()) (int r.statusCode) (string r.statusCode))

                if r.printHint.responsePrintHint.printHeader then
                    printHeaderCollection ((r.headers |> Seq.toList) @ (r.content.Headers |> Seq.toList))
                    newLine()

                if r.printHint.responsePrintHint.printContent.enabled then
                    let trimmedContentText =
                        try
                            let contentText =
                                if r.printHint.responsePrintHint.printContent.format then
                                    toFormattedText r
                                else
                                    toText r
                            let maxLength = r.printHint.responsePrintHint.printContent.maxLength
                            if contentText.Length > maxLength then
                                (contentText.Substring (0,maxLength)) + "\n..."
                            else
                                contentText
                        with ex -> sprintf "ERROR reading response content: %s" (ex.ToString())
                    append trimmedContentText
                    newLine()
        
        (newLine >> printRequest >> printResponse)()
        sb.ToString()

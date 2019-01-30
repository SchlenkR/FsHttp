
namespace FsHttp

module FsiPrinting =

    open System
    open System.Collections.Generic
    open System.Text
    open FsHttp

    // TODO: Printer for FinalContext

    let print (r: Response) =
        let sb = StringBuilder()

        let append (s:string) = sb.Append s |> ignore
        let appendLine s = sb.AppendLine s |> ignore
        let newLine() = appendLine ""
        let appendSection s =
            appendLine s
            new System.String([0..s.Length] |> List.map (fun _ -> '-') |> List.toArray) |> appendLine
        
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
            appendSection "REQUEST"
            
            let requestPrintHint = r.printHint.requestPrintHint
            if requestPrintHint.enabled then
                appendLine (sprintf "%s %s HTTP/%s" (r.requestContext.request.method.ToString()) r.requestContext.request.url (r.version.ToString()))

                if requestPrintHint.printHeader then
                    let contentHeader =
                        if r.requestMessage.Content <> null 
                        then r.requestMessage.Content.Headers |> Seq.toList 
                        else []

                    printHeaderCollection ((r.requestMessage.Headers |> Seq.toList) @ contentHeader)
                
                newLine()

        let printResponse() =
            appendSection "RESPONSE"
            
            if r.printHint.responsePrintHint.enabled then
                appendLine (sprintf "HTTP/%s %d %s" (r.version.ToString()) (int r.statusCode) (string r.statusCode))

                if r.printHint.responsePrintHint.printHeader then
                    printHeaderCollection ((r.headers |> Seq.toList) @ (r.content.Headers |> Seq.toList))
                    newLine()

                if r.printHint.responsePrintHint.printContent.enabled then
                    let contentText =
                        if r.printHint.responsePrintHint.printContent.format then
                            toFormattedText r
                        else
                            toText r
                        |> Async.RunSynchronously
                    let trimmedContentText =
                        let maxLength = r.printHint.responsePrintHint.printContent.maxLength
                        if contentText.Length > maxLength then
                            (contentText.Substring (0,maxLength)) + "\n..."
                        else
                            contentText
                    append trimmedContentText
                    newLine()
        
        (newLine >> printRequest >> printResponse)()
        sb.ToString()

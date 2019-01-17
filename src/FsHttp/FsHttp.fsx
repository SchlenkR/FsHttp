
#r "netstandard"
#r "System.Net.Http"
#r "./FsHttp.dll"

open FsHttp

// TODO: Format message based on response content type
(fun (r: Response) ->

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

    let content =
        // TODO: When Json or XML, pretty print output
        match r.printHint with
        | Show maxLength -> toString maxLength r
        | Expand -> toString System.Int32.MaxValue r
        | Header -> toString 500 r
    sprintf "%s\n%s" (headerToString r) content
)
|> fsi.AddPrinter

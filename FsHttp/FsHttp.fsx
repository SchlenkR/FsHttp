
#r "System.Net.Http"

open System
open System.Net.Http
open System.Text


let printer (r: HttpResponseMessage) =
    let sb = StringBuilder()

    let printHeader (headers: Headers.HttpHeaders) =
        for h in headers do
            let values = String.Join(", ", h.Value)
            sb.AppendLine (sprintf "%s: %s" h.Key values) |> ignore
        ()

    sb.AppendLine() |> ignore
    sb.AppendLine (sprintf "HTTP/%s %d %s" (r.Version.ToString()) (int r.StatusCode) (string r.StatusCode)) |> ignore
    printHeader r.Headers

    sb.AppendLine("---") |> ignore
    sb.AppendLine() |> ignore
    printHeader r.Headers

    sb.ToString()

fsi.AddPrinter printer


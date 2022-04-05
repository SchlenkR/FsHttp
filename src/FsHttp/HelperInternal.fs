module internal FsHttp.HelperInternal

open System
open System.Text

let base64Encoding = Encoding.GetEncoding("ISO-8859-1")
let br = Environment.NewLine

type StringBuilder with
    member sb.append (s:string) = sb.Append s |> ignore
    member sb.appendLine (s:string) = sb.AppendLine s |> ignore
    member sb.newLine() = sb.appendLine ""
    member sb.appendSection (s:string) =
        sb.appendLine s
        String([0..s.Length] |> List.map (fun _ -> '-') |> List.toArray) |> sb.appendLine

[<RequireQualifiedAccess>]
module Map =
    let union (m1: Map<'k, 'v>) (s: seq<'k * 'v>) =
        seq {
            yield! m1 |> Seq.map (fun kvp -> kvp.Key, kvp.Value)
            yield! s
        }
        |> Map.ofSeq

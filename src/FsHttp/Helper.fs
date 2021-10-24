namespace FsHttp.Helper

[<RequireQualifiedAccess>]
module String =
    open System
    open System.Text

    let urlEncode (s: string) = System.Web.HttpUtility.UrlEncode(s)

    let private base64Encoding = Encoding.GetEncoding("ISO-8859-1")

    let toBase64 (s: string) =
        s
        |> base64Encoding.GetBytes
        |> Convert.ToBase64String

    let fromBase64 (s: string) =
        s
        |> Convert.FromBase64String
        |> base64Encoding.GetString

    let substring (s:string) maxLength = string(s.Substring(0, Math.Min(maxLength, s.Length)))

[<RequireQualifiedAccess>]
module Map =
    let union (m1: Map<'k, 'v>) (s: seq<'k * 'v>) =
        seq {
            yield! m1 |> Seq.map (fun kvp -> kvp.Key, kvp.Value)
            yield! s
        }
        |> Map.ofSeq


[<RequireQualifiedAccess>]
module Url =
    let internal combine (url1:string) (url2:string) =
        (url1.TrimEnd [|'/'|]) + "/" + (url2.TrimStart [|'/'|])

module internal HttpStatusCode =
    let show (this: System.Net.HttpStatusCode) = $"{int this} ({this})"

module JsonComparison =
    open FSharp.Data
    
    type ArrayComparison = | RespectOrder | IgnoreOrder
    type StructuralComparison = | Subset | Exact
    
    let compareJson (arrayComparison: ArrayComparison) (expectedJson: JsonValue) (resultJson: JsonValue) =
        let rec toPaths (currentPath: string) (jsonValue: JsonValue) : ((string * obj) list) =
            match jsonValue with
            | JsonValue.Null -> [currentPath, null :> obj]
            | JsonValue.Record properties ->
                seq {
                    for pName, pValue in properties do
                    for innerPath in toPaths (sprintf "%s/%s" currentPath pName) pValue do
                    yield innerPath
                } |> Seq.toList
            | JsonValue.Array values ->
                let indexedValues = values |> Array.mapi (fun i x -> i,x)
                seq {
                    for index,value in indexedValues do
                    let printedIndex = match arrayComparison with | RespectOrder -> index.ToString() | IgnoreOrder -> ""
                    for inner in toPaths (sprintf "%s[%s]" currentPath printedIndex) value do
                    yield inner
                } |> Seq.toList
            | JsonValue.Boolean b -> [currentPath, b :> obj]
            | JsonValue.Float f -> [currentPath, f :> obj]
            | JsonValue.String s -> [currentPath, s :> obj]
            | JsonValue.Number n -> [currentPath, n :> obj]
    
        let getPaths x = x |> toPaths "" |> List.map (fun (path,value) -> sprintf "%s{%A}" path value)
        (getPaths expectedJson, getPaths resultJson)
    
[<AutoOpen>]
module JsonExtensions =
    open FSharp.Data

    type JsonValue with
        member this.HasProperty(propertyName: string) =
            let prop = this.TryGetProperty propertyName
            match prop with
            | Some _ -> true
            | None -> false
    
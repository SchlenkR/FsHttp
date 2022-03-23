module FsHttp.FSharpData.Response

open System.IO
open FsHttp
open FsHttp.Helper
open FSharp.Data

let toJsonAsync response = 
    response |> Response.parseAsync "JSON" (fun stream ct ->
        async {
            use sr = new StreamReader(stream)
            let! s = sr.ReadToEndAsync() |> Async.AwaitTask
            return JsonValue.Parse s
        }
    )
let toJsonTAsync response = toJsonAsync response |> Async.StartAsTask
let toJson response = toJsonAsync response |> Async.RunSynchronously

let toJsonArrayAsync response =
    async {
        let! res = toJsonAsync response
        return res.AsArray()
    }
let toJsonArrayTAsync response = toJsonArrayAsync response |> Async.StartAsTask
let toJsonArray response = toJsonArrayAsync response |> Async.RunSynchronously

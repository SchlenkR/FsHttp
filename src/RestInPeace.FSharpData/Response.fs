module RestInPeace.FSharpData.Response

open System.IO
open RestInPeace
open RestInPeace.Helper
open FSharp.Data

let toJsonAsync response =
    response
    |> Response.parseAsync
        "JSON"
        (fun stream ct ->
            async {
                use sr = new StreamReader(stream)
                let! s = sr.ReadToEndAsync() |> Async.AwaitTask
                return JsonValue.Parse s
            }
        )

let toJsonTAsync cancellationToken response =
    Async.StartAsTask(
        toJsonAsync response,
        cancellationToken = cancellationToken)
let toJson response =
    toJsonAsync response |> Async.RunSynchronously

let toJsonArrayAsync response =
    async {
        let! res = toJsonAsync response
        return res.AsArray()
    }

let toJsonArrayTAsync cancellationToken response = 
    Async.StartAsTask(
        toJsonArrayAsync response,
        cancellationToken = cancellationToken)
let toJsonArray response = 
    toJsonArrayAsync response |> Async.RunSynchronously

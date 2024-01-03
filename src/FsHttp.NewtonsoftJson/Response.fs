module FsHttp.NewtonsoftJson.Response

open System.IO

open FsHttp
open FsHttp.Helper
open FsHttp.NewtonsoftJson.GlobalConfig.Json

open Newtonsoft.Json
open Newtonsoft.Json.Linq

// -----------------------------
// JSON.Net doesn't really support async - but we try to keep the API in sync with System.Text.Json
// -----------------------------

let private loadJsonAsync loader response =
    response
    |> Response.parseAsync
        "JSON"
        (fun stream ct ->
            // Don't dispose sr and jr - the incoming stream is owned externally!
            let sr = new StreamReader(stream)
            let jr = new JsonTextReader(sr)
            loader jr ct |> Async.AwaitTask
        )

let toJsonWithAsync settings response = response |> loadJsonAsync (fun jr ct -> JObject.LoadAsync(jr, settings, ct))
let toJsonWithTAsync settings cancellationToken response = 
    Async.StartAsTask(
        toJsonWithAsync settings response,
        cancellationToken = cancellationToken)
let toJsonWith settings response = toJsonWithAsync settings response |> Async.RunSynchronously

let toJsonAsync response = 
    toJsonWithAsync defaultJsonLoadSettings response
let toJsonTAsync cancellationToken response =
    toJsonWithTAsync defaultJsonLoadSettings cancellationToken response
let toJson response = 
    toJsonWith defaultJsonLoadSettings response

let toJsonSeqWithAsync settings response =
    response
    |> loadJsonAsync (fun jr ct -> JArray.LoadAsync(jr, settings, ct))
    |> Async.map (fun jarr -> jarr :> JToken seq)

let toJsonSeqWithTAsync settings cancellationToken response = 
    Async.StartAsTask(
        toJsonSeqWithAsync settings response,
        cancellationToken = cancellationToken)
let toJsonSeqWith settings response = 
    toJsonSeqWithAsync settings response |> Async.RunSynchronously

let toJsonSeqAsync response = 
    toJsonSeqWithAsync defaultJsonLoadSettings response
let toJsonSeqTAsync cancellationToken response = 
    toJsonSeqWithTAsync defaultJsonLoadSettings cancellationToken response
let toJsonSeq response = 
    toJsonSeqWith defaultJsonLoadSettings response

let toJsonArrayWithAsync settings response = 
    toJsonSeqWithAsync settings response |> Async.map Seq.toArray
let toJsonArrayWithTAsync settings cancellationToken response = 
    Async.StartAsTask(
        toJsonArrayWithAsync settings response,
        cancellationToken = cancellationToken)
let toJsonArrayWith settings response = 
    toJsonArrayWithAsync settings response |> Async.RunSynchronously

let toJsonArrayAsync response = 
    toJsonArrayWithAsync defaultJsonLoadSettings response
let toJsonArrayTAsync cancellationToken response = 
    toJsonArrayWithTAsync defaultJsonLoadSettings cancellationToken response
let toJsonArray response = 
    toJsonArrayWith defaultJsonLoadSettings response

let toJsonListWithAsync settings response = 
    toJsonSeqWithAsync settings response |> Async.map Seq.toList
let toJsonListWithTAsync settings cancellationToken response = 
    Async.StartAsTask(
        toJsonListWithAsync settings response,
        cancellationToken = cancellationToken)
let toJsonListWith settings response = 
    toJsonListWithAsync settings response |> Async.RunSynchronously

let toJsonListAsync response = 
    toJsonListWithAsync defaultJsonLoadSettings response
let toJsonListTAsync cancellationToken response = 
    toJsonListWithTAsync defaultJsonLoadSettings cancellationToken response
let toJsonList response = 
    toJsonListWith defaultJsonLoadSettings response

let deserializeJsonWithAsync<'a> (settings: JsonSerializerSettings) response =
    async {
        let json = Response.toText response
        return JsonConvert.DeserializeObject<'a>(json, settings)
    }
let deserializeWithJsonTAsync<'a> settings cancellationToken response =
    Async.StartAsTask(
        deserializeJsonWithAsync<'a> settings response,
        cancellationToken = cancellationToken)
let deserializeWithJson<'a> settings response = 
    deserializeJsonWithAsync<'a> settings response |> Async.RunSynchronously

let deserializeJsonAsync<'a> response = 
    deserializeJsonWithAsync<'a> defaultJsonSerializerSettings response
let deserializeJsonTAsync<'a> cancellationToken response = 
    deserializeWithJsonTAsync<'a> defaultJsonSerializerSettings cancellationToken response
let deserializeJson<'a> response = 
    deserializeWithJson<'a> defaultJsonSerializerSettings response

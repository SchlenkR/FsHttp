module FsHttp.NewtonsoftJson.Response

open System.IO

open FsHttp
open FsHttp.NewtonsoftJson.GlobalConfig.Json

open Newtonsoft.Json
open Newtonsoft.Json.Linq

// -----------------------------
// JSON.Net doesn't really support async - but we try to keep the API in sync with System.Text.Json
// -----------------------------

let private loadJsonAsync loader response =
    response |> Response.parseAsync "JSON" (fun stream ct ->
        // Don't dispose sr and jr - the incoming stream is owned externally!
        let sr = new StreamReader(stream)
        let jr = new JsonTextReader(sr)
        loader jr ct |> Async.AwaitTask)

let toJsonWithAsync settings response =
    response |> loadJsonAsync (fun jr ct -> JObject.LoadAsync(jr, settings, ct))
let toJsonWithTAsync settings response = toJsonWithAsync settings response |> Async.StartAsTask
let toJsonWith settings response = toJsonWithAsync settings response |> Async.RunSynchronously

let toJsonAsync response = toJsonWithAsync defaultJsonLoadSettings response
let toJsonTAsync response = toJsonWithTAsync defaultJsonLoadSettings response
let toJson response = toJsonWith defaultJsonLoadSettings response

let toJsonSeqWithAsync settings response =
    response
    |> loadJsonAsync (fun jr ct -> JArray.LoadAsync(jr, settings, ct))
    |> Async.map (fun jarr -> jarr :> JToken seq)
let toJsonSeqWithTAsync settings response = toJsonSeqWithAsync settings response |> Async.StartAsTask
let toJsonSeqWith settings response = toJsonSeqWithAsync settings response |> Async.RunSynchronously

let toJsonSeqAsync response = toJsonSeqWithAsync defaultJsonLoadSettings response
let toJsonSeqTAsync response = toJsonSeqWithTAsync defaultJsonLoadSettings response
let toJsonSeq response = toJsonSeqWith defaultJsonLoadSettings response

let toJsonArrayWithAsync settings response = toJsonSeqWithAsync settings response |> Async.map Seq.toArray
let toJsonArrayWithTAsync settings response = toJsonArrayWithAsync settings response |> Async.StartAsTask
let toJsonArrayWith settings response = toJsonArrayWithAsync settings response |> Async.RunSynchronously

let toJsonArrayAsync response = toJsonArrayWithAsync defaultJsonLoadSettings response
let toJsonArrayTAsync response = toJsonArrayWithTAsync defaultJsonLoadSettings response
let toJsonArray response = toJsonArrayWith defaultJsonLoadSettings response

let deserializeJsonWithAsync<'a> (settings: JsonSerializerSettings) response =
    async {
        let json = Response.toText response
        return JsonConvert.DeserializeObject<'a>(json, settings)
    }
let deserializeWithJsonTAsync<'a> settings response = deserializeJsonWithAsync<'a> settings response |> Async.StartAsTask
let deserializeWithJson<'a> settings response = deserializeJsonWithAsync<'a> settings response |> Async.RunSynchronously

let deserializeJsonAsync<'a> response = deserializeJsonWithAsync<'a> defaultJsonSerializerSettings response
let deserializeJsonTAsync<'a> response = deserializeWithJsonTAsync<'a> defaultJsonSerializerSettings response
let deserializeJson<'a> response = deserializeWithJson<'a> defaultJsonSerializerSettings response

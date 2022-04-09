module FsHttp.NewtonsoftJson.GlobalConfig

open Newtonsoft.Json
open Newtonsoft.Json.Linq

module Json =
    let mutable defaultJsonLoadSettings = JsonLoadSettings()
    let mutable defaultJsonSerializerSettings = JsonSerializerSettings()

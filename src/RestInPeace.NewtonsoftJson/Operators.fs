namespace RestInPeace.NewtonsoftJson

open Newtonsoft.Json.Linq

[<AutoOpen>]
module JsonDynamic =
    let (?) (json: JToken) (key: string) : JToken = json[key]

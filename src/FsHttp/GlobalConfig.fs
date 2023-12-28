module FsHttp.GlobalConfig

open FsHttp.Domain

let mutable private mutableDefaults = Defaults.defaultConfig

type GlobalConfigWrapper(config: Config option) =
    member this.Config = config |> Option.defaultValue mutableDefaults

    interface IConfigure<ConfigTransformer, GlobalConfigWrapper> with
        member this.Configure(t) = GlobalConfigWrapper(Some(t this.Config))

    interface IConfigure<PrintHintTransformer, GlobalConfigWrapper> with
        member this.Configure(t) = configPrinter this t

let defaults = GlobalConfigWrapper(None)
let set (config: GlobalConfigWrapper) = mutableDefaults <- config.Config

// TODO: Do we need something like this, which is more intuitive, but doesn't
// support the pipelined config API?
////module Defaults =
////    let get () = mutableDefaults
////    let set (config: Config) = mutableDefaults <- config

// TODO: Document this
module Json =
    let mutable defaultJsonDocumentOptions = Defaults.defaultJsonDocumentOptions
    let mutable defaultJsonSerializerOptions = Defaults.defaultJsonSerializerOptions

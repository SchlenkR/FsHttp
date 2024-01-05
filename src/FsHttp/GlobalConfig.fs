module FsHttp.GlobalConfig

open FsHttp.Domain

let mutable private mutableDefaultConfig = Defaults.defaultConfig
let mutable private mutableDefaultPrintHint = Defaults.defaultPrintHint

// This thing enables config settings like in pipe style, but for the "global" config.
type GlobalConfigWrapper(config: Config option, printHint: PrintHint option) =
    member _.Config = config |> Option.defaultValue mutableDefaultConfig
    member _.PrintHint = printHint |> Option.defaultValue mutableDefaultPrintHint

    interface IUpdateConfig<GlobalConfigWrapper> with
        member this.UpdateConfig(transformConfig) =
            let updatedConfig = transformConfig this.Config
            GlobalConfigWrapper(Some updatedConfig, Some this.PrintHint)

    interface IUpdatePrintHint<GlobalConfigWrapper> with
        member this.UpdatePrintHint(transformPrintHint) =
            let updatedConfig = transformPrintHint this.PrintHint
            GlobalConfigWrapper(Some this.Config, Some updatedConfig)

let defaults = GlobalConfigWrapper(None, None)
let set (config: GlobalConfigWrapper) = mutableDefaultConfig <- config.Config

// TODO: Do we need something like this, which is more intuitive, but doesn't
// support the pipelined config API?
////module Defaults =
////    let get () = mutableDefaults
////    let set (config: Config) = mutableDefaults <- config

// TODO: Document this
module Json =
    let mutable defaultJsonDocumentOptions = Defaults.defaultJsonDocumentOptions
    let mutable defaultJsonSerializerOptions = Defaults.defaultJsonSerializerOptions

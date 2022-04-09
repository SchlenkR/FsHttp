module FsHttp.GlobalConfig

open System
open System.Text.Json
open FsHttp.Domain

let inline internal defaultHeadersAndBodyPrintMode() =
    {
        format = true
        maxLength = Some 7000
    }

let mutable internal mutableDefaults =
    { 
        timeout = TimeSpan.FromSeconds 10.0
        printHint = {
            printDebugMessages = false
            requestPrintMode = HeadersAndBody (defaultHeadersAndBodyPrintMode())
            responsePrintMode = HeadersAndBody (defaultHeadersAndBodyPrintMode())
        }
        httpMessageTransformer = None
        httpClientHandlerTransformer = None
        httpClientTransformer = None
        httpClientFactory = None
        httpCompletionOption  = System.Net.Http.HttpCompletionOption.ResponseHeadersRead
        proxy = None
        certErrorStrategy = Default
        bufferResponseContent = false
    }

type GlobalConfigWrapper(config) =
    member this.Config = config
    interface IConfigure<ConfigTransformer, GlobalConfigWrapper> with
        member this.Configure(t) = GlobalConfigWrapper(t mutableDefaults)
    interface IConfigure<PrintHintTransformer, GlobalConfigWrapper> with
        member this.Configure(t) = Domain.configPrinter this t

let defaults = GlobalConfigWrapper mutableDefaults
let set (config: GlobalConfigWrapper) = mutableDefaults <- config.Config

// TODO: Do we need something like this, which is more intuitive, but doesn't
// support the pipelined config API?
////module Defaults =
////    let get () = mutableDefaults
////    let set (config: Config) = mutableDefaults <- config

module Json =
    // TODO: Document this
    let mutable defaultJsonDocumentOptions = JsonDocumentOptions()
    let mutable defaultJsonSerializerOptions = JsonSerializerOptions JsonSerializerDefaults.Web

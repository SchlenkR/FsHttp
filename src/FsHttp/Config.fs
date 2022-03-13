module FsHttp.Config

open System
open FsHttp.Domain

let private defaultPreviewLength = 7000

let defaultHeadersAndBodyPrintMode =
    { 
        format = true
        maxLength = Some defaultPreviewLength
    }

let mutable internal defaultConfig =
    { 
        timeout = TimeSpan.FromSeconds 10.0
        printHint = {
            printDebugMessages = false
            requestPrintMode = HeadersAndBody defaultHeadersAndBodyPrintMode
            responsePrintMode = HeadersAndBody defaultHeadersAndBodyPrintMode
        }
        httpMessageTransformer = None
        httpClientHandlerTransformer = None
        httpClientTransformer = None
        httpClientFactory = None
        httpCompletionOption  = System.Net.Http.HttpCompletionOption.ResponseHeadersRead
        proxy = None
        certErrorStrategy = Default
    }

let setDefaultConfig (setter: Config -> Config) =
    defaultConfig <- setter defaultConfig

module FsHttp.Config

open System

open FsHttp.Domain

let private defaultPreviewLength = 7000

let internal initialPrintHint : PrintHint =
    { isEnabled = true
      requestPrintHint =
          { printHeader = true
            printBody = true }
      responsePrintHint =
          { printHeader = true
            printContent =
                { isEnabled = true
                  format = true
                  maxLength = defaultPreviewLength } } }

let mutable internal defaultConfig =
    { timeout = TimeSpan.FromSeconds 10.0
      printDebugMessages = false
      printHint = initialPrintHint
      httpMessageTransformer = None
      httpClientHandlerTransformer = None
      httpClientTransformer = None
      httpClientFactory = None
      httpCompletionOption  = System.Net.Http.HttpCompletionOption.ResponseHeadersRead
      proxy = None
      certErrorStrategy = Default }

let setDefaultConfig (setter: Config -> Config) =
    defaultConfig <- setter defaultConfig

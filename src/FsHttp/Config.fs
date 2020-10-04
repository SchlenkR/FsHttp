[<AutoOpen>]
module FsHttp.Config

open System
open Domain

let private defaultPreviewLength = 10000

// this is similar to "expand"
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

let mutable internal defaultPrintHint = initialPrintHint
let getPrintHint () = defaultPrintHint
let setPrintHint (printHint: PrintHint) = defaultPrintHint <- printHint

let mutable internal defaultTimeout = TimeSpan.FromSeconds 10.0

let setTimeout (timeout: TimeSpan) =
    if timeout.TotalMilliseconds <= 0.0 then failwith "Negative timeout given."
    defaultTimeout <- timeout

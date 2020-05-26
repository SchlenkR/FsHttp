[<AutoOpen>]
module FsHttp.Config

open System
open Domain

let private defaultPreviewLength = 10000

let internal initialPrintHint =
    { isEnabled = true
      requestPrintHint =
          { isEnabled = true
            printHeader = true }
      responsePrintHint =
          { isEnabled = true
            printHeader = true
            printContent =
                { isEnabled = false
                  format = true
                  maxLength = defaultPreviewLength } } }

let mutable internal defaultPrintHint = initialPrintHint
let getPrintHint () = defaultPrintHint
let setPrintHint (printHint: PrintHint) = defaultPrintHint <- printHint

let mutable internal defaultTimeout = TimeSpan.FromSeconds 10.0

let setTimeout (timeout: TimeSpan) =
    if timeout.TotalMilliseconds <= 0.0 then failwith "Negative timeout given."
    defaultTimeout <- timeout

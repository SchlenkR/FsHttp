
namespace FsHttp

open System

[<AutoOpen>]
module Config =

    let mutable internal defaultTimeout = TimeSpan.FromSeconds 10.0
    let setTimeout (timeout:TimeSpan) =
        if timeout.TotalMilliseconds <= 0.0 then failwith "Negative timeout given."
        defaultTimeout <- timeout

    let private defaultPreviewLength = 5000

    let mutable internal defaultPrintHint =
        {
            isEnabled = true
            requestPrintHint = {
                enabled = true
                printHeader = true
            }
            responsePrintHint = {
                enabled = true
                printHeader = true
                printContent = {
                    enabled = false
                    format = true
                    maxLength = defaultPreviewLength
                }
            }
        }
    let setPrintHint (printHint: PrintHint) = defaultPrintHint <- printHint

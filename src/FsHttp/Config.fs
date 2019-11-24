
namespace FsHttp

open System

[<AutoOpen>]
module Config =

    let private defaultPreviewLength = 5000
    let internal initialPrintHint =
        {
            isEnabled = true
            requestPrintHint = {
                isEnabled = true
                printHeader = true
            }
            responsePrintHint = {
                isEnabled = true
                printHeader = true
                printContent = {
                    isEnabled = false
                    format = true
                    maxLength = defaultPreviewLength
                }
            }
        }
    let mutable internal defaultPrintHint = initialPrintHint
    let setPrintHint (printHint: PrintHint) = defaultPrintHint <- printHint

    let mutable internal defaultTimeout = TimeSpan.FromSeconds 10.0
    let setTimeout (timeout:TimeSpan) =
        if timeout.TotalMilliseconds <= 0.0 then failwith "Negative timeout given."
        defaultTimeout <- timeout

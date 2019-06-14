
namespace FsHttp

open System

[<AutoOpen>]
module PrintModifier =

    let noCustomPrinting printHint = { printHint with isEnabled = false }
    let noRequest printHint = { printHint with requestPrintHint = { printHint.requestPrintHint with enabled = false } }
    let noRequestHeader printHint = { printHint with requestPrintHint = { printHint.requestPrintHint with printHeader = false } }
    let noResponse printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with enabled = false } }
    let noResponseHeader printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with printHeader = false } }
    let withResponseContent printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with printContent = { printHint.responsePrintHint.printContent with enabled = true } } }
    let noResponseContentFormatting printHint = { printHint with responsePrintHint = { printHint.responsePrintHint with printContent = { printHint.responsePrintHint.printContent with format = false } } }
    let withResponseContentMaxLength maxLength printHint =
        { printHint with responsePrintHint = { printHint.responsePrintHint with printContent = { printHint.responsePrintHint.printContent with maxLength = maxLength } } } 
        |> withResponseContent

    // Printing (Response -> Response)
    let print f r = { r with printHint = f r.printHint }

    let raw = noCustomPrinting |> print
    let noContent = print id
    let show maxLength = (withResponseContentMaxLength maxLength >> withResponseContent) |> print
    let preview = withResponseContent |> print
    let go = preview
    let expand = (withResponseContentMaxLength Int32.MaxValue >> withResponseContent) |> print

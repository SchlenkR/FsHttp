module FsHttp.Fsi

let mutable internal logDebugMessages = None
let enableDebugLogs() = logDebugMessages <- Some true
let disableDebugLogs() = logDebugMessages <- Some false
let logfn message =
    let logDebugMessages = logDebugMessages |> Option.defaultValue false
    message |> Printf.kprintf (fun s -> if logDebugMessages then printfn "%s" s)

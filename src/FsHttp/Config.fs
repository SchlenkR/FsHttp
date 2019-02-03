
namespace FsHttp

[<AutoOpen>]
module Config =

    open System

    let mutable internal defaultTimeout:TimeSpan = TimeSpan.FromSeconds 7.5

    let setTimeout (timeout:TimeSpan) =
        if timeout.TotalMilliseconds <= 0.0 then failwith "Negative timeout given."
        defaultTimeout <- timeout

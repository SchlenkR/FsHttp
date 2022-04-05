namespace FsHttp

module Async =
    let map f x = 
        async {
            let! x = x
            return f x
        }
    let await f x = 
        async {
            let! x = x
            return! f x
        }

// TODO: F# 6 task comp switch
module Task =
    let map f x = 
        async {
            let! x = x |> Async.AwaitTask
            return f x
        }
    let await f x = 
        async {
            let! x = x |> Async.AwaitTask
            return! f x |> Async.AwaitTask
        }

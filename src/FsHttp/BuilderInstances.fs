
namespace FsHttp

open FsHttp

[<AutoOpen>]
module BuilderInstances =
        
    type HttpBuilder with
        member this.Bind(m, f) = f m
        member this.Return(x) = x
        member this.Yield(x) = StartingContext
        member this.For(m, f) = this.Bind m f

    type HttpBuilderSync() =
        inherit HttpBuilder()
        member inline this.Delay(f: unit -> 'a) =
            f() |> send

    type HttpBuilderAsync() =
        inherit HttpBuilder()
        member inline this.Delay(f: unit -> 'a) =
            f() |> sendAsync

    type HttpBuilderLazySync() =
        inherit HttpBuilder()
        member inline this.Delay(f: unit -> 'a) =
            fun() -> f() |> finalizeContext

    let http = HttpBuilderSync()
    let httpAsync = HttpBuilderAsync()
    let httpLazy = HttpBuilderLazySync()

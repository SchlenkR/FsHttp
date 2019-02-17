
namespace FsHttp

[<AutoOpen>]
module BuilderInstances =

    open FsHttp

    type HttpBuilder with
        member this.Bind(m, f) = f m
        member this.Return(x) = x
        member this.Yield(x) = StartingContext
        member this.For(m, f) = this.Bind m f

    type HttpBuilderSync() =
        inherit HttpBuilder()
        member inline this.Delay(f: unit -> 'a) = f() .> preview

    type HttpBuilderAsync() =
        inherit HttpBuilder()
        member inline this.Delay(f: unit -> 'a) = f() >. preview

    type HttpBuilderLazy() =
        inherit HttpBuilder()

    let http = HttpBuilderSync()
    let httpAsync = HttpBuilderAsync()
    let httpLazy = HttpBuilderLazy()

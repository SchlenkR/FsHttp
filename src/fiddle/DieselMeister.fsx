#r "../FsHttp/bin/Debug/net5.0/FsHttp.dll"

open FsHttp
open FsHttp.DslCE

type LazyHttpBuilder<'context when 'context :> IToRequest> with

    [<CustomOperation("csv")>]
    member this.Csv(builder: LazyHttpBuilder<_>, csvContent: string) =
        FsHttp.Dsl.Body.content "text/csv" (StringContent csvContent) builder.Context
        |> LazyHttpBuilder

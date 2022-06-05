module FsHttp.FsiInit

open System
open FsHttp.Domain

type InitResult =
    | Uninitialized
    | Initialized
    /// FSI object not found (this is expected when running in a notebook).
    | NoFsiObjectFound
    | IsNotInteractive
    | InitializationError of Exception

let mutable private state = Uninitialized

// This seems like a HACK, but there shouldn't be the requirement of referencing FCS in FSI.
let doInit() =
    if state <> Uninitialized then state else

    let fsiAssemblyName = "FSI-ASSEMBLY"

    let isInteractive =
        // This hack is indeed one (see https://fsharp.github.io/fsharp-compiler-docs/fsi-emit.html)
        AppDomain.CurrentDomain.GetAssemblies()
        |> Array.exists (fun asm -> (*asm.IsDynamic &&*) asm.GetName().Name.StartsWith(fsiAssemblyName))

    state <-
        try
            if isInteractive then
                AppDomain.CurrentDomain.GetAssemblies()
                |> Array.tryFind (fun x -> x.GetName().Name = "FSharp.Compiler.Interactive.Settings")
                |> Option.map (fun asm ->
                    asm.ExportedTypes
                    |> Seq.tryFind (fun t -> t.FullName = "FSharp.Compiler.Interactive.Settings")
                    |> Option.map (fun settings ->
                        settings.GetProperty("fsi")
                        |> Option.ofObj
                        |> Option.map (fun x -> x.GetValue(null)))
                )
                |> Option.flatten
                |> Option.flatten
                |> function
                    | None ->
                        NoFsiObjectFound
                    | Some fsiInstance ->
                        ////let printTransformer (r: Response) =
                        ////    match r.request.config.printHint.isEnabled with
                        ////    | true -> (PrintableResponse r) :> obj
                        ////    | false -> null
                        let printer (r: Response) =
                            try Response.print r
                            with ex -> ex.ToString()
                        let t = fsiInstance.GetType()
                        ////let addPrintTransformer = t.GetMethod("AddPrintTransformer").MakeGenericMethod([| typeof<Response> |])
                        let addPrinter = t.GetMethod("AddPrinter").MakeGenericMethod([| typeof<Response> |])
                        addPrinter.Invoke(fsiInstance, [| printer |]) |> ignore
                        ////addPrintTransformer.Invoke(fsiInstance, [| printTransformer |]) |> ignore
                        Initialized
            else IsNotInteractive
        with ex -> InitializationError ex
    state

let init() = doInit() |> ignore

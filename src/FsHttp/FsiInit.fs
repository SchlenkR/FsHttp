module internal FsHttp.FsiInit

open System
open FsHttp.Domain

let mutable private isInitialized = false
    
// This seems like a HACK, but there shouldn't be the requirement of referencing FCS in FSI.
let init() =
    if isInitialized then () else

    let fsiAssemblyName = "FSI-ASSEMBLY"

    let isInteractive =
        AppDomain.CurrentDomain.GetAssemblies()
        |> Array.exists (fun asm -> asm.IsDynamic && asm.GetName().Name.StartsWith(fsiAssemblyName))

    if isInteractive then
        ////let printTransformer (r: Response) =
        ////    match r.request.config.printHint.isEnabled with
        ////    | true -> (PrintableResponse r) :> obj
        ////    | false -> null
        let printer (r: Response) =
            try Response.print r
            with ex -> ex.ToString()

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
                ////printfn "--- FsHttp: FSI object not found (this is expected when running in a notebook)."
                ()
            | Some fsiInstance ->
                let t = fsiInstance.GetType()

                ////let addPrintTransformer = t.GetMethod("AddPrintTransformer").MakeGenericMethod([| typeof<Response> |])
                let addPrinter = t.GetMethod("AddPrinter").MakeGenericMethod([| typeof<Response> |])
        
                addPrinter.Invoke(fsiInstance, [| printer |]) |> ignore
                ////addPrintTransformer.Invoke(fsiInstance, [| printTransformer |]) |> ignore
                    
                ////printfn "--- FsHttp: Printer successfully registered."
                ()
            
    isInitialized <- true

open System
open System.IO
open System.Reflection
open FSharp.Reflection

let path = Path.Combine(__SOURCE_DIRECTORY__, "../FsHttp/bin/Debug/net5.0/FsHttp.dll")
let asm = Assembly.LoadFile(path)

//let dslHeaderType = asm.GetExportedTypes() |> Array.map (fun t -> t.FullName)

let findType typeName = asm.GetExportedTypes() |> Array.find (fun t -> t.FullName = typeName)

let getMethodsOfType typeName =
    let t = findType typeName
    let methods = 
        t.GetMethods() 
        |> Array.map (fun m -> m.Name) 
        |> Array.except [| "GetType"; "ToString"; "Equals"; "GetHashCode" |]
    do
        let amb = 
            methods 
            |> Array.groupBy id 
            |> Array.filter (fun (_,values) -> values.Length > 1)
            |> Array.map fst
        if amb |> Array.length > 0 then
            failwith $"""Ambiguous methods: {String.concat ";" amb}"""
    methods

let getDslFunctions typeName =
    getMethodsOfType typeName
    
let getDslCEMethods typeName =
    getMethodsOfType typeName

    

let dslHeaderMethods = getDslFunctions "FsHttp.Dsl+Header"
let dslCeHeaderMethods = getDslCEMethods "FsHttp.DslCE+Header"

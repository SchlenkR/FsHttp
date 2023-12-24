#r @"..\FsHttp\bin\Debug\net8.0\FsHttp.dll"

open FsHttp
open System
open System.Reflection

let allTypes = 
    let rec getNestedTypes (t: Type) =
        [
            yield t
            yield! t.GetNestedTypes() |> Seq.collect (fun t -> t :: (getNestedTypes t |> Seq.toList))
        ]
    [
        for t in typedefof<IRequestContext<_>>.Assembly.GetTypes() do
            yield! getNestedTypes t
    ]
    
for t in allTypes do
    let members = t.GetMembers(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Static)
    for memb in members do
        let cop = memb.GetCustomAttribute<CustomOperationAttribute>()
        if cop :> obj <> null then
             printfn $"Type = {t.FullName}, Member = {memb.Name}, Operation = {cop.Name}"

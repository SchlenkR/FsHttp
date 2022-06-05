
#r "../FsHttp/bin/debug/net6.0/fshttp.dll"

FsHttp.FsiInit.doInit()


open System
open System.Reflection

AppDomain.CurrentDomain.GetAssemblies() |> Seq.map (fun asm -> asm.FullName) |> Seq.toList


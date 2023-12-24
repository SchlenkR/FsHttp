
#r "../RestInPeace/bin/debug/net8.0/RestInPeace.dll"

RestInPeace.FsiInit.doInit()


open System
open System.Reflection

AppDomain.CurrentDomain.GetAssemblies() |> Seq.map (fun asm -> asm.FullName) |> Seq.toList


#nowarn "211"

#I __SOURCE_DIRECTORY__                                      // FsHttp.dll (dev after build)
#I "bin/debug/netstandard2.0"                                // FsHttp.dll (dev before build)

#I "../../packages/FSharp.Data/lib/netstandard2.0"           // FSharp.Data.dll - dev before build + paket
#I "../../../../../packages/FSharp.Data/lib/netstandard2.0"  // FSharp.Data.dll - dev after build + paket
#I "../../../FSharp.Data/lib/netstandard2.0"                 // FSharp.Data.dll - package + paket
#I "../../../FSharp.Data.3.1.1/lib/netstandard2.0"           // FSharp.Data.dll - package + nuget

#r "netstandard"
#r "System.Net.Http"
#r "Fsharp.Data.dll"
#r "FsHttp.dll"

open FsHttp
open FsHttp.FsiPrintModifier

// for debugging purpose
// #r "./bin/Debug/netstandard2.0/FsHttp.dll"

type PrintableResponse = | PrintableResponse of Response

let printTransformer (r:Response) =
    match r.printHint.isEnabled with
    | true -> (PrintableResponse r) :> obj
    | false -> null
fsi.AddPrintTransformer printTransformer

let printer (r:PrintableResponse) =
    let (PrintableResponse inner) = r
    try FsiPrinting.print inner
    with ex -> ex.ToString()
fsi.AddPrinter printer

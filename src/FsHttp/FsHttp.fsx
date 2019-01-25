
#r "netstandard"
#r "System.Net.Http"
#r "./FsHttp.dll"

fsi.AddPrinter (fun r -> 
    try FsHttp.FsiPrinting.print r
    with ex -> ex.ToString()
)

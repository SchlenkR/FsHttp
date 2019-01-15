
#r "netstandard"
#r "System.Net.Http"
#r "./FsHttp.dll"

open FsHttp

// TODO: Format message based on response content type
fsi.AddPrinter
    (fun (r: Response) ->
        let content =
            // TODO: When Json or XML, pretty print output
            match r.printHint with
            | Show maxLength -> toString maxLength r
            | Expand -> toString System.Int32.MaxValue r
            | Header -> toString 500 r
        sprintf "%s\n%s" (headerToString r) content
    )
    
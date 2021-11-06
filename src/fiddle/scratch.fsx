#r "../FsHttp/bin/Debug/net5.0/FsHttp.dll"


open FsHttp
open FsHttp.DslCE



get "https://www.google.de" {
    multipart
    stringPart "" ""
}




module Dsl =
    open FsHttp
    open FsHttp.Dsl
    
    Http.get "https://www.wikipedia.de"
    |> Header.accept "application/text"
    |> Body.json "{}"
    |> Request.send
    |> ignore

    Http.get "https://www.wikipedia.de"
    |> Header.accept "text"


open FsHttp
open FsHttp.DslCE
open FsHttp.Operators

% get "https://www.wikipedia.de"
|> Response.httpResponseMessage



open System

let queryParamCharLength qp =
    qp
    |> List.map (fun (k,v) -> k + "=" + v)
    |> String.concat "&" 
    |> String.length
    |> string

http {
    POST "http://echo.jsontest.com"
    query [ "a", "b" ]
}


http {
    POST "http://echo.jsontest.com"
    body
    formUrlEncoded [ "a", "b" ]
}




// retry
let rec retry work resultOk retries = async {
    let! res = work
    if (resultOk res) || (retries = 0) then return res
    else return! retry work resultOk (retries - 1) }
let work = 
    httpLazyAsync {
        GET "http://httpbin.org/status/500:10,200:1"
    }
let retryRes =
    retry work (fun r -> int r.statusCode = 200) 5
    |> Async.RunSynchronously


httpMsg {
    GET "http://www.wikipedia.de"
    query [ "q1", "<>" ]
}


open System

Uri("http://www.google.de?x=a&z=b").ToString()
Uri("http://www.google.de?x=a&z=b").Query

UriBuilder("http://www.google.de")
UriBuilder("http://www.google.de?x=<>").ToString()



post "https://reqres.in/api/users" {
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}
|> Request.send

http {
    POST "https://reqres.in/api/users"
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}


get "http://localhost:5000/test/lines" {
    timeoutInSeconds 20.0
}
|> Request.send
|> Response.toStream
|> fun stream ->
    let mutable i = 0
    let sr = new System.IO.StreamReader(stream)
    while sr.Peek() > 0 do
        let line = sr.ReadLine()
        i <- i + 1
        printfn "%d" i


http {
    POST "http://www.csm-testcenter.org/test"
    multipart
    filePart "c:\\temp\\test.txt"
    filePart "c:\\temp\\test.txt"
}


http {
    GET "https://reqres.in/api/users"
}



http {
    POST "https://reqres.in/api/users"
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}

// shortcuts
open FsHttp.DslCE.Operators

% get "https://reqres.in/api/users"

Config.setDefaultConfig (fun config ->
    printfn "%A" config
    config)

open FsHttp.Fsi

Config.setDefaultConfig (fun config ->
    { config with printHint = Fsi.previewPrinterTransformer config.printHint })





open System
open System.IO
open System.Net
open System.Net.Http
open System.Threading

let httpClient = new HttpClient()

let requestUri = "http://localhost:5000/test/lines"
let request = new HttpRequestMessage(HttpMethod.Get, requestUri)
httpClient.Timeout <- TimeSpan.FromMilliseconds(float Timeout.Infinite)

//let stream = httpClient.GetStreamAsync(requestUri).Result
//let response = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result
let response = httpClient.SendAsync(request).Result
let stream = response.Content.ReadAsStreamAsync().Result
let sr = new StreamReader(stream)

let mutable i = 0
while sr.Peek() > 0 do
    let line = sr.ReadLine()
    i <- i + 1
    printfn "%d" i


printfn "DONE"


open System
open System.Reflection

let isInteractive =
    let asm = Assembly.GetExecutingAssembly()
    asm.IsDynamic && asm.GetName().Name.StartsWith("FSI-ASSEMBLY")

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
|> Option.iter (fun fsiInstance ->
    let t = fsiInstance.GetType()
    let addPrintTransformer = t.GetMethod("AddPrintTransformer").MakeGenericMethod([| typeof<string> |])
    let addPrinter = t.GetMethod("AddPrinter").MakeGenericMethod([| typeof<string> |])

    let printer (x: string) = "sdfsdfsd " + x

    addPrinter.Invoke(fsiInstance, [| printer |]) |> ignore
)

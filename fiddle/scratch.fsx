
#r "nuget: FsHttp"

open FsHttp

http {
    POST "https://reqres.in/api/users"
    CacheControl "no-cache"
    body
    jsonSerialize
        {|
            name = "morpheus"
            job = "leader"
        |}
}


#r "nuget: Newtonsoft.Json"
open Newtonsoft.Json.Linq
let x = JObject.Parse """ { "name": "Hans", "age": 23.4 }  """



open System.IO
open FsHttp
open FsHttp.DslCE
open FsHttp.Operators

let resp =
    % get "https://www.google.de"
    |> Response.asOriginalHttpResponseMessage

let s1 = resp.Content.ReadAsStream()

let c = resp.Content
c.LoadIntoBufferAsync()

let bstream = new BufferedStream(s)

let buffer = Array.zeroCreate<byte> 2000
bstream.Read(buffer, 0, 2000)

bstream.Position <- 0


#r "../FsHttp/bin/Debug/net6.0/FsHttp.dll"

open FsHttp


// Pre-Config
type PreBuilder<'a> = string -> IRequestContext<'a>

let auth (builder: PreBuilder<_>) url =
    builder url {
        Authorization "Bearer 4711"
    }



let httpPre method url = Http.method method url

type Pre<'a> = string * string -> IRequestContext<'a>

let getx (pre: Pre) = 

let pre method url : IRequestContext<'a> =
    httpPre method url {
        Accept "sdfsdf"
    }



get "https://www.google.de" {
    CacheControl "no-cache"
}
|> Request.send


let a = get "https://www.google.de"
let a' = get "https://www.google.de" :> IBuilder<HeaderContext>

let x = a' {
    multipart
    textPart "" ""
}

get "https://www.google.de" {
    multipart
    textPart "" ""
}
|> Request.send


http {
    GET "https://www.google.de"
    multipart
    textPart "" ""
}
|> Request.send


http {
    DELETE $"..."
    header "Access-Token" "..."
}


module Dsl =
    get "https://www.wikipedia.de"
    |> Header.accept "application/text"
    |> Body.json "{}"
    |> Request.send
    |> ignore

    get "https://www.wikipedia.de"
    |> Header.accept "text"


open FsHttp.Operators

% get "https://www.wikipedia.de"
|> Response.asOriginalHttpResponseMessage



open System

let queryParamCharLength qp =
    qp
    |> List.map (fun (k,v) -> k + "=" + v)
    |> String.concat "&" 
    |> String.length
    |> string

// TODO: Docu
http {
    POST "http://echo.jsontest.com"
    query [ "a", "b" ]
}


http {
    POST "http://echo.jsontest.com"
    body
    formUrlEncoded [ "a", "b" ]
}




// retry (TODO)
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
    config_timeoutInSeconds 20.0
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
    asm.IsDynamic && asm.GetName().Name.StartsWith("FSI-ASSEMBLY", StringComparison.Ordinal)

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






//type StartingBuilder() =
//    do
//        printfn $"new StartingBuilder"
//    member this.Yield(_) =
//        printfn $"StartingBuilder Yield"
//        Builder 99
//and Builder(context: int) =
//    do
//        printfn $"new Builder: {context}"
//    member this.Context = context
//    member this.Yield(_) =
//        printfn $"StartingBuilder Yield"
//        Builder 99
//let httpLazy = StartingBuilder()

//type Builder with
//    [<CustomOperation("test")>]
//    member inline this.Test(builder: StartingBuilder) =
//        Builder (builder.Context + 1)



type ConfigBuilder(value: int) =
    member _.Value = value
    [<CustomOperation("a")>]
    member inline this.Test(builder: ConfigBuilder) =
        printfn $"test"
        ConfigBuilder (value + 1)

type Builder(context: int option) =
    do
        printfn $"new Builder: {context}"
    member this.Context = context |> Option.defaultValue 10
    member this.Yield(_: unit) =
        printfn $"Yield unit"
        Builder context
    member this.Yield(b: Builder) =
        printfn $"Yield builder"
        Builder context
    member this.Yield(s: string) =
        printfn $"Yield string"
        Builder context
    member this.Delay(f) = f
    member this.For((args: Builder, delayedArgs: unit -> Builder)) =
        args.Context + (delayedArgs()).Context |> Some |> Builder
    member this.Combine((args: Builder, delayedArgs: unit -> Builder)) =
        args.Context + (delayedArgs()).Context |> Some |> Builder
let b = Builder(None)


type Config = { timeout: int; ignoreCert: bool }
let config = { timeout = 10; ignoreCert = false }

type Builder with
    [<CustomOperation("test")>]
    member inline this.Test(builder: Builder) =
        printfn $"test"
        Builder (Some (builder.Context + 1))
    [<CustomOperation("configure")>]
    member inline this.Configure(builder: Builder, config: Config) =
        printfn $"test"
        Builder (Some (builder.Context + 1))

b {
    test
    test
    configure { config with ignoreCert = true }
    yield "Hurz"
    test
}

open System

module C =
    let mutable a = DateTime.Now
    let getDt () = a
    let x = getDt()

AppDomain.CurrentDomain.GetAssemblies() 
|> Array.find (fun x -> x.GetName().Name = "FSI-ASSEMBLY")
|> fun asm -> asm.GetTypes()
|> Seq.find (fun t -> t.Name = "B")
|> fun t -> t.GetProperties()

C.a <- DateTime.Now
let a = C.x
let b = C.x

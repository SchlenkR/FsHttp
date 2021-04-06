
#if INTERACTIVE
#r @"../../packages/fsharp.data/lib/net45/FSharp.Data.dll"
#r @"../../packages/NUnit/lib/netstandard2.1/nunit.framework.dll"
#r @"../../packages/fsunit/lib/netstandard2.1/FsUnit.NUnit.dll"
#r @"../../packages/Suave/lib/netstandard2.1\Suave.dll"
#r @"../FsHttp/bin/Debug/netstandard2.1/FsHttp.dll"
#r @"../FsHttp.NUnit/bin/Debug/netstandard2.1/FsHttp.NUnit.dll"
#load @"./Server.fs"
#else
module ``Integration tests for FsHttp``
#endif

open FsUnit
open FsHttp
open FsHttp.DslCE
open FsHttp.Testing
open NUnit.Framework
open Server
open System
open System.IO
open System.Net
open System.Net.Http
open System.Threading
open Suave
open Suave.Cookie
open Suave.ServerErrors
open Suave.Operators
open Suave.Filters
open Suave.Utils.Collections
open Suave.Successful
open Suave.Response
open Suave.Writers

[<AutoOpen>]
module Helper =

    let keyNotFoundString = "KEY_NOT_FOUND"
    let query key (r: HttpRequest) = defaultArg (Option.ofChoice (r.query ^^ key)) keyNotFoundString
    let header key (r: HttpRequest) = defaultArg (Option.ofChoice (r.header key)) keyNotFoundString
    let form key (r: HttpRequest) = defaultArg (Option.ofChoice (r.form ^^ key)) keyNotFoundString
    let text (r: HttpRequest) = r.rawForm |> System.Text.Encoding.UTF8.GetString


[<TestCase>]
let ``Synchronous calls are invoked immediately``() =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve

    http { GET (url @"?test=Hallo") }
    |> Response.toText
    |> should equal "test=Hallo"

[<TestCase>]
let ``Asynchronous calls are sent immediately``() =

    let mutable time = DateTime.MaxValue

    use server =
        GET
        >=> request (fun r ->
            time <- DateTime.Now
            r.rawQuery |> OK)
        |> serve

    let req =
        get (url "?test=Hallo")
        |> Request.sendAsync
    
    Thread.Sleep 3000

    req
    |> Async.RunSynchronously
    |> Response.toText
    |> should equal "test=Hallo"

    (DateTime.Now - time > TimeSpan.FromSeconds 2.0) |> should equal true

[<TestCase>]
let ``Split URL are interpreted correctly``() =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve

    http { GET (url @"
                    ?test=Hallo
                    &test2=Welt")
    }
    |> Response.toText
    |> should equal "test=Hallo&test2=Welt"

[<TestCase>]
let ``Smoke test for a header``() =
    use server = GET >=> request (header "accept-language" >> OK) |> serve

    let lang = "zh-Hans"
    
    http {
        GET (url @"")
        AcceptLanguage lang
    }
    |> Response.toText
    |> should equal lang

[<TestCase>]
let ``ContentType override``() =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let contentType = "application/xxx"

    http {
        POST (url @"")
        body
        ContentType contentType
        text "hello world"
    }
    |> Response.toText
    |> should contain contentType

[<TestCase>]
let ``Multiline urls``() =
    use server = 
        GET
        >=> request (fun r -> (query "q1" r) + "_" + (query "q2" r) |> OK)
        |> serve

    http {
        GET (url @"
                    ?q1=Query1
                    &q2=Query2")
    }
    |> Response.toText
    |> should equal "Query1_Query2"

[<TestCase>]
let ``Comments in urls are discarded``() =
    use server =
        GET 
        >=> request (fun r -> (query "q1" r) + "_" + (query "q2" r) + "_" + (query "q3" r) |> OK)
        |> serve

    http {
        GET (url @"
                    ?q1=Query1
                    //&q2=Query2
                    &q3=Query3")
    }
    |> Response.toText
    |> should equal ("Query1_" + keyNotFoundString + "_Query3")


[<TestCase>]
let ``Empty query params``() =
    use server = 
        GET
        >=> request (fun _ -> "" |> OK)
        |> serve

    http {
        GET (url "")
        query []
    }
    |> Response.toText
    |> should equal ""
    
[<TestCase>]
let ``Merge query params with url params``() =
    use server = 
        GET
        >=> request (fun r -> (query "q1" r) + "_" + (query "q2" r) |> OK)
        |> serve

    http {
        GET (url "?q1=Query1")
        query ["q2", "Query2"]
    }
    |> Response.toText
    |> should equal "Query1_Query2"    
    
[<TestCase>]
let ``Query params``() =
    use server = 
        GET
        >=> request (fun r -> (query "q1" r) + "_" + (query "q2" r) |> OK)
        |> serve

    http {
        GET (url "")
        query ["q1", "Query1"
               "q2", "Query2"]
    }
    |> Response.toText
    |> should equal "Query1_Query2"

[<TestCase>]
let ``POST string data``() =
    use server =
        POST 
        >=> request (text >> OK)
        |> serve

    let data = "hello world"

    http {
        POST (url @"")
        body
        text data
    }
    |> Response.toText
    |> should equal data

[<TestCase>]
let ``POST binary data``() =
    use server =
        POST 
        >=> request (fun r -> r.rawForm |> Suave.Successful.ok)
        |> serve

    let data = [| 12uy; 22uy; 99uy |]

    http {
        POST (url @"")
        body
        binary data
    }
    |> Response.toBytes
    |> should equal data

[<TestCase>]
let ``POST Form url encoded data``() =
    use server =
        POST 
        >=> request (fun r -> (form "q1" r) + "_" + (form "q2" r) |> OK) 
        |> serve

    http {
        POST (url @"")
        body
        formUrlEncoded [
            "q1","Query1"
            "q2","Query2"
        ]
    }
    |> Response.toText
    |> should equal ("Query1_Query2")

[<TestCase>]
let ``POST Multipart form data``() =
    
    let joinLines =  String.concat "\n"
    
    use server =
        POST 
        >=> request (fun r ->
            let fileContents =
                r.files
                |> List.map (fun f -> File.ReadAllText f.tempFilePath)
                |> joinLines
            let multipartContents =
                r.multiPartFields
                |> List.map (fun (k,v) -> k + "=" + v)
                |> joinLines
            [ fileContents; multipartContents ] |> joinLines |> OK)
        |> serve

    http {
        POST (url @"")
        multipart
        filePart "uploadFile.txt"
        filePart "uploadFile2.txt"
        valuePart "hurz1" "das"
        valuePart "hurz2" "Lamm"
        valuePart "hurz3" "schrie"
    }
    |> Response.toText
    |> should equal (joinLines [
        "I'm a chicken and I can fly!"
        "Lemonade was a popular drink, and it still is."
        "hurz1=das"
        "hurz2=Lamm"
        "hurz3=schrie"
    ])

// TODO: Post single file

// TODO: POST stream
// TODO: POST multipart

[<TestCase>]
let ``Expect status code``() =
    use server = GET >=> BAD_GATEWAY "" |> serve

    http { GET (url @"") }
    |> statusCodeShouldBe System.Net.HttpStatusCode.BadGateway
    |> ignore

    Assert.Throws<AssertionException>(fun() ->
        http { GET (url @"") }
        |> statusCodeShouldBe System.Net.HttpStatusCode.Ambiguous
        |> ignore
    )
    |> ignore

[<TestCase>]
let ``Specify content type explicitly``() =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let contentType = "application/whatever"
    
    http {
        POST (url @"")
        body
        ContentType contentType
    }
    |> Response.toText
    |> should equal contentType

[<TestCase>]
let ``Default content type for JSON is specified correctly``() =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    http {
        POST (url @"")
        body
        json " [] "
    }
    |> Response.toText
    |> should equal MimeTypes.applicationJson

[<TestCase>]
let ``Explicitly specified content type is dominant``() =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let explicitContentType = "application/whatever"

    http {
        POST (url @"")
        body
        ContentType explicitContentType
        json " [] "
    }
    |> Response.toText
    |> should equal explicitContentType

[<TestCase>]
let ``Explicitly specified content type part is dominant``() =
    
    let explicitContentType1 = "application/whatever1"
    let explicitContentType2 = "application/whatever2"

    use server =
        POST 
        >=> request (fun r ->
            r.files
            |> List.map (fun f -> f.mimeType)
            |> String.concat ","
            |> OK)
        |> serve

    http {
        POST (url @"")
        multipart

        ContentTypePart explicitContentType1
        filePart "uploadFile.txt"
        
        ContentTypePart explicitContentType2
        filePart "uploadFile2.txt"
    }
    |> Response.toText
    |> should equal (explicitContentType1 + "," + explicitContentType2)

[<TestCase>]
let ``Cookies can be sent``() =
    use server =
        GET
        >=> request (fun r ->
            r.cookies
            |> Map.find "test"
            |> fun httpCookie -> httpCookie.value
            |> OK)
        |> serve

    http {
        GET (url @"")
        Cookie "test" "hello world"
    }
    |> Response.toText
    |> should equal "hello world"

[<TestCase>]
let ``Custom HTTP method``() =
    use server =
        ``method`` (HttpMethod.parse "FLY")
        >=> request (fun r -> OK "flying")
        |> serve

    http {
        Request "FLY" (url @"")
    }
    |> Response.toText
    |> should equal "flying"

[<TestCase>]
let ``Custom Headers``() =
    let customHeaderKey = "X-Custom-Value"

    use server =
        GET
        >=> request (fun r ->
            r.header customHeaderKey
            |> function | Choice1Of2 v -> v | Choice2Of2 e -> failwithf "Failed %s" e
            |> OK)
        |> serve

    http {
        GET (url @"")
        Header customHeaderKey "hello world"
    }
    |> Response.toText
    |> should equal "hello world"
    
[<TestCase>]
let ``Shortcut for GET works``() =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve
    
    get (url @"?test=Hallo") { send }
    |> Response.toText
    |> should equal "test=Hallo"

[<TestCase>]
let ``Proxy usage works`` () =
    use server = GET >=> OK "proxified" |> serve

    http {
        GET "http://google.com"
        proxy (url "")
    }
    |> Response.toText
    |> should equal "proxified"

[<TestCase>]
let ``Proxy usage with credentials works`` () =
    use server =
        GET >=> request (fun r ->
            printfn "Headers: %A" r.headers
            
            match r.header "Proxy-Authorization" with
            | Choice1Of2 cred -> cred |> OK
            | _ ->
                response HTTP_407 (Text.Encoding.UTF8.GetBytes "No credentials")
                >=> setHeader "Proxy-Authenticate" "Basic")
        |> serve
    let credentials = NetworkCredential("test", "password")

    http {
        GET "http://google.com"
        proxyWithCredentials (url "") credentials
    }
    |> Response.toText
    |> should equal ("Basic " + ("test:password" |> Text.Encoding.UTF8.GetBytes |> Convert.ToBase64String))

[<TestCase>]
let ``Inject custom HttpClient`` () =
    use server = GET >=> OK "" |> serve

    let mutable intercepted = false
    
    let interceptor =
        { new DelegatingHandler(InnerHandler = new HttpClientHandler()) with
            member _.SendAsync(request: HttpRequestMessage, cancellationToken: CancellationToken) =
                intercepted <- true
                base.SendAsync(request, cancellationToken) }
    let httpClient = new HttpClient(interceptor)

    intercepted |> should equal false

    http {
        GET "http://google.com"
        useHttpClient httpClient
    }
    |> ignore
    
    intercepted |> should equal true
   
// TODO: 

// [<TestCase>]
// let ``Http reauest message can be modified``() =
//     use server = GET >=> request (header "accept-language" >> OK) |> serve
    
//     let lang = "fr"
//     http {
//         GET (url @"")
//         transformHttpRequestMessage (fun httpRequestMessage ->
//             httpRequestMessage
//         )
//     }
//     |> toText
//     |> should equal lang

// TODO: Timeout
// TODO: ToFormattedText
// TODO: transformHttpRequestMessage
// TODO: transformHttpClient
// TODO: Cookie tests (test the overloads)

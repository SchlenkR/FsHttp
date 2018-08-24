
module FsHttp

open System
open System.Net.Http


type Header = {
    url: string;
    method: HttpMethod;
    headers: (string*string) list;
}

type Content = {
    content: string;
    contentType: string;
    headers: (string*string) list;
} 


type StartingContext = StartingContext


type FinalContext = {
    request: Header;
    content: Content option;
}

let invoke (context:FinalContext) =
    let request = context.request
    let requestMessage = new HttpRequestMessage(request.method, request.url)
    
    requestMessage.Content <-
        match context.content with
        | Some c -> 
            let stringContent = new StringContent(c.content, System.Text.Encoding.UTF8, c.contentType)
            for name,value in c.headers do
                stringContent.Headers.TryAddWithoutValidation(name, value) |> ignore
            stringContent
        | _ -> null
    
    for name,value in request.headers do
        requestMessage.Headers.TryAddWithoutValidation(name, value) |> ignore
    
    use client = new HttpClient()
    client.SendAsync(requestMessage).Result

// let run (context:FinalContext) =
//     let response = context |> invoke
//     let content = response.Content.ReadAsStringAsync().Result
//     printfn "%A" content



type HeaderContext = {
    request: Header;
} with
    static member header (this:HeaderContext, name:string, value:string) = this
    static member run (this:HeaderContext) =
        let finalContext = { request=this.request; content=None }
        invoke finalContext

type BodyContext = {
    request: Header;
    content: Content;
} with
    static member header (this:BodyContext, name:string, value:string) = this
    static member run (this:BodyContext) =
        let finalContext:FinalContext = { request=this.request; content=Some this.content }
        invoke finalContext

let inline run (context:^t) =
    let response = (^t: (static member run: ^t -> HttpResponseMessage) (context))
    let content = response.Content.ReadAsStringAsync().Result
    content


type HttpBuilder() =

    member this.Bind(m, f) = f m
    member this.Return(x) = x
    member this.Yield(x) = StartingContext
    member this.For(m, f) = this.Bind m f

// Request methods
type HttpBuilder with

    [<CustomOperation("Request")>]
    member this.CreateRequest (context:StartingContext) (method:HttpMethod) (url:string) =
        let formattedUrl =
            url.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries)
            |> Seq.map (fun x -> x.Trim())
            |> Seq.filter (fun x -> not (x.StartsWith("//")))
            |> Seq.reduce (+)
        {
            request = { url=formattedUrl; method=method; headers=[] }
        }

    // RFC 2626 specifies 8 methods
    [<CustomOperation("GET")>]
    member this.Get(StartingContext, url:string) =
        this.CreateRequest StartingContext HttpMethod.Get url
    [<CustomOperation("PUT")>]
    member this.Put(StartingContext, url:string) =
        this.CreateRequest StartingContext HttpMethod.Put url
    [<CustomOperation("POST")>]
    member this.Post(StartingContext, url:string) =
        this.CreateRequest StartingContext HttpMethod.Post url
    [<CustomOperation("DELETE")>]
    member this.Delete(StartingContext, url:string) =
        this.CreateRequest StartingContext HttpMethod.Delete url
    [<CustomOperation("OPTIONS")>]
    member this.Options(StartingContext, url:string) =
        this.CreateRequest StartingContext HttpMethod.Options url
    [<CustomOperation("HEAD")>]
    member this.Head(StartingContext, url:string) =
        this.CreateRequest StartingContext HttpMethod.Head url
    [<CustomOperation("TRACE")>]
    member this.Trace(StartingContext, url:string) =
        this.CreateRequest StartingContext HttpMethod.Trace url
    // TODO: Connect
    // [<CustomOperation("CONNECT")>]
    // member this.Post(StartingContext, url:string) =
    //     this.CreateRequest StartingContext HttpMethod.Connect url

    // RFC 4918 (WebDAV) adds 7 methods
    // TODO

// Header
type HttpBuilder with

    [<CustomOperation("header")>]
    member inline this.Header(context:^t, name, value) =
        (^t: (static member header: ^t * string * string -> ^t) (context,name,value))
        
// body
type HttpBuilder with
    [<CustomOperation("body")>]
    member this.Body(context:HeaderContext) : BodyContext =
        {
            request = context.request;
            content = { content=""; contentType=""; headers=[] }
        }
    
    [<CustomOperation("json")>]
    member this.Json(context:BodyContext, json:string) =
        let content = context.content
        { context with
            content = { content with content=json; contentType="application/json";  }
        }

let http = HttpBuilder()


let test x = x

http {
   POST @"http://www.google.de"
   header "a" "b"

   body
   header "c" "d"
   json """
   {
       "name": "hsfsdf sdf s"
   }
   """
}
|> run
|> printfn "%A"

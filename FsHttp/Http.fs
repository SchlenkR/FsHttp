
module SchlenkR.FsHttp

open System
open System.Net.Http

type Request = {
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
    request: Request;
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

let run (context:FinalContext) =
    let response = context |> invoke
    let content = response.Content.ReadAsStringAsync().Result
    printfn "%A" content

type RequestContext = {
    request: Request;
} with
    static member header (this:RequestContext, name:string, value:string) = this
    static member run (this:RequestContext, name:string, value:string) = this

type ContentContext = {
    request: Request;
    content: Content;
} with
    static member header (this:ContentContext, name:string, value:string) = this

type HttpBuilder() =

    let initializeRequest (context:StartingContext) (url:string) (method:HttpMethod) =
        let formattedUrl =
            url.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries)
            |> Seq.map (fun x -> x.Trim())
            |> Seq.filter (fun x -> not (x.StartsWith("//")))
            |> Seq.reduce (+)
        {
            request = { url=formattedUrl; method=method; headers=[] }
        }

    member this.Bind(m, f) = f m
    member this.Return(x) = x
    member this.Yield(x) = StartingContext
    member this.For(m, f) = this.Bind m f

    [<CustomOperation("GET")>]
    member this.Get(StartingContext, url:string) =
        initializeRequest StartingContext url HttpMethod.Get
    [<CustomOperation("PUT")>]
    member this.Put(StartingContext, url:string) =
        initializeRequest StartingContext url HttpMethod.Put
    [<CustomOperation("POST")>]
    member this.Post(StartingContext, url:string) =
        initializeRequest StartingContext url HttpMethod.Post

    [<CustomOperation("header")>]
    member inline this.Header(context:^t, name, value) =
        (^t: (static member header: ^t * string * string -> ^t) (context,name,value))
        
    [<CustomOperation("content")>]
    member this.Content(context:RequestContext) : ContentContext =
        {
            request = context.request;
            content = { content=""; contentType=""; headers=[] }
        }
    
    [<CustomOperation("json")>]
    member this.Json(context:ContentContext, json:string) =
        let content = context.content
        { context with
            content = { content with content=json; contentType="application/json";  }
        }

let http = HttpBuilder()



//http {
//    POST @"http://www.google.de"
//    header "a" "b"

//    content
//    header "c" "d"
//    json """
//    {
//        "name": "hsfsdf sdf s"
//    }
//    """
//}
//|> run

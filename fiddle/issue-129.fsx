
#r "../src/FsHttp/bin/debug/net7.0/FsHttp.dll"

open System.IO
open System.Net.Http
open System.Net.Http.Headers
open FsHttp

let superBodyContentType = { ContentType.value = "superBody"; charset = None }

type IRequestContext<'self> with
    [<CustomOperation("superBody")>]
    member this.SuperBody(context: IRequestContext<BodyContext>, csvContent: string) =
        FsHttp.Dsl.Body.content superBodyContentType (TextContent csvContent) context.Self

MediaTypeHeaderValue.Parse("text/xxx; charset=utf-8")


http {
    POST "http://www.google.de"
    body
    superBody "Hello"
    print_useObjectFormatting
}

http {
    POST "http://www.google.de"
    multipart

    textPart "Resources/uploadFile.txt" "name1"
    ContentType "text/json"

    textPart "Resources/uploadFile.txt" "name2"
    ContentType "text/json"
}
|> Request.toHttpRequestMessage


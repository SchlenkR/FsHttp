
#r "../src/FsHttp/bin/debug/net7.0/FsHttp.dll"

open System.IO
open System.Net.Http
open FsHttp

let request = task {
    let! theContent = File.ReadAllTextAsync("c:/temp/content.txt")
    return http {
        GET "https://www.wikipedia.de"
        config_transformHttpRequestMessage (fun msg ->
            msg.Content <- new TextContent(theContent)
            msg
        )
    }
}

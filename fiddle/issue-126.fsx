
#r "../src/RestInPeace/bin/debug/net7.0/RestInPeace.dll"

open System.IO
open System.Net.Http
open RestInPeace

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

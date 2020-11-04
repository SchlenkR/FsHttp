
#load "./bin/Debug/netstandard2.0/FsHttp.fsx"

open FsHttp
open FsHttp.DslCE



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

get "https://reqres.in/api/users" {go}




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

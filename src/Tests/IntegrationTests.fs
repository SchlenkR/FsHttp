
#if INTERACTIVE
#r @"..\packages\NUnit\lib\netstandard2.0\NUnit.Framework.dll"
#r @"..\packages\FsUnit\lib\netstandard2.0\FsUnit.NUnit.dll"
#r @"..\packages\Suave\lib\netstandard2.0\Suave.dll"
#r @"..\FsHttp\bin\Debug\netstandard2.0\FsHttp.dll"
#load "Server.fs"
#else
module ``Integration tests for FsHttp``
#endif


open FsUnit
open FsHttp
open NUnit.Framework
open Server
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Utils.Collections
open Suave.Successful


let keyNotFoundString = "KEY_NOT_FOUND"
let query key (r: HttpRequest) = defaultArg (Option.ofChoice (r.query ^^ key)) keyNotFoundString
let header key (r: HttpRequest) = defaultArg (Option.ofChoice (r.header key)) keyNotFoundString
let form key (r: HttpRequest) = defaultArg (Option.ofChoice (r.form ^^ key)) keyNotFoundString

let testGet handler =
    [
        {
            method = GET;
            route = "/";
            handler = (fun r -> handler r |> OK)
        }
    ]


[<TestCase>]
let ``Synchronous GET call is invoked immediately``() =
    
    //use server = query "test" |> testGet |> serve
    use server = (fun r -> r.rawQuery) |> testGet |> serve

    http {  GET (url @"?test=Hallo")
    }
    |> toText
    |> should equal "test=Hallo"

    server.Dispose()


// http {  GET @"http://www.google.com"
//         AcceptLanguage "de-DE"
// }
// |> send


// http {  POST @"https://reqres.in/api/users"
//         CacheControl "no-cache"
           
//         body
//         json """
//         {
//             "name": "morpheus",
//             "job": "leader"
//         }
//         """
// } 
// |> send

// http {  GET "https://reqres.in/api/users"
// }
// |> send
// |> toJson
// |> test
// >>= byExample IgnoreIndexes Subset
//     """
//     {
//         "page": 1,
//         "data": [
//             { "id": 1 }
//         ]
//     }
//     """
// >>= expect *> fun json -> json?Data.AsArray() |> should haveLength 2
// |> eval


module ``Tests for FsHttp``

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

open FsHttp

[<TestMethod>]
let ``Synchronous GET call is invoked immediately`` =
    http {  GET @"https://reqres.in/api/users?page=2&delay=3"
    }
    |> toJson
    //>>> async {
    //    return 4
    //}
    //>> send



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

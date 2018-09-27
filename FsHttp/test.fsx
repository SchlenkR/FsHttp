
#r @"C:\Users\ronal\.nuget\packages\fsharp.data\2.4.6\lib\net45\FSharp.Data.dll"
#load "./bin/debug/netstandard2.0/FsHttp.fsx"

open FsHttp


http {  GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> send


http {  GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> send


http {  GET @"http://www.google.com"
        AcceptLanguage "de-DE"
}
|> send


http {  POST @"https://reqres.in/api/users"
        CacheControl "no-cache"
           
        body
        json """
        {
            "name": "morpheus",
            "job": "leader"
        }
        """
} 
|> send

http {  GET "https://reqres.in/api/users"
}
|> send
|> toJson
|> test
>>= byExample IgnoreIndexes Subset
    """
    {
        "page": 1,
        "data": [
            { "id": 1 }
        ]
    }
    """
>>= expect *> fun json -> json?Data.AsArray() |> should haveLength 2
|> eval

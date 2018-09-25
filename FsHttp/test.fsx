
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

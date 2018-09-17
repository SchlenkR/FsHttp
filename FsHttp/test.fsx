
#load "FsHttp.fsx" 
// #r "netstandard"
// #r "System.Net.Http"
// #r "./bin/Debug/netstandard2.0/FsHttp.dll"

open FsHttp


http {  GET @"https://www.google.de"
}
|> send |> preview


http {  GET @"https://reqres.in/api/users?page=2&delay=3"
}
|> send


http {
GET @"https://reqres.in/api/users
        ?page=2
        &delay=3"
} |> run


http {

POST @"https://reqres.in/api/users"
CacheControl "no-cache"
   
body
json """
{
"name": "morpheus",
"job": "leader"
}
"""

} |> run


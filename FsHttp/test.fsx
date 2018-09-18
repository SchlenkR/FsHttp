
#load "FsHttp.fsx" 

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


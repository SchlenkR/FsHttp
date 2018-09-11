
#r "netstandard"
#r "System.Net.Http"
#r "./bin/Debug/netstandard2.0/FsHttp.dll"

open FsHttp


http {
GET @"https://reqres.in/api/users?page=2&delay=3"
} |> run


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


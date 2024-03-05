
#r "../src/FsHttp/bin/Debug/net6.0/FsHttp.dll"

open FsHttp

let httpWithAuth =
    http {
        AuthorizationBearer "sdfsdffsdf"
    }

httpWithAuth {
    GET "https://jsonplaceholder.typicode.com/todos/1"
}
|> Request.send

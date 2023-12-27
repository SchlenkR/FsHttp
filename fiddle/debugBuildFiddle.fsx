
#r "../src/RestInPeace/bin/Debug/net8.0/RestInPeace.dll"

open RestInPeace

let httpWithAuth =
    http {
        AuthorizationBearer "sdfsdffsdf"
    }

httpWithAuth {
    GET "https://jsonplaceholder.typicode.com/todos/1"
}
|> Request.send

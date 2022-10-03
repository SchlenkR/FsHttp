#r "../FsHttp/bin/Debug/net6.0/FsHttp.dll"

open FsHttp

let weather = [||]

http {
    POST $"https://api.telegram.org/bot_apiTelegramKey/sendPhoto"

    multipart
    part (ContentData.ByteArrayContent weather) (Some "image/jpeg") "photo"
}
|> Request.send

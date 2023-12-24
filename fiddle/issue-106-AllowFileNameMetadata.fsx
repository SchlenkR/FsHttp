#r "../RestInPeace/bin/Debug/net8.0/RestInPeace.dll"

open RestInPeace

let weather = [||]

http {
    POST $"https://api.telegram.org/bot_apiTelegramKey/sendPhoto"

    multipart
    part (ContentData.BinaryContent weather) (Some "image/jpeg") "photo"
}
|> Request.send

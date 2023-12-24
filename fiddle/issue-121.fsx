
#r "../src/RestInPeace/bin/debug/net7.0/RestInPeace.dll"

open System
open System.Net.Http
open RestInPeace

do Fsi.disableDebugLogs()

http {
    GET "https://www.wikipedia.de"
}
|> Request.sendAsync

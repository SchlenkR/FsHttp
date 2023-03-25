
#r "../src/FsHttp/bin/debug/net7.0/fshttp.dll"

open System
open System.Net.Http
open FsHttp

do Fsi.disableDebugLogs()

http {
    GET "https://www.wikipedia.de"
}
|> Request.sendAsync

#r "../src/FsHttp/bin/debug/net7.0/fshttp.dll"

open FsHttp

http {
    POST "http://localhost"

    multipart
    // part (ContentData.ByteArrayContent [| byte 0xff |]) (Some "image/jpeg") "theFieldName"

    // goal of today
    part (ContentData.ByteArrayContent [| byte 0xff |]) (Some "image/jpeg") "theFieldName" "theFileName"
    
}
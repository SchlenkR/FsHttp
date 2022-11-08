// dotnet fsi with SDK 5 and FsHttp version 8.0.1 will work
#r "nuget: FsHttp, 8.0.1"

open FsHttp
open FsHttp.DslCE
open FsHttp.Operators

% get "https://www.google.de"
|> Response.toText
|> String.length

// dotnet fsi with SDK 5 and RestInPeace version 8.0.0 will hang when making the request below!
#r "nuget: RestInPeace, 8.0.0"

open RestInPeace
open RestInPeace.DslCE
open RestInPeace.Operators

% get "https://www.google.de"
|> Response.toText
|> String.length

// dotnet fsi with SDK 5 and RestInPeace version 8.0.1 will work
#r "nuget: RestInPeace, 8.0.1"

open RestInPeace
open RestInPeace.DslCE
open RestInPeace.Operators

% get "https://www.google.de"
|> Response.toText
|> String.length

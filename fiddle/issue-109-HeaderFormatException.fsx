#r "nuget: RestInPeace"

open RestInPeace

http {
    POST "https://api.jspm.io/generate"
    body
    json """
    {
        "install": [ "lodash" ],
        "env": [ "browser", "module" ],
        "graph":true,
        "provider": "jspm"
    }
    """
}
|> Request.send
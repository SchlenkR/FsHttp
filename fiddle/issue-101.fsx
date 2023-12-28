#r "nuget: FsHttp"
open FsHttp

let getString url rjson = async {
    let! response =
        // See also: https://schlenkr.github.io/FsHttp/Migrations.html
        // 'httpAsync' is replaced by 'http { ... } |> Request.sendAsync'
        http {
            POST url
            Origin "Web3.fs"
            
            // a) 'ContentType' is an operation only valid
            //    on the reqiest 'body' definition.
            // b) Setting ContentType to 'application/json' is
            //    redundant when "json" operation is used.
            body
            ContentType "application/json"
            json rjson
            
            config_timeoutInSeconds 18.5
        }
        |> Request.sendAsync
    let! responseContent = response |> Response.toTextAsync
    return responseContent
}

// Instead of using 'asyn { ... }' CE, it sometimes is sufficient
// piping async 
let getStringAlternative url rjson =
    http {
        POST url
        Origin "Web3.fs"
            
        body
        json rjson
        ContentType "application/json"
            
        config_timeoutInSeconds 18.5
    }
    |> Request.sendAsync
    // use 'Async.await' that works like 'bind':
    |> Async.await Response.toTextAsync
    // use 'Async.map' to transform results in an async context:
    |> Async.map (fun text -> text.ToUpperInvariant())

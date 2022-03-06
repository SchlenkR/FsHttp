#r "nuget: FsHttp"

open FsHttp
open FsHttp.DslCE

http {
    POST "https://reqres.in/api/users"
    CacheControl "no-cache"
    body
    json """
    {
        "name": "morpheus",
        "job": "leader"
    }
    """
}

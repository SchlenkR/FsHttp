#r "./bin/Debug/net6.0/FsHttp.dll"
open FsHttp


// ---------
// Builder
// ---------


type HttpBuilder() =
    member _.Delay(f: unit -> _) = f ()

    member _.Zero() = id
    member _.Yield(t: HeaderContext -> HeaderContext) = t

    member _.Yield(t: HeaderContext -> BodyContext) = t
    member _.Yield(t: BodyContext -> BodyContext) = t

    // define header after header
    member _.Combine(outer: HeaderContext -> HeaderContext, inner: HeaderContext -> HeaderContext) : HeaderContext -> HeaderContext =
        fun hc -> inner (outer hc)
    // transition from header to body (using the "body" function)
    member _.Combine(outer: HeaderContext -> HeaderContext, inner: HeaderContext -> BodyContext) : HeaderContext -> BodyContext =
        fun hc -> inner (outer hc)
    // temp. transition "body" to "json"
    member _.Combine(outer: HeaderContext -> BodyContext, inner: BodyContext -> BodyContext) : HeaderContext -> BodyContext =
        fun hc -> inner (outer hc)
    // define body after body
    member _.Combine(outer: BodyContext -> BodyContext, inner: BodyContext -> BodyContext) : BodyContext -> BodyContext =
        fun hc -> inner (outer hc)
    
    member _.Run(t: HeaderContext -> HeaderContext) =
        t (HeaderContext.create ())
    member _.Run(t: HeaderContext -> BodyContext) =
        t (HeaderContext.create ())


let http = HttpBuilder()


[<AutoOpen>]
module Methods =
    let GET url ctx = HeaderContext.setUrl HttpMethods.get url ctx
    let PUT url ctx = HeaderContext.setUrl HttpMethods.put url ctx
    let POST url ctx = HeaderContext.setUrl HttpMethods.post url ctx
    let DELETE url ctx = HeaderContext.setUrl HttpMethods.delete url ctx
    let OPTIONS url ctx = HeaderContext.setUrl HttpMethods.options url ctx
    let HEAD url ctx = HeaderContext.setUrl HttpMethods.head url ctx
    let TRACE url ctx = HeaderContext.setUrl HttpMethods.trace url ctx
    let CONNECT url ctx = HeaderContext.setUrl HttpMethods.connect url ctx
    let PATCH url ctx = HeaderContext.setUrl HttpMethods.patch url ctx


[<AutoOpen>]
module Header =
    /// List of acceptable human languages for response
    let AcceptLanguage language ctx =
        Header.acceptLanguage language ctx

    /// Authorization credentials for HTTP authorization
    let Authorization credentials ctx =
        Header.authorization credentials ctx


[<AutoOpen>]
module Body =
    let body (ctx: HeaderContext) =
        (ctx :> IToBodyContext).ToBodyContext()

    let json jsonString (ctx: BodyContext) =
        Body.json jsonString ctx






let res =
    http {
        GET "http://www.pxl-clock.com"
        AcceptLanguage "en"
        Authorization "credOuter"
        if true then
            Authorization "credInner"

        body
        json """ { name: "Hans" } """
    }

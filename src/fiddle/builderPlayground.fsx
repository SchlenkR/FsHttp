
module A =
    type StartingContext() = class end

    type HeaderContext = { 
        url: string
        method: string
        headers: (string * string) list }

    type StartingContext with
        member this.Yield(_) = this

        [<CustomOperation("GET")>]
        member this.Get(_: StartingContext, url) = { url = url; method = "GET"; headers = [] }


    type HeaderContext with
        member this.Yield(_) = this

        [<CustomOperation("Accept")>]
        member this.Accept(context, contentType) = this


    let http = StartingContext()

    http {
        GET "xxx"
        Accept "text/json"
    }

module B =
    type HeaderContext = { 
        url: string
        method: string
        headers: (string * string) list }

    type HeaderContext with
        member this.Yield(_) = this

        [<CustomOperation("GET")>]
        member this.Get(_: HeaderContext, url) = { url = url; method = "GET"; headers = [] }

        [<CustomOperation("Accept")>]
        member this.Accept(context, contentType) = this


    let http = { url = ""; method = "GET"; headers = [] }

    http {
        GET "xxx"
        Accept "text/json"
    }



module C =

    [<AutoOpen>]
    module BuilderExtensions =
        type IBuilder<'implementor> = interface end

    type StartingContext() =
        interface IBuilder<StartingContext>

    type HeaderContext = { 
        url: string
        method: string
        headers: (string * string) list } with
        interface IBuilder<HeaderContext>

    type IBuilder<'implementor> with
        member this.Yield(_) = this
    
    type StartingContext with
        [<CustomOperation("GET")>]
        member this.Get(context: IBuilder<StartingContext>, url) =
            { url = url; method = "GET"; headers = [] }

    type IBuilder<'implementor> with
        [<CustomOperation("Accept")>]
        member this.Accept(context: IBuilder<HeaderContext>, contentType) =
            context

    let http = StartingContext()

    http {
        GET "xxx"
        Accept "text/json"
    }

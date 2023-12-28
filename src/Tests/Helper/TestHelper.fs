[<AutoOpen>]
module FsHttp.Tests.TestHelper

open System.Text

open Suave
open Suave.Utils.Collections
open FsUnit

let assertionExn (msg: string) =
    let otype =
        [
            "Xunit.Sdk.XunitException, xunit.assert"
            "NUnit.Framework.AssertionException, nunit.framework"
            "Expecto.AssertException, expecto"
        ]
        |> List.tryPick (System.Type.GetType >> Option.ofObj)

    match otype with
    | None -> failwith msg
    | Some t ->
        let ctor = t.GetConstructor [| typeof<string> |]
        ctor.Invoke [| msg |] :?> exn

let joinLines lines = String.concat "\n" lines
let keyNotFoundString = "KEY_NOT_FOUND"
let query key (r: HttpRequest) = defaultArg (Option.ofChoice (r.query ^^ key)) keyNotFoundString
let header key (r: HttpRequest) = defaultArg (Option.ofChoice (r.header key)) keyNotFoundString
let form key (r: HttpRequest) = defaultArg (Option.ofChoice (r.form ^^ key)) keyNotFoundString
let contentText (r: HttpRequest) = r.rawForm |> Encoding.UTF8.GetString

let shouldEqual (a: 'a) (b: 'b) = a |> should equal b
let shouldNotEquals (a: 'a) (b: 'b) = a |> should not' (equal b)

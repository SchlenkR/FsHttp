module FsHttp.Tests.TestHelper

open System.Text

open Suave
open Suave.Utils.Collections

let joinLines lines = String.concat "\n" lines
let keyNotFoundString = "KEY_NOT_FOUND"
let query key (r: HttpRequest) = defaultArg (Option.ofChoice (r.query ^^ key)) keyNotFoundString
let header key (r: HttpRequest) = defaultArg (Option.ofChoice (r.header key)) keyNotFoundString
let form key (r: HttpRequest) = defaultArg (Option.ofChoice (r.form ^^ key)) keyNotFoundString
let text (r: HttpRequest) = r.rawForm |> Encoding.UTF8.GetString

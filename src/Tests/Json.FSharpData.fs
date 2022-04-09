module FsHttp.Tests.Json.FSharpData

open System
open FSharp.Data
open FsHttp.FSharpData
open FsUnit
open NUnit.Framework


let [<TestCase>] ``To JSON``() =
    
    let referenceJson = """ { "a": "aValue", "b": 12 } """
    let expectedJson  = """ { "a": "aValue" } """

    referenceJson
    |> JsonValue.Parse
    |> Json.expectJsonSubset expectedJson
    |> ignore

    (fun () -> 
        referenceJson
        |> JsonValue.Parse
        |> Json.expectJsonExact expectedJson
        |> ignore)
    |> should throw typeof<Exception>
module FsHttp.Tests.``Json Comparison``

open FSharp.Data
open FsHttp
open FsUnit
open NUnit.Framework


let [<TestCase>] ``Simple property as subset``() =
    
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
    |> shouldFail

let [<TestCase>] ``Property names are case sensitive``() =
    
    let referenceJson = """ { "a": "aValue", "b": 12 } """
    let expectedJson  = """ { "A": "aValue" } """

    (fun () -> 
        referenceJson
        |> JsonValue.Parse
        |> Json.assertJsonSubset expectedJson
        |> ignore
    )
    |> shouldFail

    (fun () -> 
        referenceJson 
        |> JsonValue.Parse
        |> Json.assertJsonExact expectedJson
        |> ignore
    )
    |> shouldFail

let [<TestCase>] ``Property values are case sensitive``() =
    
    let referenceJson = """ { "a": "aValue", "b": 12 } """
    let expectedJson  = """ { "a": "AValue" } """

    (fun () -> 
        referenceJson
        |> JsonValue.Parse
        |> Json.assertJsonSubset expectedJson
        |> ignore
    )
    |> shouldFail

    (fun () -> 
        referenceJson 
        |> JsonValue.Parse
        |> Json.assertJsonExact expectedJson
        |> ignore
    )
    |> shouldFail
    
let [<TestCase>] ``Arrays``() =
    
    let referenceJson =  """ [ 1, 2, 3, 4, 5 ] """
    
    referenceJson
    |> JsonValue.Parse
    |> Json.assertJsonSubset """ [ 2, 3, 1 ] """
    |> ignore
    
    (fun () -> 
        referenceJson 
        |> JsonValue.Parse
        |> Json.assertJson RespectOrder Subset """ [ 2, 3, 1 ] """
        |> ignore)
    |> shouldFail
    
    referenceJson 
    |> JsonValue.Parse
    |> Json.assertJson RespectOrder Subset """ [ 1, 2, 3 ] """
    |> ignore

    (fun () -> 
        referenceJson 
        |> JsonValue.Parse
        |> Json.assertJsonExact """ [ 2, 3, 1 ] """
        |> ignore
    )
    |> shouldFail
    
    (fun () -> 
        referenceJson 
        |> JsonValue.Parse
        |> Json.assertJson RespectOrder Exact """ [ 2, 3, 1, 5, 4 ] """
        |> ignore
    )
    |> shouldFail
    
    referenceJson 
    |> JsonValue.Parse
    |> Json.assertJson RespectOrder Exact """ [ 1, 2, 3, 4, 5 ] """
    |> ignore
    

let [<TestCase>] ``Exact Match Simple``() =
    
    """ { "a": 1, "b": 2 } """
    |> JsonValue.Parse
    |> Json.assertJsonExact """ { "a": 1, "b": 2 } """
    |> ignore
    
    (fun () -> 
        """ { "a": 1, "b": 2, "c": 3 } """
        |> JsonValue.Parse
        |> Json.assertJsonExact """ { "a": 1, "b": 2 } """
        |> ignore
    )
    |> shouldFail
    
    (fun () -> 
        """ { "a": 1, "b": 2 } """
        |> JsonValue.Parse
        |> Json.assertJsonExact """ { "a": 1, "b": 2, "c": 3 } """
        |> ignore)
    |> shouldFail
    
    """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
    |> JsonValue.Parse
    |> Json.assertJsonExact """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
    |> ignore
    
    """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
    |> JsonValue.Parse
    |> Json.assertJsonExact """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
    |> ignore

let [<TestCase>] ``Exact Match Complex``() =
        
    """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
    |> JsonValue.Parse
    |> Json.assertJsonExact """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
    |> ignore
    
    (fun () -> 
        """ { "a": 1, "b": { "ba": 3, "bb": 4, "bc": 5 } } """
        |> JsonValue.Parse
        |> Json.assertJsonExact """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
        |> ignore
    )
    |> shouldFail
    
    (fun () -> 
        """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
        |> JsonValue.Parse
        |> Json.assertJsonExact """ { "a": 1, "b": { "ba": 3, "bb": 4, "bc": 5 } } """
        |> ignore
    )
    |> shouldFail

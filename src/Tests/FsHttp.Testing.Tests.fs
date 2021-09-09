namespace FsHttp.Tests.``Tests for: FsHttp_Testing``

open FsHttp.Testing
open FsUnit
open NUnit.Framework

open FsHttp.DslCE
open FsHttp.Tests.Server

open Suave.ServerErrors
open Suave.Operators
open Suave.Filters


module ``Json Comparison`` =

    let [<TestCase>] ``Simple property as subset``() =
    
        let referenceJson = """ { "a": "aValue", "b": 12 } """
        let expectedJson  = """ { "a": "aValue" } """

        referenceJson 
        |> jsonStringShouldLookLike IgnoreOrder Subset expectedJson
        |> ignore

        (fun () -> 
            referenceJson 
            |> jsonStringShouldLookLike IgnoreOrder Exact expectedJson
            |> ignore)
        |> shouldFail

    let [<TestCase>] ``Property names are case sensitive``() =
    
        let referenceJson = """ { "a": "aValue", "b": 12 } """
        let expectedJson  = """ { "A": "aValue" } """

        (fun () -> 
            referenceJson 
            |> jsonStringShouldLookLike IgnoreOrder Subset expectedJson
            |> ignore)
        |> shouldFail

        (fun () -> 
            referenceJson 
            |> jsonStringShouldLookLike IgnoreOrder Exact expectedJson
            |> ignore)
        |> shouldFail

    let [<TestCase>] ``Property values are case sensitive``() =
    
        let referenceJson = """ { "a": "aValue", "b": 12 } """
        let expectedJson  = """ { "a": "AValue" } """

        (fun () -> 
            referenceJson
            |> jsonStringShouldLookLike IgnoreOrder Subset expectedJson
            |> ignore)
        |> shouldFail

        (fun () -> 
            referenceJson 
            |> jsonStringShouldLookLike IgnoreOrder Exact expectedJson
            |> ignore)
        |> shouldFail
    
    let [<TestCase>] ``Arrays``() =
    
        let referenceJson =  """ [ 1, 2, 3, 4, 5 ] """
    
        referenceJson
        |> jsonStringShouldLookLike IgnoreOrder Subset """ [ 2, 3, 1 ] """
        |> ignore
    
        (fun () -> 
            referenceJson 
            |> jsonStringShouldLookLike RespectOrder Subset """ [ 2, 3, 1 ] """
            |> ignore)
        |> shouldFail
    
        referenceJson 
        |> jsonStringShouldLookLike RespectOrder Subset """ [ 1, 2, 3 ] """
        |> ignore

        (fun () -> 
            referenceJson 
            |> jsonStringShouldLookLike IgnoreOrder Exact """ [ 2, 3, 1 ] """
            |> ignore)
        |> shouldFail
    
        (fun () -> 
            referenceJson 
            |> jsonStringShouldLookLike RespectOrder Exact """ [ 2, 3, 1, 5, 4 ] """
            |> ignore)
        |> shouldFail
    
        referenceJson 
        |> jsonStringShouldLookLike RespectOrder Exact """ [ 1, 2, 3, 4, 5 ] """
        |> ignore
    

    let [<TestCase>] ``Exact Match Simple``() =
    
        """ { "a": 1, "b": 2 } """
        |> jsonStringShouldLookLike IgnoreOrder Exact """ { "a": 1, "b": 2 } """
        |> ignore
    
        (fun () -> 
            """ { "a": 1, "b": 2, "c": 3 } """
            |> jsonStringShouldLookLike IgnoreOrder Exact """ { "a": 1, "b": 2 } """
            |> ignore)
        |> shouldFail
    
        (fun () -> 
            """ { "a": 1, "b": 2 } """
            |> jsonStringShouldLookLike IgnoreOrder Exact """ { "a": 1, "b": 2, "c": 3 } """
            |> ignore)
        |> shouldFail
    
        """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
        |> jsonStringShouldLookLike IgnoreOrder Exact """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
        |> ignore
    
        """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
        |> jsonStringShouldLookLike IgnoreOrder Exact """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
        |> ignore

    let [<TestCase>] ``Exact Match Complex``() =
        
        """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
        |> jsonStringShouldLookLike IgnoreOrder Exact """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
        |> ignore
    
        (fun () -> 
            """ { "a": 1, "b": { "ba": 3, "bb": 4, "bc": 5 } } """
            |> jsonStringShouldLookLike IgnoreOrder Exact """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
            |> ignore)
        |> shouldFail
    
        (fun () -> 
            """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
            |> jsonStringShouldLookLike IgnoreOrder Exact """ { "a": 1, "b": { "ba": 3, "bb": 4, "bc": 5 } } """
            |> ignore)
        |> shouldFail


module ``Expectations`` =

    // TODO: exactMatch = true

    let [<TestCase>] ``Expect status code``() =
        use server = GET >=> BAD_GATEWAY "" |> serve

        http { GET (url @"") }
        |> statusCodeShouldBe System.Net.HttpStatusCode.BadGateway
        |> ignore

        Assert.Throws<AssertionException>(fun() ->
            http { GET (url @"") }
            |> statusCodeShouldBe System.Net.HttpStatusCode.Ambiguous
            |> ignore
        )
        |> ignore

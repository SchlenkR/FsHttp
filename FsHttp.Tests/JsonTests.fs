
module ``Tests for FsHttp JSON Comparison``

open FsHttp
open FsUnit
open NUnit.Framework

let shouldBe value testResult = testResult |> isOk |> should be value

[<TestCase>]
let ``Simple property as subset``() =
    
    let referenceJson = """ { "a": "aValue", "b": 12 } """
    let expectedJson  = """ { "a": "aValue" } """

    referenceJson 
    |> expectJsonStringByExample IgnoreOrder Subset expectedJson
    |> shouldBe True

    referenceJson 
    |> expectJsonStringByExample IgnoreOrder Exact expectedJson
    |> shouldBe False

[<TestCase>]
let ``Property names are case sensitive``() =
    
    let referenceJson = """ { "a": "aValue", "b": 12 } """
    let expectedJson  = """ { "A": "aValue" } """

    referenceJson 
    |> expectJsonStringByExample IgnoreOrder Subset expectedJson
    |> shouldBe False

    referenceJson 
    |> expectJsonStringByExample IgnoreOrder Exact expectedJson
    |> shouldBe False

[<TestCase>]
let ``Property values are case sensitive``() =
    
    let referenceJson = """ { "a": "aValue", "b": 12 } """
    let expectedJson  = """ { "a": "AValue" } """

    referenceJson
    |> expectJsonStringByExample IgnoreOrder Subset expectedJson
    |> shouldBe False

    referenceJson 
    |> expectJsonStringByExample IgnoreOrder Exact expectedJson
    |> shouldBe False
    
[<TestCase>]
let ``Arrays``() =
    
    let referenceJson =  """ [ 1, 2, 3, 4, 5 ] """
    
    referenceJson
    |> expectJsonStringByExample IgnoreOrder Subset """ [ 2, 3, 1 ] """
    |> shouldBe True
    
    referenceJson 
    |> expectJsonStringByExample RespectOrder Subset """ [ 2, 3, 1 ] """
    |> shouldBe False
    
    referenceJson 
    |> expectJsonStringByExample RespectOrder Subset """ [ 1, 2, 3 ] """
    |> shouldBe True

    referenceJson 
    |> expectJsonStringByExample IgnoreOrder Exact """ [ 2, 3, 1 ] """
    |> shouldBe False
    
    referenceJson 
    |> expectJsonStringByExample RespectOrder Exact """ [ 2, 3, 1, 5, 4 ] """
    |> shouldBe False
    
    referenceJson 
    |> expectJsonStringByExample RespectOrder Exact """ [ 1, 2, 3, 4, 5 ] """
    |> shouldBe True
    

[<TestCase>]
let ``Exact Match Simple``() =
    
    """ { "a": 1, "b": 2 } """
    |> expectJsonStringByExample IgnoreOrder Exact """ { "a": 1, "b": 2 } """
    |> shouldBe True
    
    """ { "a": 1, "b": 2, "c": 3 } """
    |> expectJsonStringByExample IgnoreOrder Exact """ { "a": 1, "b": 2 } """
    |> shouldBe False
    
    """ { "a": 1, "b": 2 } """
    |> expectJsonStringByExample IgnoreOrder Exact """ { "a": 1, "b": 2, "c": 3 } """
    |> shouldBe False
    
    """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
    |> expectJsonStringByExample IgnoreOrder Exact """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
    |> shouldBe True
    
    """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
    |> expectJsonStringByExample IgnoreOrder Exact """ { "a": 1, "b": { "ba": 3, "bb": 4} } """
    |> shouldBe True

[<TestCase>]
let ``Exact Match Complex``() =
        
    """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
    |> expectJsonStringByExample IgnoreOrder Exact """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
    |> shouldBe True
    
    """ { "a": 1, "b": { "ba": 3, "bb": 4, "bc": 5 } } """
    |> expectJsonStringByExample IgnoreOrder Exact """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
    |> shouldBe False
    
    """ { "a": 1, "b": { "ba": 3, "bb": 4 } } """
    |> expectJsonStringByExample IgnoreOrder Exact """ { "a": 1, "b": { "ba": 3, "bb": 4, "bc": 5 } } """
    |> shouldBe False


// TODO: exactMatch = true
module FsHttp.Tests.Helper

open System
open System.IO
open System.Text

open FsUnit
open FsHttp.Helper
open FsHttp.Tests

open NUnit.Framework

[<TestCase>]
let ``URL combine`` () =
    let a = "http://xxx.com"
    let b = "sub"
    let expectedUrl = $"{a}/{b}"

    Url.combine $"{a}" $"{b}" |> should equal expectedUrl
    Url.combine $"{a}/" $"{b}" |> should equal expectedUrl
    Url.combine $"{a}" $"/{b}" |> should equal expectedUrl
    Url.combine $"{a}/" $"/{b}" |> should equal expectedUrl
    Url.combine $"{a}/" $"/{b}/" |> should equal expectedUrl

[<TestCase>]
let ``Stream ReadUtf8StringAsync`` () =

    let text = "a😉b🙁🙂d"

    let test len (expected: string) =
        let res =
            new MemoryStream(Encoding.UTF8.GetBytes(text))
            |> Stream.readUtf8StringAsync len
            |> Async.RunSynchronously
        let s1 = Encoding.UTF8.GetBytes res |> Array.toList
        let s2 = Encoding.UTF8.GetBytes expected |> Array.toList
        let res = (s1 = s2)
        if not res then
            printfn ""
            printfn "count = %d" len
            printfn "expected = %s" expected
            printfn ""
            printfn "Expected: %A" s2
            printfn ""
            printfn "Actual  : %A" s1
            printfn ""
            printfn " ----------------------------"
        res |> should equal true

    test 0 ""
    test 1 "a"
    test 2 "a😉"
    test 3 "a😉b"
    test 4 "a😉b🙁"
    test 5 "a😉b🙁🙂"
    test 6 "a😉b🙁🙂d"
    test 100 "a😉b🙁🙂d"

// TODO: Test other helper functions

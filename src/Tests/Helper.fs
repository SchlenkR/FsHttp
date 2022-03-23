module FsHttp.Tests.Helper

open System
open System.IO
open System.Text

open FsUnit
open FsHttp.Helper
open FsHttp.Tests

open NUnit.Framework

let [<TestCase>] ``URL combine``() =
    let a = "http://xxx.com"
    let b = "sub"
    let expectedUrl = $"{a}/{b}"
    
    Url.combine $"{a}" $"{b}" |> should equal expectedUrl
    Url.combine $"{a}/" $"{b}" |> should equal expectedUrl
    Url.combine $"{a}" $"/{b}" |> should equal expectedUrl
    Url.combine $"{a}/" $"/{b}" |> should equal expectedUrl
    Url.combine $"{a}/" $"/{b}/" |> should equal expectedUrl

let [<TestCase>] ``Stream ReadUtf8StringAsync``() =

    let text = "a😉b🙁🙂d"
    let read len =
        new MemoryStream(Encoding.UTF8.GetBytes(text))
        |> Stream.readUtf8StringAsync len
        |> Async.RunSynchronously

    read 0 |> shouldEqual ""
    read 1 |> shouldEqual "a"
    read 2 |> shouldEqual "a"
    read 3 |> shouldEqual "a😉"
    read 4 |> shouldEqual "a😉b"
    read 5 |> shouldEqual "a😉b"
    read 6 |> shouldEqual "a😉b🙁"
    read 7 |> shouldEqual "a😉b🙁"
    read 8 |> shouldEqual "a😉b🙁🙂"
    read 9 |> shouldEqual "a😉b🙁🙂d"
    read 100 |> shouldEqual "a😉b🙁🙂d"

let private testUtf8StringBufferingStream limit =
    let text = "abcdefghijklmnop"
    let bs = new Stream.Utf8StringBufferingStream(
        new MemoryStream(Encoding.UTF8.GetBytes(text)),
        limit)
    let sr = new StreamReader(bs)
    do sr.ReadToEnd() |> ignore
    let expectation = match limit with Some limit -> text.Substring(0, limit) | _ -> text
    bs.GetUtf8String() |> shouldEqual expectation

let [<TestCase>] ``Stream Utf8StringBufferingStream with limit``() =
    testUtf8StringBufferingStream (Some 2)

let [<TestCase>] ``Stream Utf8StringBufferingStream no limit``() =
    testUtf8StringBufferingStream None

// TODO: Test other helper functions

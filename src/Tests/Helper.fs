module FsHttp.Tests.Helper

open System

open FsUnit
open FsHttp.Helper

open NUnit.Framework

let a = "http://xxx.com"
let b = "sub"
let expectedUrl = $"{a}/{b}"

let [<TestCase>] ``URL combine``() =
    Url.combine $"{a}" $"{b}" |> should equal expectedUrl
    Url.combine $"{a}/" $"{b}" |> should equal expectedUrl
    Url.combine $"{a}" $"/{b}" |> should equal expectedUrl
    Url.combine $"{a}/" $"/{b}" |> should equal expectedUrl
    Url.combine $"{a}/" $"/{b}/" |> should equal expectedUrl


// TODO: Test other helper functions

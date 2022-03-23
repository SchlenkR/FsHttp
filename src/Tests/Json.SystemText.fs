module FsHttp.Tests.Json.SystemText

open FsUnit
open FsHttp
open FsHttp.Tests.TestHelper
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful

type Person = { name: string; age: int }

let returnBody () = POST >=> request (contentText >> OK) |> serve

let [<TestCase>] ``Serialize / Deserialize JSON object``() =
    use server = returnBody()

    let person = { name = "John Doe"; age = 34 }

    http {
        POST (url "")
        body
        jsonSerialize person
    }
    |> Request.send
    |> Response.deserializeJson<Person>
    |> shouldEqual person

let [<TestCase>] ``To JSON and dynamic operator``() =
    use server = returnBody()

    let jsonString = """
    {
        "name": "John Doe",
        "age": 34
    }
    """

    let json =
        http {
            POST (url "")
            body
            json jsonString
        }
        |> Request.send
        |> Response.toJson
    
    json?name.GetString() |> should equal "John Doe"
    json?age.GetInt32() |> should equal 34

let [<TestCase>] ``To JSON array``() =
    use server = returnBody()

    let jsonString = """
    [
        {
            "name": "John Doe",
            "age": 34
        },
        {
            "name": "Foo Bar",
            "age": 99
        }
    ]
    """

    http {
        POST (url "")
        body
        json jsonString
    }
    |> Request.send
    |> Response.toJsonSeq
    |> Seq.map (fun json -> json?name.GetString())
    |> Seq.toList
    |> shouldEqual [ "John Doe"; "Foo Bar" ]

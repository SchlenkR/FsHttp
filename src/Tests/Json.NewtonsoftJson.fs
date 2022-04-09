module FsHttp.Tests.Json.NewtonsoftJson

open FsUnit
open FsHttp
open FsHttp.Tests.TestHelper
open FsHttp.Tests.Server
open FsHttp.NewtonsoftJson

open Newtonsoft.Json.Linq
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
    json?name.ToObject<string>() |> should equal "John Doe"
    json?age.ToObject<int>() |> should equal 34

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
    |> Seq.map (fun json -> json?name.ToObject<string>())
    |> Seq.toList
    |> shouldEqual [ "John Doe"; "Foo Bar" ]

let [<TestCase>] ``Unicode chars``() =
    use server = returnBody()

    let name = "John+Doe"

    http {
        POST (url "")
        body
        json (sprintf """
            {
                "name": "%s"
            }
        """ name)
    }
    |> Request.send
    |> Response.toJson
    |> fun json -> json?name.ToObject<string>()
    |> Seq.toList
    |> shouldEqual name

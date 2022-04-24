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

open System.Text.Json
open System.Text.Json.Serialization

let returnBody () = POST >=> request (contentText >> OK) |> serve

type Person = { name: string; age: int }
type SuperPerson = { name: string; age: int; address: string option }

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

let [<TestCase>] ``Serialize / Deserialize JSON object with Tarmil``() =
    use server = returnBody()

    FsHttp.GlobalConfig.Json.defaultJsonSerializerOptions <-
        let options = JsonSerializerOptions()
        options.Converters.Add(JsonFSharpConverter())
        options

    let person1 = { name = "John Doe"; age = 34; address = Some "Whereever" }
    let person2 = { name = "Bryan Adams"; age = 55; address = None }
    let payload = [ person1; person2 ]
   
    http {
        POST (url "")
        body
        jsonSerialize payload
    }
    |> Request.send
    |> Response.deserializeJson<SuperPerson list>
    |> shouldEqual payload

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
    |> fun json -> json?name.GetString()
    |> Seq.toList
    |> shouldEqual name

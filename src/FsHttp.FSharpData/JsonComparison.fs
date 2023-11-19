namespace FsHttp.FSharpData

open System
open FsHttp.Helper
open FSharp.Data

[<AutoOpen>]
module JsonComparisonTypes =
    type ArrayComparison =
        | RespectOrder
        | IgnoreOrder

    type StructuralComparison =
        | Subset
        | Exact

module Json =
    let compareJson (arrayComparison: ArrayComparison) (expectedJson: JsonValue) (resultJson: JsonValue) =
        let rec toPaths (currentPath: string) (jsonValue: JsonValue) : ((string * obj) list) =
            match jsonValue with
            | JsonValue.Null -> [ currentPath, null :> obj ]
            | JsonValue.Record properties ->
                seq {
                    for pName, pValue in properties do
                        for innerPath in toPaths (sprintf "%s/%s" currentPath pName) pValue do
                            yield innerPath
                }
                |> Seq.toList
            | JsonValue.Array values ->
                let indexedValues = values |> Array.mapi (fun i x -> i, x)

                seq {
                    for index, value in indexedValues do
                        let printedIndex =
                            match arrayComparison with
                            | RespectOrder -> index.ToString()
                            | IgnoreOrder -> ""

                        for inner in toPaths (sprintf "%s[%s]" currentPath printedIndex) value do
                            yield inner
                }
                |> Seq.toList
            | JsonValue.Boolean b -> [ currentPath, b :> obj ]
            | JsonValue.Float f -> [ currentPath, f :> obj ]
            | JsonValue.String s -> [ currentPath, s :> obj ]
            | JsonValue.Number n -> [ currentPath, n :> obj ]

        let getPaths x = x |> toPaths "" |> List.map (fun (path, value) -> sprintf "%s{%A}" path value)
        (getPaths expectedJson, getPaths resultJson)

    let expectJson
        (arrayComparison: ArrayComparison)
        (structuralComparison: StructuralComparison)
        (expectedJson: string)
        (actualJsonValue: JsonValue)
        =
        let expectedPaths, resultPaths =
            compareJson arrayComparison (JsonValue.Parse expectedJson) actualJsonValue

        let aggregateUnmatchedElements list =
            match list with
            | [] -> ""
            | x :: xs -> xs |> List.fold (fun curr next -> curr + "\n" + next) x

        match structuralComparison with
        | Subset ->
            let eMinusR = expectedPaths |> List.except resultPaths

            match eMinusR with
            | [] -> Ok actualJsonValue
            | _ -> Error(sprintf "Elements not contained in source: \n%s" (eMinusR |> aggregateUnmatchedElements))
        | Exact ->
            let eMinusR = expectedPaths |> List.except resultPaths
            let rMinusE = resultPaths |> List.except expectedPaths

            match eMinusR, rMinusE with
            | [], [] -> Ok actualJsonValue
            | _ ->
                let a1 =
                    (sprintf "Elements not contained in source: \n%s" (eMinusR |> aggregateUnmatchedElements))

                let a2 =
                    (sprintf "Elements not contained in expectation: \n%s" (rMinusE |> aggregateUnmatchedElements))

                Error(sprintf "%s\n%s" a1 a2)

    let expectJsonSubset (expectedJson: string) (actualJsonValue: JsonValue) =
        expectJson IgnoreOrder Subset expectedJson actualJsonValue

    let expectJsonExact (expectedJson: string) (actualJsonValue: JsonValue) =
        expectJson IgnoreOrder Exact expectedJson actualJsonValue
        |> Result.getValueOrThrow Exception


    // -----------
    // Assert
    // -----------
    let assertJson
        (arrayComparison: ArrayComparison)
        (structuralComparison: StructuralComparison)
        (expectedJson: string)
        (actualJsonValue: JsonValue)
        =
        expectJson arrayComparison structuralComparison expectedJson actualJsonValue
        |> Result.getValueOrThrow Exception

    let assertJsonSubset (expectedJson: string) (actualJsonValue: JsonValue) =
        assertJson IgnoreOrder Subset expectedJson actualJsonValue

    let assertJsonExact (expectedJson: string) (actualJsonValue: JsonValue) =
        assertJson IgnoreOrder Exact expectedJson actualJsonValue

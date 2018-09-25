
[<AutoOpen>]
module FsHttp.Testing

open FSharp.Data

// useful to support lambdas without parenthesis:
// ... |> testJson *> fun json -> json.Properties.Length |> should be (greaterThan 5)
let ( *> ) f x = f x

// TODO: try

type TestResult<'a> =
    | Ok of 'a
    | Failed of 'a * message: string

let test x = Ok x

let eval t =
    match t with
    | Ok x -> "Ok"
    | Failed (x,y) -> sprintf "Failed: %s" y

let expect f x =
    try
        f x |> ignore
        Ok x
    with
    | ex -> Failed (x,ex.Message)

// let private assertExpectation (f: FsHttp.Response -> _) (r: FsHttp.Response) =
//     try
//         f r |> ignore
//         Ok r
//     with
//     | ex -> Failed (r,ex.Message)
// let testString (f: string -> 'a) =
//     assertExpectation (FsHttp.contentAsString >> f)
// let testJson (f: JsonValue -> 'a) =
//     assertExpectation (toJson >> f)
// let testJsonArray (f: JsonValue[] -> 'a) =
//     assertExpectation (toJson >> (fun json -> json.AsArray()) >> f)

type ArrayComparison = | UseIndex | IgnoreIndexes
type StructuralComparison = | Subset | Exact

let bind m f =
    match m with
    | Ok x -> f x
    | Failed (x,y) -> Failed (x,y)
let (>>=) = bind

let compareJson (arrayComparison: ArrayComparison) (expectedJson: JsonValue) (resultJson: JsonValue) =

    let rec toPaths (currentPath: string) (jsonValue: JsonValue) : ((string*obj) list) =
        match jsonValue with
        | JsonValue.Null -> [currentPath, null :> obj]
        | JsonValue.Record properties ->
            seq {
                for pName, pValue in properties do
                for innerPath in toPaths (sprintf "%s/%s" currentPath pName) pValue do
                yield innerPath
            } |> Seq.toList
        | JsonValue.Array values ->
            let indexedValues = values |> Array.mapi (fun i x -> i,x)
            seq {
                for index,value in indexedValues do
                let printedIndex = match arrayComparison with | UseIndex -> index.ToString() | IgnoreIndexes -> ""
                for inner in toPaths (sprintf "%s[%s]" currentPath printedIndex) value do
                yield inner
            } |> Seq.toList
        | JsonValue.Boolean b -> [currentPath, b :> obj]
        | JsonValue.Float f -> [currentPath, f :> obj]
        | JsonValue.String s -> [currentPath, s :> obj]
        | JsonValue.Number n -> [currentPath, n :> obj]

    let getPaths x = x |> toPaths "" |> List.map (fun (path,value) -> sprintf "%s{%A}" path value)
    (getPaths expectedJson, getPaths resultJson)

let byExample
        (arrayComparison: ArrayComparison)
        (structuralComparison: StructuralComparison)
        (expectedJson: string)
        (resultJson: JsonValue) =
    let expectedPaths,resultPaths = compareJson arrayComparison (JsonValue.Parse expectedJson) resultJson
    let aggregateUnmatchedElements list =
        match list with
        | [] -> ""
        | x::xs -> xs |> List.fold (fun curr next -> curr + "\n" + next) x
    match structuralComparison with
    | Subset ->
        let eMinusR = expectedPaths |> List.except resultPaths
        match eMinusR with
        | [] -> Ok resultJson
        | _ -> Failed (resultJson, sprintf "Elements not contained in source: \n%s" (eMinusR |> aggregateUnmatchedElements))
    | Exact ->
        let eMinusR = expectedPaths |> List.except resultPaths
        let rMinusE = resultPaths |> List.except expectedPaths
        match eMinusR, rMinusE with
        | [],[] -> Ok resultJson
        | _ ->
            let a1 = (sprintf "Elements not contained in source: \n%s" (eMinusR |> aggregateUnmatchedElements))
            let a2 = (sprintf "Elements not contained in expectation: \n%s" (rMinusE |> aggregateUnmatchedElements))
            Failed (resultJson, a1 + "\n" + a2)

type JsonValue with
    member this.HasProperty(propertyName: string) =
        let prop = this.TryGetProperty propertyName
        match prop with
        | Some _ -> true
        | None -> false



// let json1 = """
// {
//     "name": "Name 1",
//     "id": "Id 1",
//     "child": {
//         "nameOfChild": "Name 2",
//         "idOfChild": "Id 2",
//         "items": [
//             {
//                 "item1Prop1": "Hallo",
//                 "item1Prop2": "Welt"
//             },
//             2,
//             null
//         ]
//     }
// }
// """


// json1 |> testJsonStringByExample IgnoreIndexes Exact """
// {
//     "name": "Name 1",
//     "id": "Id 1",
//     "child": {
//         "nameOfChild": "Name 2",
//         "idOfChild": "Id 2",
//         "items": [
//             {
//                 "item1Prop1": "Hallo",
//                 "item1Prop2": "Welt",
//                 "item1Prop3": "Welt"
//             },
//             2,
//             null
//         ]
//     }
// }
// """

// #if INTERACTIVE

// open Microsoft.FSharp.Reflection

// fsi.AddPrinter
//     (fun (x: obj) ->
//         if x <> null && x.GetType().ReflectedType.Name = typeof<TestResult<_>>.Name then

//         match tr with
//         | Ok _ -> "Ok"
//         | Failed(a,b) -> sprintf "Failed: %s" b
//         | _ -> null
//     )

// #endif


// let cases = typeof<TestResult<_>>.Name
// (Ok 1).GetType().ReflectedType.Name
// (Failed (1,"j")).GetType().ReflectedType.Name
// (Failed (1,"j")).GetType().Name

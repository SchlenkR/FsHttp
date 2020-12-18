
//#load "./packages/docu/FSharp.Formatting/FSharp.Formatting.fsx"

//open FSharp.Literate
//open System.IO

//let source = __SOURCE_DIRECTORY__

//let script = Path.Combine(source, "./src/Docu/Demo.DslCE.fsx")
//let md = Literate.ParseScriptFile(script).MarkdownDocument

open System
open System.IO

let source = __SOURCE_DIRECTORY__
let script = Path.Combine(source, "./src/Docu/Demo.DslCE.fsx")

type Mode 
    = Markdown
    | Code

let taggedLines =
    [
        let mutable currentMode = Markdown

        for line in File.ReadAllLines script do
            let trimmedLine = line.Trim()
            let isMarkdownStart = trimmedLine = "(**"
            let isMarkdownEnd = trimmedLine = "*)"

            match currentMode, isMarkdownStart, isMarkdownEnd with
            | Markdown, _, true ->
                yield currentMode, ""
                yield currentMode, "```fsharp"
                currentMode <- Code
            | Code, true, _ ->
                currentMode <- Markdown
                yield  currentMode, "```"
            | _, true,_
            | _, _, true ->
                ()
            | _ ->
                yield currentMode, line
    ]

let linesWhereTrailingEmptyCodeLinesRemoved =
    [
        let mutable lastMode = Markdown

        for mode,line in taggedLines |> List.rev do
            match mode, lastMode, line.Trim() with
            | Code, Code, _ ->
                yield line
            | Code, _, ""->
                ()
            | _ ->
                lastMode <- mode
                yield line
    ]
    |> List.rev
//let linesWhereTrailingEmptyCodeLinesRemoved = taggedLines |> List.map (fun (_,x) -> x)

let readme = Path.Combine(source, "./Readme.md")
File.WriteAllLines(readme, linesWhereTrailingEmptyCodeLinesRemoved)


//#load "./packages/docu/FSharp.Formatting/FSharp.Formatting.fsx"

//open FSharp.Literate
//open System.IO

//let source = __SOURCE_DIRECTORY__

//let script = Path.Combine(source, "./src/Samples/Demo.DslCE.fsx")
//let md = Literate.ParseScriptFile(script).MarkdownDocument

open System.IO

let source = __SOURCE_DIRECTORY__
let script = Path.Combine(source, "./src/Samples/Demo.DslCE.fsx")

type Mode = Start | Markdown | Code

let lines =
    [
        let mutable mode = Start
        for line in File.ReadAllLines script do
            let isMarkdownStart = line.Trim() = "(**"
            let isMarkdownEnd = line.Trim() = "*)"
            match mode, isMarkdownStart, isMarkdownEnd with
            | Start, true, _ ->
                mode <- Markdown
            | Markdown, _, true ->
                mode <- Code
                yield "```fsharp"
            | Code, true, _ ->
                mode <- Markdown
                yield "```"
            | _ ->
                yield line
    ]

let readme = Path.Combine(source, "./Readme.md")
File.WriteAllLines(readme, lines)

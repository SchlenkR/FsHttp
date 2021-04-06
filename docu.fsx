open System
open System.IO

type Mode 
    = Markdown
    | Code

let taggedLines script =
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

let linesWhereTrailingEmptyCodeLinesRemoved script =
    [
        let mutable lastMode = Markdown

        for mode,line in taggedLines script |> List.rev do
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

let generate () =
    let source = __SOURCE_DIRECTORY__
    let script = Path.Combine(source, "./src/Docu/Demo.DslCE.fsx")
    let readme = Path.Combine(source, "./Readme.md")
    File.WriteAllLines(readme, linesWhereTrailingEmptyCodeLinesRemoved script)

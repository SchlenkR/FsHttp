module FsxProcessor =
    open System.IO

    type Mode = Markdown | Code

    type Directive =
        MarkdownStart
        | MarkdownEnd
        | HideStart
        | HideEnd

    let openCodeTag = "{% highlight fsharp %}"
    let closeCodeTag = "{% endhighlight %}"

    let private taggedLines script =

        [
            let mutable currentMode = Markdown
            let mutable isHidden = false

            for line in File.ReadAllLines script do
                let trimmedLine = line.Trim()

                let directive = 
                    match trimmedLine with
                    | "(**" -> Some MarkdownStart
                    | "*)" -> Some MarkdownEnd
                    | _ -> None

                let wasHidden = isHidden
                isHidden <-
                    match trimmedLine with
                    | "(** hide **)" -> true
                    | "(** unhide **)" -> false
                    | _ -> isHidden
                let isCurrentlyHidden = wasHidden || isHidden

                match currentMode, directive with
                | Markdown, Some MarkdownEnd ->
                    yield currentMode, isCurrentlyHidden, false, ""
                    yield currentMode, isCurrentlyHidden, false, openCodeTag
                    currentMode <- Code
                | Code, Some MarkdownStart ->
                    currentMode <- Markdown
                    yield  currentMode, isCurrentlyHidden, false, closeCodeTag
                    yield  currentMode, isCurrentlyHidden, false, ""
                | _, Some _ ->
                    ()
                | _ ->
                    yield currentMode, isCurrentlyHidden, false, line
            
            // last line
            yield currentMode, isHidden, true, ""
        ]

    let processScriptFile script =
        [
            let mutable lastMode = Markdown

            for mode,isHidden,isLast,line in taggedLines script |> List.rev do
                match isLast,isHidden,mode,lastMode,line.Trim() with
                | true, _, Code, _, _ -> 
                    yield closeCodeTag
                | _, true, _, _, _ ->
                    ()
                | _, _, Code, Code, _ ->
                    yield line
                | _, _, Code, _, ""->
                    ()
                | _ ->
                    lastMode <- mode
                    yield line
        ]
        |> List.rev

open System.IO

let generate () =

    let source = __SOURCE_DIRECTORY__
    let input = Path.Combine(source, "./src/Docu")
    let output = Path.Combine(source, "./docs")

    do // remove all .md files from docs
        Directory.EnumerateFiles(output, "*.md") |> Seq.iter File.Delete

    do // copy all .md files to docs
        Directory.EnumerateFiles(input, "*.md")
        |> Seq.iter (fun f -> File.Copy(f, Path.Combine(output, Path.GetFileName(f))))
    
    do // process all fsx script (-> .md) files and copy them to docs
        Directory.EnumerateFiles(input, "*.fsx")
        |> Seq.iter (fun script ->
            let readme = Path.Combine(output, Path.GetFileNameWithoutExtension(script) + ".md")
            File.WriteAllLines(readme, FsxProcessor.processScriptFile script)
        )

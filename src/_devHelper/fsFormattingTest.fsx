#r "nuget: FSharp.Formatting"

module ApiDocs =
    open FSharp.Formatting.ApiDocs
    open System.IO

    let root = Path.Combine(__SOURCE_DIRECTORY__, "../FsHttp/bin/Debug/net5.0/publish")
    let file = Path.Combine(root, "FsHttp.dll")
    let input = ApiDocInput.FromFile(file)

    ApiDocs.GenerateHtml
        ( [ input ],
          root = root,
          libDirs = [ root ],
          output = Path.Combine(root, "output"),
          collectionName = "YourLibrary",
          substitutions = [])

    //ApiDocs.GenerateHtml
    //    ( [ input ],
    //      output = Path.Combine(root, "output"),
    //      collectionName = "YourLibrary",
    //      template = Path.Combine(root, "templates", "template.html"),
    //      substitutions = [])


module Literate =

    open FSharp.Formatting.Literate
    open System.IO

    let root = __SOURCE_DIRECTORY__
    let file = Path.Combine(root, "readme.fsx")

    let res =
        Literate.ParseScriptString
            ( File.ReadAllText file
            )

    Literate.ConvertScriptFile
        ( input = file,
          outputKind = OutputKind.Md
          )

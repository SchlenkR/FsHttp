
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

#r "nuget: Fake.Core.Process"
#r "nuget: Fake.IO.FileSystem"

open System

open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators

Trace.trace $"Starting script..."

module Properties =
    let nugetServer = "https://api.nuget.org/v3/index.json"
    let nugetPushEnvVarName = "nuget_push"

[<AutoOpen>]
module Helper =

    let private runTarget (x: string * _) =
        let name,f = x
        Trace.trace $"Running task: {name}"
        f ()
    
    let run targets =
        for t in targets do
            runTarget t
    
    type Shell with
        static member ExecSuccess (cmd: string, ?args: string, ?dir: string) =
            let res = Shell.Exec(cmd, ?args = args, ?dir = dir)
            if res <> 0 then failwith $"Shell execute was not successful: {res}" else ()

    type Args() =
        let singleArg = fsi.CommandLineArgs.[1..] |> Array.tryExactlyOne
        let mutable switches : string list = []
        member this.hasArg arg =
            switches <- arg :: switches
            singleArg |> Option.map (fun a -> a = arg) |> Option.defaultValue false
        member this.assertArgs() =
            match singleArg with
            | None ->
                let switches = switches |> String.concat "|"
                let msg = $"USAGE: dotnet fsi build.fsx [{switches}]"
                printfn "%s" msg
                Environment.Exit -1
            | _ -> ()

let args = Args()
let shallBuild = args.hasArg "build"
let shallTest = args.hasArg "test"
let shallPublish = args.hasArg "publish"
let shallPack = args.hasArg "pack"
let shallFormat = args.hasArg "format"

do args.assertArgs()

let clean = "clean", fun () ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ ".pack"
    |> Shell.cleanDirs 

let slnPath = "./src/FsHttp.sln"

let build = "build", fun () ->
    Shell.ExecSuccess ("dotnet", $"build {slnPath}")

let test = "test", fun () ->
    Shell.ExecSuccess ("dotnet", $"test {slnPath}")

let pack = "pack", fun () ->
    !! "src/**/FsHttp*.fsproj"
    |> Seq.iter (fun p ->
        Trace.trace $"SourceDir is: {__SOURCE_DIRECTORY__}"
        Shell.ExecSuccess ("dotnet", sprintf "pack %s -o %s -c Release" p (Path.combine __SOURCE_DIRECTORY__ ".pack"))
    )

let format = "format", fun () ->
    Shell.ExecSuccess ("dotnet", $"fantomas ./src/FsHttp/ ./src/FsHttp.Testing/ ./src/Tests/")

// TODO: git tag + release
let publish = "publish", fun () ->
    let nugetApiKey = Environment.environVar Properties.nugetPushEnvVarName
    !! ".pack/*.nupkg"
    |> Seq.iter (fun p ->
        Shell.ExecSuccess ("dotnet", $"nuget push {p} -k {nugetApiKey} -s {Properties.nugetServer} --skip-duplicate")
    )

run [
    clean

    if shallBuild then
        build
    if shallTest then
        test
    if shallPack then
        pack
    if shallPublish then
        build
        pack
        publish
    if shallFormat then
        format
]

Trace.trace $"Finished script..."

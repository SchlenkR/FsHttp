
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

    type Args() =
        let taskName,taskArgs =
            match fsi.CommandLineArgs |> Array.toList with
            | fsi :: taskName :: taskArgs -> taskName, taskArgs
            | _ -> 
                let msg = $"Wrong args. Expected: fsi :: taskName :: taskArgs"
                printfn "%s" msg
                Environment.Exit -1
                failwith msg
        member _.IsTask(arg) = taskName = arg
        member _.TaskArgs = taskArgs

let args = Args()

type Shell with
    static member ExecSuccess (cmd: string, ?arg: string) =
        let args = arg |> Option.defaultValue "" |> fun x -> [| x; yield! args.TaskArgs |] |> String.concat " "
        printfn $"Executing command '{cmd}' with args: {args}"
        let res = Shell.Exec(cmd, ?args = Some args)
        if res <> 0 then failwith $"Shell execute was not successful: {res}" else ()

let shallBuild = args.IsTask("build")
let shallTest = args.IsTask("test")
let shallPublish = args.IsTask("publish")
let shallPack = args.IsTask("pack")
let shallFormat = args.IsTask("format")

let toolRestore = "toolRestore", fun () ->
    Shell.ExecSuccess ("dotnet", "tool restore")

let clean = "clean", fun () ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ ".pack"
    |> Shell.cleanDirs 

let checkFormat = "checkformat", fun () ->
    Shell.ExecSuccess ("dotnet", $"fantomas --check ./src/FsHttp/ ./src/Tests/")

let slnPath = "./src/FsHttp.sln"
let build = "build", fun () ->
    Shell.ExecSuccess ("dotnet", $"build {slnPath}")


let CsTestProjectPath = "./src/Test.CSharp/Test.CSharp.csproj"
let FsharpTestProjectPath = "./src/Tests/Tests.fsproj"
let test = "test", fun () ->
    Shell.ExecSuccess ("dotnet", $"test {FsharpTestProjectPath}")
    Shell.ExecSuccess ("dotnet", $"test {CsTestProjectPath}")

let pack = "pack", fun () ->
    !! "src/**/FsHttp*.fsproj"
    |> Seq.iter (fun p ->
        Trace.trace $"SourceDir is: {__SOURCE_DIRECTORY__}"
        Shell.ExecSuccess ("dotnet", sprintf "pack %s -o %s -c Release" p (Path.combine __SOURCE_DIRECTORY__ ".pack"))
    )

let format = "format", fun () ->
    Shell.ExecSuccess ("dotnet", $"fantomas ./src/FsHttp/ ./src/Tests/")

// TODO: git tag + release
let publish = "publish", fun () ->
    let nugetApiKey = Environment.environVar Properties.nugetPushEnvVarName
    !! ".pack/*.nupkg"
    |> Seq.iter (fun p ->
        Shell.ExecSuccess ("dotnet", $"nuget push {p} -k {nugetApiKey} -s {Properties.nugetServer} --skip-duplicate")
    )

run [
    clean
    toolRestore

    if shallBuild then
        checkFormat
        build
    if shallTest then
        test
    if shallPack then
        pack
    if shallPublish then
        checkFormat
        build
        pack
        publish
    if shallFormat then
        format
]

Trace.trace $"Finished script..."

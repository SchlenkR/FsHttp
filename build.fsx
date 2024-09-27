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
    let nugetPushEnvVarName = "nuget_fshttp"

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
        let strippedArgs = 
            fsi.CommandLineArgs 
            |> Array.skipWhile (fun x -> x <> __SOURCE_FILE__ ) 
            |> Array.skip 1
            |> Array.toList
        let taskName,taskArgs =
            match strippedArgs with
            | taskName :: taskArgs -> taskName, taskArgs
            | _ -> 
                let msg = $"Wrong args. Expected: fsi :: taskName :: taskArgs"
                printfn "%s" msg
                Environment.Exit -1
                failwith msg
        do
            printfn $"Task name: {taskName}"
            printfn $"Task args: {taskArgs}"
        member _.IsTask(arg) =
            let res = taskName = arg
            printfn $"Checking task '{arg}'... {res} (taskName: '{taskName}')"
            res
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

let toolRestore = "toolRestore", fun () ->
    Shell.ExecSuccess ("dotnet", "tool restore")

let clean = "clean", fun () ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ ".pack"
    |> Shell.cleanDirs 

let slnPath = "./FsHttp.sln"
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
        build
    if shallTest then
        test
    if shallPack then
        pack
    if shallPublish then
        build
        pack
        publish
]

Trace.trace $"Finished script..."

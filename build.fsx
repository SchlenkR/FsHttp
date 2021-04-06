
System.Environment.CurrentDirectory <- __SOURCE_DIRECTORY__

#load "./properties.fsx"
#load "./docu.fsx"

#r "nuget: Fake.Core.Process"
#r "nuget: Fake.IO.FileSystem"

open System

open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators

Trace.trace $"Starting script..."

[<AutoOpen>]
module Helper =

    let private runTarget (x: string * _) =
        let name,f = x
        Trace.trace $"Running task: {name}"
        f ()
    
    let run targets =
        for t in targets do
            runTarget t
    
    let hasArg arg = fsi.CommandLineArgs.[1..].[0]  = arg
    
    let always = true

    type Shell with
        static member ExecSuccess (cmd: string, ?args: string, ?dir: string) =
            let res = Shell.Exec(cmd, ?args = args, ?dir = dir)
            if res <> 0 then failwith $"Shell execute was not successful: {res}" else ()

let shallDocu = hasArg "docu"
let shallBuild = hasArg "build"
let shallTest = hasArg "test"
let shallPublish = hasArg "publish"
let shallPack = hasArg "pack"

let clean = "clean", fun () ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ ".pack"
    |> Shell.cleanDirs 

let docu = "docu", fun () ->
    Docu.generate()

let slnPath = "./src/FsHttp.sln"

let build = "build", fun () ->
    let config = if shallPublish then "Release" else "Debug  "
    Shell.ExecSuccess ("dotnet", $"build {slnPath} -c {config}")

let test = "test", fun () ->
    Shell.ExecSuccess ("dotnet", $"test {slnPath}")

let pack = "pack", fun () ->
    !! "src/**/FsHttp*.fsproj"
    |> Seq.iter (fun p ->
        // let packageVersion = { version with (*Patch = 4711u;*) Original = None; PreRelease = PreRelease.TryParse "alpha" }.AsString
        Trace.trace (sprintf "SourceDir is: %s" __SOURCE_DIRECTORY__)
        Shell.ExecSuccess ("dotnet", sprintf "pack %s -o %s -c Release" p (Path.combine __SOURCE_DIRECTORY__ ".pack"))
    )

// TODO: git tag + release
let publish = "publish", fun () ->
    let nugetApiKey = Environment.environVar Properties.nugetPushEnvVarName
    !! ".pack/*.nupkg"
    |> Seq.iter (fun p ->
        Trace.tracefn "------ pushing: %s" p
        Shell.ExecSuccess ("dotnet", sprintf "nuget push %s -k %s -s %s" p nugetApiKey Properties.nugetServer)
    )

run [
    if shallDocu then
        docu
    else
        clean
        if shallBuild then
            build
        if shallTest then
            test
        if shallPack then
            docu
            pack
        if shallPublish then
            docu
            pack
            publish
]

Trace.trace $"Finished script..."

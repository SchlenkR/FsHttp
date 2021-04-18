
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
let shallDocu = args.hasArg "docu"
let shallBuild = args.hasArg "build"
let shallTest = args.hasArg "test"
let shallPublish = args.hasArg "publish"
let shallPack = args.hasArg "pack"

do args.assertArgs()

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

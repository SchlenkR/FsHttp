
#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.DotNet.NuGet
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"

#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.DotNet.NuGet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

#load "./Properties.fsx"

let assertSuccess i = if i <> 0 then failwith "Shell execute was not successful." else ()

let args = Target.getArguments() |> Option.defaultValue [||]

let test = args |> Array.contains "--test"
let publish = args |> Array.contains "--publish"

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ ".pack"
    |> Shell.cleanDirs 
)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (
        DotNet.build (fun opt ->
            { opt with
                Configuration =
                    if publish
                    then DotNet.BuildConfiguration.Release
                    else DotNet.BuildConfiguration.Debug }
    ))
)

Target.create "Test" (fun _ ->
    !! "src/**/*Tests.fsproj"
    |> Seq.iter (fun p ->
        Shell.Exec ("dotnet", sprintf "test %s --no-build" p) |> assertSuccess)
)

Target.create "Pack" (fun _ ->
    !! "src/**/FsHttp*.fsproj"
    |> Seq.iter (fun p ->
        // let packageVersion = { version with (*Patch = 4711u;*) Original = None; PreRelease = PreRelease.TryParse "alpha" }.AsString

        Trace.trace (sprintf "SourceDir is: %s" __SOURCE_DIRECTORY__)
        Shell.Exec ("dotnet", sprintf "pack %s -o %s --no-build -c Release" p (Path.combine __SOURCE_DIRECTORY__ ".pack"))
        |> assertSuccess
    )
)

Target.create "Publish" (fun _ ->
    let nugetApiKey = Environment.environVar Properties.nugetPushEnvVarName
    !! ".pack/*.nupkg"
    |> Seq.iter (fun p ->
        Trace.tracefn "------ pushing: %s" p
        Shell.Exec ("dotnet", sprintf "nuget push %s -k %s -s %s" p nugetApiKey Properties.nugetServer)
        |> assertSuccess
    )

    // TODO: git tag + release

    // setPackageVersion { packageVersion with (*Patch = 4711u;*) Original = None; PreRelease = PreRelease.TryParse "alpha" }.AsString
    // setPackageVersion { packageVersion with Minor = packageVersion.Minor + 1u; Original = None }.AsString
)

Target.create "Final" ignore

"Clean"
    ==> "Build"
    =?> ("Test", publish || test)
    =?> ("Pack", publish)
    =?> ("Publish", publish)
    ==> "Final"

Target.runOrDefaultWithArguments "Final"


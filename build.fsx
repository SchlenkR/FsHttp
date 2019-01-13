
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

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ ".pack"
    |> Shell.cleanDirs 
)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build (fun opt -> 
        { opt with
            Configuration = DotNet.BuildConfiguration.Debug }
    ))
)

Target.create "BuildOnly" ignore

let versionFileName = "./packageVersion"
let packageVersion = File.readAsString versionFileName |> SemVer.parse
let setPackageVersion version = File.writeString false versionFileName version

Target.create "Pack" (fun _ ->
    !! "src/**/FsHttp*.fsproj"
    |> Seq.iter (fun p ->
        // let packageVersion = { version with (*Patch = 4711u;*) Original = None; PreRelease = PreRelease.TryParse "alpha" }.AsString

        Shell.Exec ("dotnet", sprintf "pack %s -o %s -p:PackageVersion=%s" p (Path.combine __SOURCE_DIRECTORY__ ".pack") packageVersion.AsString)
        |> assertSuccess
    )
)

Target.create "Test" (fun _ ->
    !! "src/**/*Tests.fsproj"
    |> Seq.iter (DotNet.test id)
)

Target.create "Publish" (fun _ ->
    let nugetApiKey = Environment.environVar Properties.nugetPushEnvVarName
    !! ".pack/*.nupkg"
    |> Seq.iter (fun p ->
        Trace.tracefn "------ pushing: %s" p
        Shell.Exec ("dotnet", sprintf "nuget push %s -k %s -s %s" p nugetApiKey Properties.nugetServer)
        |> assertSuccess
    )

    // setPackageVersion { packageVersion with (*Patch = 4711u;*) Original = None; PreRelease = PreRelease.TryParse "alpha" }.AsString
    setPackageVersion { packageVersion with Minor = packageVersion.Minor + 1u; Original = None }.AsString
)

"Clean"
    ==> "Build"
    ==> "BuildOnly"

"Clean"
    ==> "Test"
    ==> "Pack"
    ==> "Publish"


Target.runOrDefault "BuildOnly"

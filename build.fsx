// include Fake lib
#r @"./packages/FAKE/tools/FakeLib.dll"

open Fake

RestorePackages()

// Directories
let buildAppDir = "./build/app/"
let appSrcDir = "./WebToEpub.Website/"

Target "CleanApp" (fun _ -> CleanDir buildAppDir)

Target "BuildApp" (fun _ -> 
    !!(appSrcDir + "**/*.fsproj")
    |> MSBuildRelease buildAppDir "Build"
    |> Log "AppBuild-Output: ")

// Dependencies
"CleanApp" ==> "BuildApp"

// start build
RunTargetOrDefault "BuildApp"

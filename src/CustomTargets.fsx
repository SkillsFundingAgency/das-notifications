open Fake

let testDirectory = getBuildParamOrDefault "buildMode" "Debug"
let nUnitRunner = "nunit3-console.exe"
let mutable nUnitToolPath = @"tools\NUnit.ConsoleRunner\"
let acceptanceTestPlayList = getBuildParamOrDefault "playList" ""
let nunitTestFormat = getBuildParamOrDefault "nunitTestFormat" "nunit2"

Target "Dotnet Restore" (fun _ ->
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.Notifications.Api.Client" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.Notifications.Api.Types" })
    DotNetCli.Restore(fun p ->
        { p with
                Project = ".\\SFA.DAS.Notifications.Messages" })
)
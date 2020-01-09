#load "pipeline/setup.cake"
// #load "pipeline/build.cake"
// #load "pipeline/test.cake"
// #load "pipeline/release.cake"

string target = Argument("target", "Default");

Task("Post-Setup")
    .Does<BuildInfo>(info =>
{
    info.SolutionFile = "src/MyProject.sln";
});

Task("Post-Build")
    .Does<BuildInfo>(info =>
{
    Information("Post build is running!");
    // Copy Yarhl.Media for the integration tests
    // EnsureDirectoryExists($"src/Yarhl.IntegrationTests/{netBinDir}/Plugins");
    // CopyFileToDirectory(
    //     $"src/Yarhl.Media/{netstandardBinDir}/Yarhl.Media.dll",
    //     $"src/Yarhl.IntegrationTests/{netBinDir}/Plugins");

    // EnsureDirectoryExists($"src/Yarhl.IntegrationTests/{netcoreBinDir}/Plugins");
    // CopyFileToDirectory(
    //     $"src/Yarhl.Media/{netstandardBinDir}/Yarhl.Media.dll",
    //     $"src/Yarhl.IntegrationTests/{netcoreBinDir}/Plugins");
});

Task("Default")
    .IsDependentOn("Post-Setup")
    .IsDependentOn("Set-Version");
    // .IsDependentOn("Build")
    // .IsDependentOn("Post-Build");

RunTarget(target);

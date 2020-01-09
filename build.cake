#load "pipeline/setup.cake"
#load "pipeline/build.cake"
// #load "pipeline/test.cake"
// #load "pipeline/release.cake"

string target = Argument("target", "Default");

Task("Fill-BuildInfo")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.LibraryProjects = new[] { "MyLibrary" };
    info.ApplicationProjects = new[] { "MyConsole" };
    info.TestProjects = new string[0];
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
    .IsDependentOn("Fill-BuildInfo")
    .IsDependentOn("Build")
    .IsDependentOn("Pack");

RunTarget(target);

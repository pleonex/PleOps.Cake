#load "pipeline/setup.cake"
#load "pipeline/build.cake"
#load "pipeline/test.cake"
// #load "pipeline/release.cake"

string target = Argument("target", "Default");

Task("Fill-BuildInfo")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.AddLibraryProjects("MyLibrary");
    info.AddApplicationProjects("MyConsole");
    info.AddTestProjects("MyTests");
});


Task("Post-Build")
    .Does<BuildInfo>(info =>
{
    Information("Post build ran");
});

Task("Build-Test")
    .IsDependentOn("Fill-BuildInfo")
    .IsDependentOn("Build")
    .IsDependentOn("Post-Build")
    .IsDependentOn("Test");

Task("Prepare-Release")
    .IsDependentOn("Build-Test")
    .IsDependentOn("Pack-Libs")
    .IsDependentOn("Pack-Apps");

Task("Create-TestRelease")
    .IsDependentOn("Prepare-Release");
    // Push to the test feed

Task("Draft-Release")
    .IsDependentOn("Create-TestRelease");
    // Generate release notes
    // Get build and release links (for release notes)
    // Create draft release in GitHub

Task("Confirm-Release");
    // Get release notes from GitHub
    // Repack NuGet without preview version and with release notes
    // Push to official feed
    // Confirm GitHub release

Task("Default")
    .IsDependentOn("Prepare-Release");

RunTarget(target);

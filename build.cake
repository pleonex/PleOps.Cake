#load "pipeline/setup.cake"
#load "pipeline/build.cake"
#load "pipeline/test.cake"
#load "pipeline/release.cake"

string target = Argument("target", "Default");

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.AddLibraryProjects("MyLibrary");
    info.AddApplicationProjects("MyConsole");
    info.AddTestProjects("MyTests");
});

Task("Build-Test")
    .IsDependentOn("Define-Project")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("Prepare-Release")
    .IsDependentOn("Build-Test")
    // Generate release notes
    // Build doc
    .IsDependentOn("Pack-Libs")
    .IsDependentOn("Pack-Apps");

Task("Create-PreviewRelease")
    .IsDependentOn("Prepare-Release")
    .IsDependentOn("Publish-NuGetTestFeed");
    // Push docs to preview version
    // Create a git tag for the release

Task("Promote-Release")
    // Repack NuGet without preview version
    .IsDependentOn("Publish-NuGetReleaseFeed");
    // Push docs as new version
    // Create GitHub release / Create a tag

Task("Default")
    .IsDependentOn("Prepare-Release");

RunTarget(target);

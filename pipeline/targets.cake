#load "setup.cake"
#load "build.cake"
#load "documentation.cake"
#load "test.cake"
#load "release.cake"

Task("Build-Test")
    .IsDependentOn("Define-Project")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("Create-Artifacts")
    .IsDependentOn("Build-Test")
    // Generate release notes
    // .IsDependentOn("Build-Docs")
    .IsDependentOn("Pack-Libs")
    .IsDependentOn("Pack-Apps");

Task("Create-PreviewRelease")
    .IsDependentOn("Create-Artifacts")
    .IsDependentOn("Publish-NuGetTestFeed");
    // Push apps into the Azure DevOps test feed
    // Push docs to preview version
    // Create a git tag for the release

Task("Promote-Release")
    // Repack NuGet without preview version
    .IsDependentOn("Publish-NuGetReleaseFeed");
    // Push docs as new version
    // Create GitHub release / Create a tag

Task("Serve-Doc")
    .IsDependentOn("Build-ServeDoc");

Task("Default")
    .IsDependentOn("Create-Artifacts");

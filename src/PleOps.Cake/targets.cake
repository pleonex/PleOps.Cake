#load "setup.cake"
#load "build.cake"
#load "documentation.cake"
#load "test.cake"
#load "release.cake"

Task("BuildTest")
    .IsDependentOn("Define-Project") // Must be defined by the user
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("Create-Artifacts")
    .IsDependentOn("BuildTest")
    .IsDependentOn("Build-Doc")
    .IsDependentOn("Pack-Libs")
    .IsDependentOn("Pack-Apps");

Task("Create-PreviewRelease")
    // Generate release notes
    .IsDependentOn("Publish-NuGetTestFeed");
    // Push apps into the Azure DevOps test feed
    // Push docs to preview version

Task("Create-OfficialRelease")
    // Generate release notes
    .IsDependentOn("Publish-NuGetReleaseFeed");
    // Push docs as new version
    // Create GitHub release

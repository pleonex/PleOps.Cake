#load "setup.cake"
#load "build.cake"
#load "documentation.cake"
#load "test.cake"
#load "release.cake"

Task("Build-Test")
    .IsDependentOn("Define-Project") // Must be defined by the user
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("Create-Artifacts")
    // Generate release notes
    .IsDependentOn("Build-Test")
    .IsDependentOn("Build-Doc")
    .IsDependentOn("Pack-Libs")
    .IsDependentOn("Pack-Apps");

Task("Create-PreviewRelease")
    .IsDependentOn("Publish-NuGetTestFeed");
    // Push apps into the Azure DevOps test feed
    // Push docs to preview version

Task("Promote-Release")
    .IsDependentOn("Publish-NuGetReleaseFeed");
    // Push docs as new version
    // Create GitHub release
    // Create a tag

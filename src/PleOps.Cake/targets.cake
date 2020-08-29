#load "setup.cake"
#load "build.cake"
#load "documentation.cake"
#load "test.cake"
#load "release.cake"

Task("BuildTest")
    .IsDependentOn("Define-Project") // Must be defined by the user
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("Stage-Artifacts")
    .IsDependentOn("BuildTest")
    .IsDependentOn("Build-Doc")
    .IsDependentOn("Pack-Libs")
    .IsDependentOn("Pack-Apps");

Task("Create-PreviewRelease")
    // Generate release notes
    // Update preview documentation
    // Push docs

Task("Create-OfficialRelease")
    // Generate release notes
    // Create new version for documentation
    // Push docs
    // Create GitHub release with artifacts

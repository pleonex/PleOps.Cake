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
    .IsDependentOn("Create-GitHubDraftRelease") // only preview builds
    .IsDependentOn("Export-GitHubReleaseNotes") // only preview and stable builds
    .IsDependentOn("Build-Doc")
    // Update preview documentation
    // Create new version for stable documentation
    .IsDependentOn("Pack-Libs")
    .IsDependentOn("Pack-Apps");

Task("Update-DocBranch");
    // Push docs

#load "setup.cake"
#load "build.cake"
#load "documentation.cake"
#load "test.cake"
#load "releasenotes.cake"

Task("BuildTest")
    .IsDependentOn("Define-Project") // Must be defined by the user
    .IsDependentOn("Show-Info")
    .IsDependentOn("Build")
    .IsDependentOn("Test");

Task("Generate-ReleaseNotes")
    .IsDependentOn("Define-Project") // Must be defined by the user
    .IsDependentOn("Show-Info")
    .IsDependentOn("Create-GitHubDraftRelease")     // only preview builds
    .IsDependentOn("Export-GitHubReleaseNotes");    // only preview and stable builds

Task("Stage-Artifacts")
    .IsDependentOn("BuildTest")
    .IsDependentOn("Build-Doc")
    .IsDependentOn("Pack-Libs")
    .IsDependentOn("Pack-Apps");

Task("Push-Artifacts")
    .IsDependentOn("Define-Project") // Must be defined by the user
    .IsDependentOn("Show-Info")
    .IsDependentOn("Push-NuGets")   // only preview and stable builds
    .IsDependentOn("Push-Apps")     // only stable builds
    .IsDependentOn("Push-Doc")      // only preview and stable builds
    .IsDependentOn("Close-GitHubMilestone");    // only stable builds

#load "setup.cake"

#tool "nuget:?package=gitreleasemanager&version=0.9.0"
#tool "nuget:?package=GitReleaseNotes&version=0.7.1"

Task("Create-ReleaseNotes")
    .Description("Create the release notes")
    .Does<BuildInfo>(info =>
{

});

Task("Create-GitHubRelease")
    .Description("Create a draft release in GitHub")
    .Does<BuildInfo>(info =>
{

});

#tool "nuget:?package=gitreleasemanager&version=0.9.0"
#tool "nuget:?package=GitReleaseNotes&version=0.7.1"

Task("Create-ReleaseNotes")
    .Description("Create the release notes")
    .Does<BuildInfo>(info =>
{

});

Task("Create-GitHubDraftRelease")
    .Description("Create a draft release in GitHub")
    .Does<BuildInfo>(info =>
{

});

Task("Confirm-GitHubRelease")
    .Description("Promote draft release to official")
    .Does<BuildInfo>(info =>
{

});

Task("Publish-NuGetTestFeed")
    .Description("Publish the NuGet artifacts to the test feed")
    .Does<BuildInfo>(info =>
{
    var settings = new DotNetCoreNuGetPushSettings {
        Source = "TestFeed",
        ApiKey = "key", // it doesn't matter for Azure DevOps
    };
    DotNetCoreNuGetPush(System.IO.Path.Combine("artifacts", "*.nupkg"), settings);
});

Task("Publish-NuGetReleaseFeed")
    .Description("Publish the NuGet artifacts to the release feed")
    .Does<BuildInfo>(info =>
{
    var settings = new DotNetCoreNuGetPushSettings {
        Source = "https://api.nuget.org/v3/index.json",
        ApiKey = Environment.GetEnvironmentVariable("NUGET_KEY"),
    };
    DotNetCoreNuGetPush(System.IO.Path.Combine("artifacts", "*.nupkg"), settings);
});

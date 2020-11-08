#load "setup.cake"

#tool "nuget:?package=gitreleasemanager&version=0.11.0"

Task("Create-GitHubDraftRelease")
    .Description("Create a GitHub draft release with release notes")
    .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType == BuildType.Preview)
    .Does<BuildInfo>(info =>
{
    if (string.IsNullOrEmpty(info.GitHubToken)) {
        throw new Exception("Missing GitHub token");
    }

    // Create or update draft release
    var createOptions = new GitReleaseManagerCreateSettings {
        Milestone = info.WorkMilestone,
    };
    GitReleaseManagerCreate(
        info.GitHubToken,
        "SceneGate",
        "Yarhl",
        createOptions);
});

Task("Export-GitHubReleaseNotes")
    .Description("Export all the release notes from GitHub into a file")
    .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType != BuildType.Development)
    .Does<BuildInfo>(info =>
{
    if (string.IsNullOrEmpty(info.GitHubToken)) {
        throw new Exception("Missing GitHub token");
    }

    GitReleaseManagerExport(
        info.GitHubToken,
        "SceneGate",
        "Yarhl",
        $"{info.ArtifactsDirectory}/CHANGELOG.md");
});

Task("Add-AssetsToGitHubRelease")
    .Description("Add assets to the GitHub release")
    .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType == BuildType.Stable)
    .Does<BuildInfo>(info =>
{
    if (string.IsNullOrEmpty(info.GitHubToken)) {
        throw new Exception("Missing GitHub token");
    }

    throw new NotImplementedException();
});

#load "setup.cake"

#tool "nuget:?package=gitreleasemanager&version=0.11.0"

Task("Create-GitHubDraftRelease")
    .Description("Create a GitHub draft release with release notes")
    .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType == BuildType.Preview)
    .WithCriteria<BuildInfo>((ctxt, info) => !string.IsNullOrEmpty(info.GitHubToken))
    .Does<BuildInfo>(info =>
{
    // Create or update draft release
    var createOptions = new GitReleaseManagerCreateSettings {
        Milestone = info.WorkMilestone,
    };
    GitReleaseManagerCreate(
        info.GitHubToken,
        info.RepositoryOwner,
        info.RepositoryName,
        createOptions);
});

Task("Export-GitHubReleaseNotes")
    .Description("Export all the release notes from GitHub into a file")
    .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType != BuildType.Development)
    .WithCriteria<BuildInfo>((ctxt, info) => !string.IsNullOrEmpty(info.GitHubToken))
    .Does<BuildInfo>(info =>
{
    if (FileExists(info.ChangelogFile)) {
        Information("Skipping exporting GitHub release notes as file exists");
        return;
    }

    // Export last milestone to embed in apps and NuGets
    string milestone = info.BuildType switch {
        BuildType.Preview => info.WorkMilestone,
        BuildType.Stable => $"v{info.Version}",
        _ => throw new Exception("Unknown build type for milestone"),
    };

    var exportOptions = new GitReleaseManagerExportSettings {
        TagName = milestone,
    };
    GitReleaseManagerExport(
        info.GitHubToken,
        info.RepositoryOwner,
        info.RepositoryName,
        info.ChangelogFile,
        exportOptions);

    // Export full changelog for documentation
    string docsDir = System.IO.Path.GetDirectoryName(info.DocFxFile);
    GitReleaseManagerExport(
        info.GitHubToken,
        info.RepositoryOwner,
        info.RepositoryName,
        $"{docsDir}/dev/Changelog.md");
});

Task("Add-AssetsToGitHubRelease")
    .Description("Add assets to the GitHub release")
    .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType == BuildType.Stable)
    .Does<BuildInfo>(info =>
{
    if (string.IsNullOrEmpty(info.GitHubToken)) {
        throw new Exception("Missing GitHub token");
    }

    // TODO: #15 push assets to GH release
    throw new NotImplementedException();
});

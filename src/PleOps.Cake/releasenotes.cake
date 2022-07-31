#load "setup.cake"
#tool "dotnet:?package=GitReleaseManager.Tool&version=0.13.0"
#addin nuget:?package=Octokit&version=1.0.0

using System.Linq;

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
    // Cannot build for dev builds because GitReleaseManager will fail if there
    // isn't any PR / issue on a milestone closed.
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
        info.ChangelogNextFile,
        exportOptions);

    // Export full changelog for documentation
    string docsDir = System.IO.Path.GetDirectoryName(info.DocFxFile);
    GitReleaseManagerExport(
        info.GitHubToken,
        info.RepositoryOwner,
        info.RepositoryName,
        info.ChangelogFile);
});

Task("Close-GitHubMilestone")
    .Description("Rename vNext milestone and close it. Rename and re-create Future milestone")
    .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType == BuildType.Stable)
    .WithCriteria<BuildInfo>((ctxt, info) => !string.IsNullOrEmpty(info.GitHubToken))
    .Does<BuildInfo>(info =>
{
    string versionName = $"v{info.Version}";

    var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("PleOps.Cake"));
    client.Credentials = new Octokit.Credentials(info.GitHubToken);

    var milestoneClient = client.Issue.Milestone;
    var milestones = milestoneClient.GetAllForRepository(
        info.RepositoryOwner,
        info.RepositoryName).Result;
    if (milestones.Any(m => m.Title == versionName)) {
        Information("Found already a milestone for the current version. Skipping.");
        return;
    }

    var result = RenameMilestone(info, milestoneClient, milestones, info.WorkMilestone, versionName);
    if (result) {
        Information("Closing work milestone");
        GitReleaseManagerClose(
            info.GitHubToken,
            info.RepositoryOwner,
            info.RepositoryName,
            versionName);

        result = RenameMilestone(info, milestoneClient, milestones, info.FutureMilestone, info.WorkMilestone);
        if (result) {
            CreateMilestone(info, milestoneClient, info.FutureMilestone);
        }
    }
});

bool RenameMilestone(BuildInfo info, Octokit.IMilestonesClient client, IEnumerable<Octokit.Milestone> milestones, string currentName, string newName)
{
    var milestone = milestones.FirstOrDefault(m => m.Title == currentName);
    if (milestone == null) {
        Information($"Can't find milestone '{currentName}'. Skipping.");
        return false;
    }

    if (milestone.State != Octokit.ItemState.Open) {
        Information($"Milestone '{currentName} is not open: '{milestone.State}'. Skipping.");
        return false;
    }

    Information($"Renaming milestone '{currentName}' to '{newName}'");
    var renameRequest = new Octokit.MilestoneUpdate {
        Title = newName,
        Description = milestone.Description,
        State = milestone.State.Value,
        DueOn = milestone.DueOn,
    };
    client.Update(
        info.RepositoryOwner,
        info.RepositoryName,
        milestone.Number,
        renameRequest).Wait();
    return true;
}

void CreateMilestone(BuildInfo info, Octokit.IMilestonesClient client, string name)
{
    Information($"Creating milestone '{name}'");
    var request = new Octokit.NewMilestone(name);
    client.Create(
        info.RepositoryOwner,
        info.RepositoryName,
        request).Wait();
}

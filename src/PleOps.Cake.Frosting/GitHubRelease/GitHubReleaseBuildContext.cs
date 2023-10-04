namespace PleOps.Cake.Frosting.GitHubRelease;

using global::Cake.Core;

public class GitHubReleaseBuildContext
{
    public GitHubReleaseBuildContext()
    {
        WorkMilestone = "vNext";
        FutureMilestone = "Future";

        GitHubToken = string.Empty;
        RepositoryOwner = string.Empty;
        RepositoryName = string.Empty;
    }

    public string WorkMilestone { get; set; }

    public string FutureMilestone { get; set; }

    public string RepositoryOwner { get; set; }

    public string RepositoryName { get; set; }

    [LogIgnore]
    public string GitHubToken { get; set; }

    public void ReadArguments(BuildContext context)
    {
        GitHubToken = context.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        FindRepoInfo(context);
    }

    private void FindRepoInfo(ICakeContext context)
    {
        string? repo = context.Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
        string[]? parts = repo?.Split('/');
        if (repo is null || parts?.Length != 2) {
            return;
        }

        RepositoryOwner = parts[0];
        RepositoryName = parts[1];
    }
}

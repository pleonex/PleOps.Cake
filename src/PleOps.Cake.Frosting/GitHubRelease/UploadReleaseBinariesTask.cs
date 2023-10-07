namespace PleOps.Cake.Frosting.GitHubRelease;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::Cake.Common.Tools.GitReleaseManager;
using global::Cake.Frosting;

[TaskName(nameof(GitHubRelease) + ".UploadReleaseBinaries")]
public class UploadReleaseBinariesTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) =>
        (context.BuildKind == BuildKind.Stable) &&
        !string.IsNullOrEmpty(context.GitHubReleaseContext.GitHubToken);

    public override void Run(BuildContext context)
    {
        string tagName = $"v{context.Version}";
        foreach (string artifact in Directory.EnumerateFiles(context.ArtifactsPath, "*.zip", SearchOption.TopDirectoryOnly)) {
            context.GitReleaseManagerAddAssets(
                context.GitHubReleaseContext.GitHubToken,
                context.GitHubReleaseContext.RepositoryOwner,
                context.GitHubReleaseContext.RepositoryName,
                tagName,
                artifact);
        }
    }
}

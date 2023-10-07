namespace PleOps.Cake.Frosting.GitHubRelease;

using global::Cake.Common.Tools.GitReleaseManager;
using global::Cake.Common.Tools.GitReleaseManager.Export;
using global::Cake.Core.Diagnostics;
using global::Cake.Frosting;

[TaskName(nameof(GitHubRelease) + ".ExportReleaseNotes")]
public class ExportReleaseNotesTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) =>
        (context.BuildKind is BuildKind.Preview or BuildKind.Stable)
        && !string.IsNullOrEmpty(context.GitHubReleaseContext.GitHubToken);

    public override void Run(BuildContext context)
    {
        // Export last release to embed in apps and libraries (if exists)
        try {
            string tagName = $"v{context.Version}";
            context.GitReleaseManagerExport(
                context.GitHubReleaseContext.GitHubToken,
                context.GitHubReleaseContext.RepositoryOwner,
                context.GitHubReleaseContext.RepositoryName,
                context.ChangelogNextFile,
                new GitReleaseManagerExportSettings { TagName = tagName });
        } catch (Exception e) {
            context.Log.Warning("Cannot extract latest release notes:\n{0}", e.ToString());
        }

        // Export full changelog for documentation
        context.GitReleaseManagerExport(
            context.GitHubReleaseContext.GitHubToken,
            context.GitHubReleaseContext.RepositoryOwner,
            context.GitHubReleaseContext.RepositoryName,
            context.ChangelogFile);
    }
}

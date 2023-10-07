namespace PleOps.Cake.Frosting.Dotnet;

using global::Cake.Common.Tools.DotNet;
using global::Cake.Common.Tools.DotNet.MSBuild;
using global::Cake.Common.Tools.DotNet.Pack;
using global::Cake.Core.Diagnostics;
using global::Cake.Frosting;

[TaskName(DotnetTasks.BundleLibsTaskName)]
public class BundleLibrariesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        string changelog = "Check the project site";
        if (File.Exists(context.ChangelogNextFile)) {
            // Get changelog and sanitize for XML NuSpec
            changelog = File.ReadAllText(context.ChangelogNextFile);
            changelog = System.Security.SecurityElement.Escape(changelog);
        } else {
            context.Log.Information(
                "Changelog for next version does not exist: {0}",
                context.ChangelogNextFile);
        }

        var packSettings = new DotNetPackSettings {
            Configuration = context.CSharpContext.Configuration,
            OutputDirectory = context.ArtifactsPath,
            NoBuild = true,
            MSBuildSettings = new DotNetMSBuildSettings()
                .SetVersion(context.Version)
                .WithProperty("PackageReleaseNotes", changelog),
        };
        context.DotNetPack(context.CSharpContext.SolutionPath, packSettings);
    }
}

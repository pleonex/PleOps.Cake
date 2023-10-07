namespace PleOps.Cake.Frosting.Dotnet;

using global::Cake.Common.Tools.DotNet;
using global::Cake.Common.Tools.DotNet.NuGet.Push;
using global::Cake.Frosting;

[TaskName(DotnetTasks.DeployNuGetTaskName)]
public class DeployNuGetTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) =>
        context.BuildKind != BuildKind.Development;

    public override void Run(BuildContext context)
    {
        string feed = (context.BuildKind == BuildKind.Stable)
            ? context.CSharpContext.StableNuGetFeed
            : context.CSharpContext.PreviewNuGetFeed;

        string token = (context.BuildKind == BuildKind.Stable)
            ? context.CSharpContext.StableNuGetFeedToken
            : context.CSharpContext.PreviewNuGetFeedToken;

        var settings = new DotNetNuGetPushSettings {
            Source = feed,
            ApiKey = token,
            SkipDuplicate = true,
        };
        context.DotNetNuGetPush($"{context.ArtifactsPath}/*.nupkg", settings);
    }
}

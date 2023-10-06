namespace PleOps.Cake.Frosting.Common;

using global::Cake.Common.IO;
using global::Cake.Core.Diagnostics;
using global::Cake.Frosting;

public class CleanArtifactsTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) =>
        !context.IsIncrementalBuild;

    public override void Run(BuildContext context)
    {
        context.Log.Information("Removing artifacts directory");
        context.CleanDirectory(context.TemporaryPath, new CleanDirectorySettings { Force = true });
        context.CleanDirectory(context.ArtifactsPath, new CleanDirectorySettings { Force = true });
    }
}

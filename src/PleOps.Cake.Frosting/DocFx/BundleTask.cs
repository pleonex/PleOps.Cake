namespace PleOps.Cake.Frosting.DocFx;

using System.Linq;
using global::Cake.Common.IO;
using global::Cake.Frosting;

[TaskName(DocFxTasks.BundleTaskName)]
[IsDependentOn(typeof(BuildTask))]
public class BundleTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Zip(
            $"{context.TemporaryPath}/docfx/_site",
            $"{context.ArtifactsPath}/docs.zip");
    }
}

namespace PleOps.Cake.Frosting.DocFx;

using System.Linq;
using global::Cake.Common.IO;
using global::Cake.Frosting;

[TaskName(DocFxTasks.StageTaskName)]
[IsDependentOn(typeof(BuildTask))]
public class StageTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Zip(
            $"{context.TemporaryPath}/docfx/_site",
            $"{context.ArtifactsPath}/docs.zip");
    }
}

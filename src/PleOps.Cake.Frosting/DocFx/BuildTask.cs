namespace PleOps.Cake.Frosting.DocFx;

using System.Text;
using global::Cake.Common.Tools.DotNet;
using global::Cake.Core.Diagnostics;
using global::Cake.Frosting;

[TaskName(DocFxTasks.BuildTaskName)]
public class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (!File.Exists(context.DocFxContext.DocFxFile)) {
            context.Log.Warning("There isn't documentation.");
            return;
        }

        if (File.Exists(context.ChangelogFile) && !string.IsNullOrEmpty(context.DocFxContext.ChangelogDocPath)) {
            string changelogDir = Path.GetDirectoryName(context.ChangelogFile)!;
            if (Directory.Exists(changelogDir)) {
                _ = Directory.CreateDirectory(changelogDir);
            }

            File.Copy(context.ChangelogFile, context.DocFxContext.ChangelogDocPath, true);
        }

        string outputDocs = Path.Combine(context.TemporaryPath, "docfx");
        string docfxArgs = new StringBuilder()
            .AppendFormat(" \"{0}\"", context.DocFxContext.DocFxFile)
            .AppendFormat(" --logLevel {0}", context.DocFxContext.ToolingVerbosity)
            .AppendFormat(" {0}", context.WarningsAsErrors ? "--warningsAsErrors" : string.Empty)
            .AppendFormat(" --output \"{0}\"", outputDocs)
            .ToString();

        context.DotNetTool("docfx" + docfxArgs);
    }
}

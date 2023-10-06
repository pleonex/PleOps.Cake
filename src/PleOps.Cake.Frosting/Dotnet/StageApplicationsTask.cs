namespace PleOps.Cake.Frosting.Dotnet;

using System;
using System.Linq;
using global::Cake.Common.IO;
using global::Cake.Common.Tools.DotNet;
using global::Cake.Common.Tools.DotNet.MSBuild;
using global::Cake.Common.Tools.DotNet.Publish;
using global::Cake.Core.Diagnostics;
using global::Cake.Frosting;

[TaskName(DotnetTasks.StageAppsTaskName)]
public class StageApplicationsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        foreach (var project in context.CSharpContext.ApplicationProjects) {
            if (project.Runtimes.Length == 0) {
                throw new Exception($"At least one runtime must be specified for {project.ProjectPath}");
            }

            foreach (string runtime in project.Runtimes) {
                StageApp(context, project, runtime);
            }
        }
    }

    private static void StageApp(BuildContext context, ProjectPublicationInfo project, string runtime)
    {
        string? projectName = project.ProjectName;
        if (string.IsNullOrEmpty(projectName)) {
            projectName = project.ProjectPath.EndsWith(".csproj")
                ? Path.GetDirectoryName(project.ProjectPath)! // full path
                : Path.GetFileName(project.ProjectPath); // dir
        }
        context.Log.Information("Packing {0} for {1}", projectName, runtime);

        // We don't use the property "OutputDirectory" because it removes the last
        // directory separator and the MSBuild convention from the default publish dir
        // is to provide the path WITH the directory separator. We pass the value
        // by property and we must ensure the folder exists.
        string outputDir = Path.Combine(context.TemporaryPath, $"{projectName}_{runtime}");
        context.CleanDirectory(outputDir);

        // Since we are not building in the RID expected path and by default
        // it builds only for the current runtime, we need to rebuild.
        // This makes debugger easier as the path is the same between arch.
        // We don't force self-contained, we let developer choose in the .csproj
        var publishSettings = new DotNetPublishSettings {
            Configuration = context.CSharpContext.Configuration,
            Runtime = runtime,
            Framework = string.IsNullOrEmpty(project.Framework) ? null : project.Framework,
            MSBuildSettings = new DotNetMSBuildSettings()
                .SetVersion(context.Version)
                .WithProperty("PublishDir", $"{outputDir}/"),
        };
        context.DotNetPublish(project.ProjectPath, publishSettings);

        // Copy license and third-party licence note
        string repoDir = context.IssuesContext.State.RepositoryRootDirectory.FullPath;
        CopyIfExists(context, Path.Combine(repoDir, "README.md"), Path.Combine(outputDir, "README.md"));
        CopyIfExists(context, Path.Combine(repoDir, "LICENSE"), Path.Combine(outputDir, "LICENSE.txt"));
        CopyIfExists(context, context.ChangelogFile, Path.Combine(outputDir, "CHANGELOG.md"));
        GenerateLicense(context, project.ProjectPath, outputDir);

        context.Zip(
            outputDir,
            $"{context.ArtifactsPath}/{projectName}_{runtime}_v{context.Version}.zip");
    }

    private static void GenerateLicense(BuildContext context, string projectPath, string outputDir)
    {
        context.Log.Information("Generating third-party notice");

        string args = $"--project {projectPath}";
        args += $" --output {outputDir}/THIRD-PARTY-NOTICES.TXT";
        context.DotNetTool("thirdlicense " + args);
    }

    private static void CopyIfExists(BuildContext context, string file, string output)
    {
        if (File.Exists(file)) {
            File.Copy(file, output);
        } else {
            context.Log.Warning($"File {file} does not exist");
        }
    }
}

namespace PleOps.Cake.Frosting.Dotnet;

using global::Cake.Common.Tools.DotNet;
using global::Cake.Common.Tools.DotNet.Clean;
using global::Cake.Common.Tools.DotNet.MSBuild;
using global::Cake.Common.Tools.DotNet.Restore;
using global::Cake.Core.Diagnostics;
using global::Cake.Frosting;

[TaskName(DotnetTasks.RestoreTaskName)]
public class RestoreDependenciesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (!context.IsIncrementalBuild) {
            context.Log.Information("Cleaning .NET projects for selected configuration and platform");
            var cleanSettings = new DotNetCleanSettings {
                Verbosity = context.CSharpContext.ToolingVerbosity,
                Configuration = context.CSharpContext.Configuration,
                MSBuildSettings = new DotNetMSBuildSettings()
                    .WithProperty("Platform", context.CSharpContext.Platform),
            };
            context.DotNetClean(context.CSharpContext.SolutionPath, cleanSettings);
        }

        context.Log.Information("Restoring .NET project dependencies");
        var dotnetSettings = new DotNetRestoreSettings {
            Verbosity = context.CSharpContext.ToolingVerbosity,
        };

        if (!string.IsNullOrEmpty(context.CSharpContext.NugetConfigPath)) {
            dotnetSettings.ConfigFile = context.CSharpContext.NugetConfigPath;
        }

        context.DotNetRestore(context.CSharpContext.SolutionPath, dotnetSettings);
    }
}

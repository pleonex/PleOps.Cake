namespace PleOps.Cake.Frosting.Dotnet;

using global::Cake.Common.Tools.DotNet;
using global::Cake.Common.Tools.DotNet.Build;
using global::Cake.Common.Tools.DotNet.MSBuild;
using global::Cake.Frosting;

[TaskName(DotnetTasks.BuildTaskName)]
[IsDependentOn(typeof(RestoreDependenciesTask))]
public class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        string logPath = Path.GetFileNameWithoutExtension(context.CSharpContext.SolutionPath) + "_msbuild.binlog";
        logPath = Path.Combine(context.TemporaryPath, logPath);
        string binaryLoggerAssembly = typeof(Microsoft.Build.Logging.StructuredLogger.BinaryLog).Assembly.Location;

        var settings = new DotNetBuildSettings {
            Verbosity = context.CSharpContext.ToolingVerbosity,
            Configuration = context.CSharpContext.Configuration,
            NoRestore = true,
            MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("Platform", context.CSharpContext.Platform)
                .SetVersion(context.Version)
                // Use the binary logger from the addin.
                // MSBuild may have an incompatible version.
                .WithLogger(
                    "BinaryLogger," + binaryLoggerAssembly,
                    "",
                    logPath)
                // For the error parser of VS Code
                .WithProperty("GenerateFullPaths", "true"),
        };
        context.DotNetBuild(context.CSharpContext.SolutionPath, settings);

        context.IssuesContext.Parameters.InputFiles.AddMsBuildBinaryLogFile(logPath);
    }
}

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

        var warningMode = context.WarningsAsErrors
            ? MSBuildTreatAllWarningsAs.Error
            : MSBuildTreatAllWarningsAs.Default;

        var settings = new DotNetBuildSettings {
            Verbosity = context.CSharpContext.ToolingVerbosity,
            Configuration = context.CSharpContext.Configuration,
            NoRestore = true,
            MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("Platform", context.CSharpContext.Platform)
                .TreatAllWarningsAs(warningMode)
                .SetVersion(context.Version)
                // Use the binary logger from the addin.
                // MSBuild may have an incompatible version.
                .WithLogger(
                    "BinaryLogger," + binaryLoggerAssembly,
                    "",
                    logPath)
                // This setting improve the experience with VS Code
                .HideDetailedSummary()
                .WithProperty("GenerateFullPaths", "true"),
        };
        context.DotNetBuild(context.CSharpContext.SolutionPath, settings);

        context.IssuesContext.Parameters.InputFiles.AddMsBuildBinaryLogFile(logPath);
    }
}

internal static class MSBuildSettingExtensions
{
    public static DotNetMSBuildSettings HideDetailedSummary(this DotNetMSBuildSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        settings.DetailedSummary = false;
        return settings;
    }
}

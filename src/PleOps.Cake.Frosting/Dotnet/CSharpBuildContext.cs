namespace PleOps.Cake.Frosting.Dotnet;

using System;
using global::Cake.Common.Tools.DotNet;
using global::Cake.Core.Diagnostics;

public class CSharpBuildContext
{
    public CSharpBuildContext()
    {
        Platform = "Any CPU";
        Configuration = "Debug";

        CoverageTarget = 100;
        TestFilter = string.Empty;

        NugetConfigPath = "nuget.config";
        NugetPackageOutputPath = string.Empty;
        PreviewNuGetFeed = "https://api.nuget.org/v3/index.json";
        StableNuGetFeed = "https://api.nuget.org/v3/index.json";
        PreviewNuGetFeedToken = string.Empty;
        StableNuGetFeedToken = string.Empty;

        ToolingVerbosity = DotNetVerbosity.Minimal;
        SolutionPath = FindSolution();
    }

    public string Platform { get; set; }

    public string Configuration { get; set; }

    public string SolutionPath { get; set; }

    public int CoverageTarget { get; set; }

    public string TestFilter { get; set; }

    public string NugetConfigPath { get; set; }

    public string NugetPackageOutputPath { get; set; }

    public string PreviewNuGetFeed { get; set; }

    [LogIgnore]
    public string PreviewNuGetFeedToken { get; set; }

    public string StableNuGetFeed { get; set; }

    [LogIgnore]
    public string StableNuGetFeedToken { get; set; }

    public DotNetVerbosity ToolingVerbosity { get; set; }

    public void ReadArguments(BuildContext context)
    {
        // From Cake script verbosity "--verbosity X" flag
        ToolingVerbosity = context.Log.Verbosity switch {
            // detailed and normal are similar and most time it's better just minimal
            Verbosity.Normal => DotNetVerbosity.Minimal,

            Verbosity.Quiet => DotNetVerbosity.Quiet,
            Verbosity.Minimal => DotNetVerbosity.Minimal,
            Verbosity.Verbose => DotNetVerbosity.Detailed,
            Verbosity.Diagnostic => DotNetVerbosity.Diagnostic,
            _ => throw new NotSupportedException("Unknown Cake verbosity"),
        };

        context.IfArgIsPresent("dotnet-configuration", x => Configuration = x);
        context.IfArgIsPresent("dotnet-platform", x => Platform = x);

        context.IfArgIsPresent("dotnet-test-filter", x => TestFilter = x);

        PreviewNuGetFeedToken = context.Environment.GetEnvironmentVariable("PREVIEW_NUGET_FEED_TOKEN");
        StableNuGetFeedToken = context.Environment.GetEnvironmentVariable("STABLE_NUGET_FEED_TOKEN");
    }

    private static string FindSolution()
    {
        string[] solutions = Directory.EnumerateFiles("./src", "*.sln").ToArray();
        return (solutions.Length == 1) ? Path.GetFullPath(solutions[0]) : string.Empty;
    }
}

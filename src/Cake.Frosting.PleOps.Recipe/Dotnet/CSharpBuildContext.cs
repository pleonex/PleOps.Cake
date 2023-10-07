// Copyright (c) 2023 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Cake.Frosting.PleOps.Recipe.Dotnet;

using System;
using System.Collections.ObjectModel;
using Cake.Common.Tools.DotNet;
using Cake.Core.Diagnostics;

public class CSharpBuildContext
{
    public CSharpBuildContext()
    {
        Configuration = "Debug";
        Platform = "Any CPU";
        ApplicationProjects = new();

        CoverageTarget = 100;
        TestFilter = string.Empty;

        NugetConfigPath = string.Empty;
        NugetPackageOutputPath = string.Empty;
        PreviewNuGetFeed = "https://api.nuget.org/v3/index.json";
        StableNuGetFeed = "https://api.nuget.org/v3/index.json";
        PreviewNuGetFeedToken = string.Empty;
        StableNuGetFeedToken = string.Empty;

        ToolingVerbosity = DotNetVerbosity.Minimal;
        SolutionPath = FindSolution();

        string expectedTestConfigPath = "./src/Tests.runsettings";
        TestConfigPath = File.Exists(expectedTestConfigPath) ? expectedTestConfigPath : string.Empty;
    }

    public string Configuration { get; set; }

    public string Platform { get; set; }

    public string SolutionPath { get; set; }

    public int CoverageTarget { get; set; }

    public string TestFilter { get; set; }

    public string TestConfigPath { get; set; }

    public string NugetConfigPath { get; set; }

    public string NugetPackageOutputPath { get; set; }

    public Collection<ProjectPublicationInfo> ApplicationProjects { get; }

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

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

/// <summary>
/// Build context information for .NET projects.
/// </summary>
public class DotNetBuildContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetBuildContext"/> class.
    /// </summary>
    public DotNetBuildContext()
    {
        Configuration = "Debug";
        Platform = "Any CPU";
        ApplicationProjects = new();

        CoverageTarget = 80;
        TestFilter = string.Empty;

        NugetConfigPath = string.Empty;
        PreviewNuGetFeed = "https://api.nuget.org/v3/index.json";
        StableNuGetFeed = "https://api.nuget.org/v3/index.json";
        PreviewNuGetFeedToken = string.Empty;
        StableNuGetFeedToken = string.Empty;

        ToolingVerbosity = DotNetVerbosity.Minimal;
        SolutionPath = FindSolution();

        string expectedTestConfigPath = "./src/Tests.runsettings";
        TestConfigPath = File.Exists(expectedTestConfigPath) ? expectedTestConfigPath : string.Empty;
    }

    /// <summary>
    /// Gets or sets the solution configuration to build.
    /// </summary>
    public string Configuration { get; set; }

    /// <summary>
    /// Gets or sets the solution platform to build.
    /// </summary>
    public string Platform { get; set; }

    /// <summary>
    /// Gets or sets the path to the solution file.
    /// </summary>
    /// <remarks>
    /// It tries to auto-detect from the 'src' folder.
    /// </remarks>
    public string SolutionPath { get; set; }

    /// <summary>
    /// Gets or sets the target for the code coverage.
    /// </summary>
    public int CoverageTarget { get; set; }

    /// <summary>
    /// Gets or sets the optional filter to run selective tests.
    /// </summary>
    public string TestFilter { get; set; }

    /// <summary>
    /// Gets or sets the path to the test configuration runsettings file.
    /// </summary>
    public string TestConfigPath { get; set; }

    /// <summary>
    /// Gets or sets the path to the optional nuget configuration file.
    /// </summary>
    public string NugetConfigPath { get; set; }

    /// <summary>
    /// Gets the collection of projects to bundle.
    /// </summary>
    public Collection<ProjectPublicationInfo> ApplicationProjects { get; }

    /// <summary>
    /// Gets or sets the URL to the NuGet feed to deploy preview versions.
    /// </summary>
    public string PreviewNuGetFeed { get; set; }

    /// <summary>
    /// Gets or sets the token to deploy in the preview NuGet feed.
    /// </summary>
    [LogIgnore]
    public string PreviewNuGetFeedToken { get; set; }

    /// <summary>
    /// Gets or sets the URL to the NuGet feed to deploy stable versions.
    /// </summary>
    public string StableNuGetFeed { get; set; }

    /// <summary>
    /// Gets or sets the token to deploy in the stable NuGet feed.
    /// </summary>
    [LogIgnore]
    public string StableNuGetFeedToken { get; set; }

    /// <summary>
    /// Gets or sets the tooling verbosity.
    /// </summary>
    /// <remarks>
    /// Set from the current Cake verbosity.
    /// </remarks>
    public DotNetVerbosity ToolingVerbosity { get; set; }

    /// <summary>
    /// Initializes the information from the command-line arguments.
    /// </summary>
    /// <param name="context">Cake context.</param>
    /// <exception cref="NotSupportedException">Invalid verbosity.</exception>
    public void ReadArguments(PleOpsBuildContext context)
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

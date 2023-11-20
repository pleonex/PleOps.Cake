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

using System.Globalization;
using System.Text;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Test;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.Frosting.PleOps.Recipe.Common;

/// <summary>
/// Run the tests of the .NET projects from the solution level.
/// </summary>
[TaskName(DotnetTasks.TestTaskName)]
[TaskDescription("Run .NET test projects and create code coverage report")]
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(RestoreToolsTask))]
public class TestTask : FrostingTask<PleOpsBuildContext>
{
    /// <inheritdoc />
    public override void Run(PleOpsBuildContext context)
    {
        string testOutput = Path.Combine(context.TemporaryPath, "tests_result");
        RunTests(context, testOutput);

        if (Directory.GetFiles(testOutput, "*", SearchOption.AllDirectories).Length > 0) {
            RunCodeCoverage(context, testOutput);
        } else {
            context.Log.Warning("dotnet couldn't find any test project");
        }
    }

    private static void RunTests(PleOpsBuildContext context, string testOutput)
    {
        context.CleanDirectory(testOutput);

        var netcoreSettings = new DotNetTestSettings {
            Configuration = context.DotNetContext.Configuration,
            ResultsDirectory = testOutput,
            NoBuild = true,
            Loggers = new[] { "trx" },
            ArgumentCustomization = x => x.AppendSwitchQuoted("--collect", "XPlat Code Coverage"),
            MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("Platform", context.DotNetContext.Platform),
        };

        if (!string.IsNullOrWhiteSpace(context.DotNetContext.TestConfigPath)) {
            netcoreSettings.Settings = context.DotNetContext.TestConfigPath;
        }

        if (!string.IsNullOrWhiteSpace(context.DotNetContext.TestFilter)) {
            netcoreSettings.Filter = $"FullyQualifiedName~{context.DotNetContext.TestFilter}";
        }

        context.DotNetTest(context.DotNetContext.SolutionPath, netcoreSettings);
    }

    private static void RunCodeCoverage(PleOpsBuildContext context, string testOutput)
    {
        string coverageOutput = Path.Combine(context.ArtifactsPath, "code_coverage");

        StringBuilder argBuilder = new StringBuilder()
            .Append(" -reports:\"").Append(testOutput).Append("/**/coverage.cobertura.xml\"")
            .Append(" -targetdir:").Append(coverageOutput)
            .Append(" -reporttypes:\"Html;Cobertura\"");
        context.DotNetTool("reportgenerator" + argBuilder);

        // Get final result
        string coverageFile = $"{coverageOutput}/Cobertura.xml";
        if (!File.Exists(coverageFile)) {
            context.Log.Warning("Coverage file doesn't exist");
            return;
        }

        var xml = System.Xml.Linq.XDocument.Load(coverageFile);
        string? xmlLineRate = xml.Root?.Attribute("line-rate")?.Value
            ?? throw new FormatException("Invalid coverage file");

        double lineRate = double.Parse(xmlLineRate, CultureInfo.InvariantCulture) * 100;

        if (lineRate >= context.DotNetContext.CoverageTarget) {
            context.Log.Information($"Valid coverage! {lineRate} >= {context.DotNetContext.CoverageTarget}");
        } else {
            string message = $"Code coverage is below target: {lineRate} < {context.DotNetContext.CoverageTarget}";
            if (context.WarningsAsErrors) {
                throw new CakeException(message);
            }

            context.Log.Warning(message);
        }
    }
}

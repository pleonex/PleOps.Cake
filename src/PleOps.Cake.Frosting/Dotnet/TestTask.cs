namespace PleOps.Cake.Frosting.Dotnet;

using System.Globalization;
using System.Text;
using global::Cake.Common.IO;
using global::Cake.Common.Tools.DotNet;
using global::Cake.Common.Tools.DotNet.MSBuild;
using global::Cake.Common.Tools.DotNet.Test;
using global::Cake.Common.Tools.ReportGenerator;
using global::Cake.Core;
using global::Cake.Core.Diagnostics;
using global::Cake.Frosting;

[TaskName(DotnetTasks.TestTaskName)]
[IsDependentOn(typeof(BuildTask))]
public class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        string testOutput = Path.Combine(context.TemporaryPath, "tests_result");
        RunTests(context, testOutput);

        if (Directory.GetFiles(testOutput, "*", SearchOption.AllDirectories).Length > 0) {
            RunCodeCoverage(context, testOutput);
        } else {
            context.Log.Warning("dotnet couldn't find any test project");
        }
    }

    private void RunTests(BuildContext context, string testOutput)
    {
        context.CleanDirectory(testOutput);

        var netcoreSettings = new DotNetTestSettings {
            Configuration = context.CSharpContext.Configuration,
            ResultsDirectory = testOutput,
            NoBuild = true,
            Loggers = new[] { "trx" },
            ArgumentCustomization = x => x.AppendSwitchQuoted("--collect", "XPlat Code Coverage"),
            MSBuildSettings = new DotNetMSBuildSettings()
                .WithProperty("Platform", context.CSharpContext.Platform)
        };

        if (!string.IsNullOrWhiteSpace(context.CSharpContext.TestConfigPath)) {
            netcoreSettings.Settings = context.CSharpContext.TestConfigPath;
        }

        if (!string.IsNullOrWhiteSpace(context.CSharpContext.TestFilter)) {
            netcoreSettings.Filter = $"FullyQualifiedName~{context.CSharpContext.TestFilter}";
        }

        context.DotNetTest(context.CSharpContext.SolutionPath, netcoreSettings);
    }

    private void RunCodeCoverage(BuildContext context, string testOutput)
    {
        string coverageOutput = Path.Combine(context.ArtifactsPath, "code_coverage");

        // Do not always fail if there isn't coverage files
        Func<int, bool> reportExitHandler = (int code) => {
            if (code != 0) {
                context.Log.Warning("The code coverage tool returned an error");
            }

            return code == 0 || !context.WarningsAsErrors;
        };

        StringBuilder argBuilder = new StringBuilder()
            .Append(" -reports:\"").Append(testOutput).Append("/**/coverage.cobertura.xml\"")
            .Append(" -targetdir:").Append(coverageOutput)
            .Append(" -reporttypes:\"Html;Cobertura\"");
        context.DotNetTool("reportgenerator" + argBuilder.ToString());

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

        if (lineRate >= context.CSharpContext.CoverageTarget) {
            context.Log.Information($"Valid coverage! {lineRate} >= {context.CSharpContext.CoverageTarget}");
        } else {
            string message = $"Code coverage is below target: {lineRate} < {context.CSharpContext.CoverageTarget}";
            if (context.WarningsAsErrors) {
                throw new Exception(message);
            }

            context.Log.Warning(message);
        }
    }
}

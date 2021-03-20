#load "setup.cake"

// Cannot upgrade until the following bug is fixed or it won't work on non-Windows
// https://github.com/cake-build/cake/issues/2216
#tool nuget:?package=ReportGenerator&version=4.2.15
using System.Globalization;

Task("Test")
    .Description("Run the tests")
    .IsDependentOn("Build")
    .Does<BuildInfo>(info =>
{
    if (info.TestProjects.Count == 0) {
        Information("There aren't test projects.");
        return;
    }

    string testOutput = GetTestFolder(info);
    string coverageOutput = $"{testOutput}/coverage";

    // Clean previous test results to not mix them
    if (DirectoryExists(testOutput)) {
        var deleteConfig = new DeleteDirectorySettings { Recursive = true };
        DeleteDirectory(testOutput, deleteConfig);
    }

    var netcoreSettings = new DotNetCoreTestSettings {
        Configuration = info.Configuration,
        ResultsDirectory = testOutput,
        NoBuild = true,
        Loggers = new[] { "trx" },
        ArgumentCustomization = x => x.AppendSwitchQuoted("--collect", "XPlat Code Coverage"),
    };

    if (!string.IsNullOrWhiteSpace(info.RunSettingsFile)) {
        netcoreSettings.Settings = info.RunSettingsFile;
    }

    if (!string.IsNullOrWhiteSpace(info.TestFilter)) {
        netcoreSettings.Filter = $"FullyQualifiedName~{info.TestFilter}";
    }

    DotNetCoreTest(info.SolutionFile, netcoreSettings);

    // Due to a bug in Azure DevOps we need to delete the *.coverage files.
    // In any case we don't use them, we relay in the Cobertura XML.
    // https://developercommunity.visualstudio.com/content/problem/790648/publish-test-results-overwrites-code-coverage.html
    DeleteFiles($"{testOutput}/**/*.coverage");

    // Do not always fail if there isn't coverage files
    Func<int, bool> reportExitHandler = (int code) => {
        if (code != 0) {
            Warning("The code coverage tool returned an error");
        }

        return code == 0 || !info.WarningsAsErrors;
    };

    // Create the report
    ReportGenerator(
        new FilePath[] { $"{testOutput}/**/coverage.cobertura.xml" },
        coverageOutput,
        new ReportGeneratorSettings {
            HandleExitCode = reportExitHandler,
            ReportTypes = new[] {
                ReportGeneratorReportType.Cobertura,
                ReportGeneratorReportType.Html
            }
        }
    );

    // Get final result
    string coverageFile = $"{coverageOutput}/Cobertura.xml";
    if (!FileExists(coverageFile)) {
        Warning("Coverage file doesn't exist");
        return;
    }

    var xml = System.Xml.Linq.XDocument.Load(coverageFile);
    var lineRate = double.Parse(xml.Root.Attribute("line-rate").Value, CultureInfo.InvariantCulture) * 100;

    if (lineRate >= info.CoverageTarget) {
        Information($"Valid coverage! {lineRate} >= {info.CoverageTarget}");
    } else {
        string message = $"Code coverage is below target: {lineRate} < {info.CoverageTarget}";
        if (info.WarningsAsErrors) {
            throw new Exception(message);
        }

        Warning(message);
    }
});

string GetTestFolder(BuildInfo info)
{
    if (!FileExists(info.RunSettingsFile)) {
        Warning("Missing runsettings file");
        return $"{info.ArtifactsDirectory}/test_results";
    }

    // Get the test output folder from the runsettings file
    var runSettings = System.Xml.Linq.XDocument.Load(info.RunSettingsFile);
    string path = runSettings.Root
        .Element("RunConfiguration")
        ?.Element("ResultsDirectory")?.Value;
    if (path == null) {
        Verbose("Test output is not defined in runsettings file");
        return $"{info.ArtifactsDirectory}/test_results";
    }

    // Paths are relative to the settings file
    string settingPath = System.IO.Path.GetDirectoryName(info.RunSettingsFile);
    settingPath = System.IO.Path.GetFullPath(settingPath);
    return System.IO.Path.GetFullPath(path, settingPath);
}

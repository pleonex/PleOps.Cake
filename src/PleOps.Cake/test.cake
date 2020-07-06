#load "setup.cake"

// Cannot upgrade until the following bug is fixed or it won't work on non-Windows
// https://github.com/cake-build/cake/issues/2216
#tool nuget:?package=ReportGenerator&version=4.2.15

Task("Test")
    .Description("Run the tests")
    .IsDependentOn("Build")
    .Does<BuildInfo>(info =>
{
    if (info.TestProjects.Count == 0) {
        Information("There aren't test projects.");
        return;
    }

    string testOutput = GetTestFolder(info.RunSettingsFile);
    string coverageOutput = $"{testOutput}/coverage";

    // Clean previous test results to not mix them
    if (DirectoryExists(testOutput)) {
        var deleteConfig = new DeleteDirectorySettings { Recursive = true };
        DeleteDirectory(testOutput, deleteConfig);
    }

    var netcoreSettings = new DotNetCoreTestSettings {
        Configuration = info.Configuration,
        NoBuild = true,
        Settings = info.RunSettingsFile,
        Logger = "trx",
        ArgumentCustomization = x => x.AppendSwitchQuoted("--collect", "XPlat Code Coverage"),
    };

    if (!string.IsNullOrWhiteSpace(info.TestFilter)) {
        netcoreSettings.Filter = $"FullyQualifiedName~{info.TestFilter}";
    }

    DotNetCoreTest(info.SolutionFile, netcoreSettings);

    // Due to a bug in Azure DevOps we need to delete the *.coverage files.
    // In any case we don't use them, we relay in the Cobertura XML.
    // https://developercommunity.visualstudio.com/content/problem/790648/publish-test-results-overwrites-code-coverage.html
    DeleteFiles($"{testOutput}/**/*.coverage");

    // Create the report
    ReportGenerator(
        new FilePath[] { $"{testOutput}/**/coverage.cobertura.xml" },
        coverageOutput,
        new ReportGeneratorSettings {
            ReportTypes = new[] {
                ReportGeneratorReportType.Cobertura,
                ReportGeneratorReportType.Html
            }
        }
    );

    // Get final result
    var xml = System.Xml.Linq.XDocument.Load($"{coverageOutput}/Cobertura.xml");
    var lineRate = double.Parse(xml.Root.Attribute("line-rate").Value) * 100;

    if (lineRate >= info.CoverageTarget) {
        Information("Full coverage!");
    } else {
        string message = $"Code coverage is below target: {lineRate} < {info.CoverageTarget}";
        if (info.WarningsAsErrors) {
            throw new Exception(message);
        }

        Warning(message);
    }
});

string GetTestFolder(string runSettingsPath)
{
    if (!FileExists(runSettingsPath)) {
        throw new Exception("Missing runsettings file");
    }

    // Get the test output folder from the runsettings file
    var runSettings = System.Xml.Linq.XDocument.Load(runSettingsPath);
    string path = runSettings.Root
        .Element("RunConfiguration")
        ?.Element("ResultsDirectory")?.Value;
    if (path == null) {
        throw new Exception("Test output is not defined");
    }

    return path;
}

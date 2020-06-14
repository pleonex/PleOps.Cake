#load "setup.cake"

#tool nuget:?package=ReportGenerator&version=4.2.15

const string TestResults = "./TestResults";
const string CoverageResults = TestResults + "/coverage";

Task("Test")
    .Description("Run the tests")
    .IsDependentOn("Build")
    .Does<BuildInfo>(info =>
{
    // Clean previous test results to not mix them
    if (DirectoryExists(TestResults)) {
        var deleteConfig = new DeleteDirectorySettings { Recursive = true };
        DeleteDirectory(TestResults, deleteConfig);
    }

    var netcoreSettings = new DotNetCoreTestSettings {
        Configuration = info.Configuration,
        NoBuild = true,
        Settings = "Tests.runsettings",
        Logger = "trx",
        ArgumentCustomization = x => x.AppendSwitchQuoted("--collect", "XPlat Code Coverage"),
    };

    if (info.Tests != string.Empty) {
        netcoreSettings.Filter = $"FullyQualifiedName~{info.Tests}";
    }

    DotNetCoreTest(info.SolutionFile, netcoreSettings);

    // Due to a bug in Azure DevOps we need to delete the *.coverage files.
    // In any case we don't use them, we relay in the Cobertura XML.
    // https://developercommunity.visualstudio.com/content/problem/790648/publish-test-results-overwrites-code-coverage.html
    DeleteFiles(TestResults + "/**/*.coverage");

    // Create the report
    ReportGenerator(
        new FilePath[] { TestResults + "/**/coverage.cobertura.xml" },
        CoverageResults,
        new ReportGeneratorSettings {
            ReportTypes = new[] {
                ReportGeneratorReportType.Cobertura,
                ReportGeneratorReportType.Html
            }
        }
    );

    // Get final result
    var xml = System.Xml.Linq.XDocument.Load(CoverageResults + "/Cobertura.xml");
    var lineRate = xml.Root.Attribute("line-rate").Value;
    if (lineRate == "1") {
        Information("Full coverage!");
    } else {
        Warning($"Missing coverage: {lineRate}");
    }
});

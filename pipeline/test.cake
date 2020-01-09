// NUnit tests
#tool nuget:?package=NUnit.ConsoleRunner&version=3.10.0

// Gendarme: decompress zip
#addin nuget:?package=Cake.Compression&loaddependencies=true&version=0.2.4

// Test coverage
#addin nuget:?package=altcover.api&version=6.0.700
#tool nuget:?package=ReportGenerator&version=4.2.15

Task("UnitTests")
    .Description("Run the unit tests")
    .IsDependentOn("Build")
    .Does<BuildInfo>(info =>
{
    return;
    // .NET Core test library
    var netcoreSettings = new DotNetCoreTestSettings {
        NoBuild = true,
        // Framework = $"netcoreapp{netcoreVersion}"
    };

    if (info.Tests != string.Empty) {
        netcoreSettings.Filter = $"FullyQualifiedName~{info.Tests}";
    }

    DotNetCoreTest(info.SolutionFile, netcoreSettings);
});

Task("Linter-Gendarme")
    .IsDependentOn("Build")
    .Does<BuildInfo>(info =>
{
    return;
    if (IsRunningOnWindows()) {
        Warning("Gendarme is not supported on Windows");
        return;
    }

    var monoTools = DownloadFile("https://github.com/pleonex/mono-tools/releases/download/v4.2.2/mono-tools-v4.2.2.zip");
    ZipUncompress(monoTools, "tools/mono_tools");
    var gendarme = "tools/mono_tools/bin/gendarme";
    if (StartProcess("chmod", $"+x {gendarme}") != 0) {
        Error("Cannot change gendarme permissions");
    }

    // TODO: Find target libraries, maybe by searching the Gendarme.ignore
    RunGendarme(
        gendarme,
        "",
        "src/Yarhl/Gendarme.ignore");
});

public void RunGendarme(string gendarme, string assembly, string ignore)
{
    var retcode = StartProcess(gendarme, $"--ignore {ignore} {assembly}");
    if (retcode != 0) {
        Warning($"Gendarme found errors on {assembly}");
    }
}

Task("CodeCoverage")
    .IsDependentOn("Build")
    .Does(() =>
{
    return;
    // Configure the tests to run with code coverage
    TestWithAltCover(
        "src/Yarhl.UnitTests",
        "Yarhl.UnitTests.dll",
        "coverage_unit.xml");

    TestWithAltCover(
        "src/Yarhl.IntegrationTests",
        "Yarhl.IntegrationTests.dll",
        "coverage_integration.xml");

    // Create the report
    ReportGenerator(
        new FilePath[] { "coverage_unit.xml", "coverage_integration.xml" },
        "coverage_report",
        new ReportGeneratorSettings {
            ReportTypes = new[] {
                ReportGeneratorReportType.Cobertura,
                ReportGeneratorReportType.HtmlInline_AzurePipelines } });

    // Get final result
    var xml = System.Xml.Linq.XDocument.Load("coverage_report/Cobertura.xml");
    var lineRate = xml.Root.Attribute("line-rate").Value;
    if (lineRate == "1") {
        Information("Full coverage!");
    } else {
        Warning($"Missing coverage: {lineRate}");
    }
});

public void TestWithAltCover(string projectPath, string assembly, string outputXml)
{
    // string inputDir = $"{projectPath}/{netBinDir}";
    // string outputDir = $"{inputDir}/__Instrumented";
    // if (DirectoryExists(outputDir)) {
    //     DeleteDirectory(
    //         outputDir,
    //         new DeleteDirectorySettings { Recursive = true });
    // }

    // var altcoverArgs = new AltCover.Parameters.Primitive.PrepareArgs {
    //     InputDirectories = new[] { inputDir },
    //     OutputDirectories = new[] { outputDir },
    //     AssemblyFilter = new[] { "nunit.framework", "NUnit3" },
    //     AttributeFilter = new[] { "ExcludeFromCodeCoverage" },
    //     TypeFilter = new[] { "Yarhl.AssemblyUtils" },
    //     XmlReport = outputXml,
    //     OpenCover = true
    // };
    // Prepare(altcoverArgs);

    // string pluginDir = $"{inputDir}/Plugins";
    // if (DirectoryExists(pluginDir)) {
    //     EnsureDirectoryExists($"{outputDir}/Plugins");
    //     CopyDirectory(pluginDir, $"{outputDir}/Plugins");
    // }

    // NUnit3($"{outputDir}/{assembly}", new NUnit3Settings { NoResults = true });
}
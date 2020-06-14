#load "setup.cake"

Task("Build")
    .Description("Build the project")
    .Does<BuildInfo>(info =>
{
    // Since we are rebuilding, we clean old artifacts
    if (DirectoryExists(info.ArtifactsDirectory)) {
        var deleteConfig = new DeleteDirectorySettings { Recursive = true };
        DeleteDirectory(info.ArtifactsDirectory, deleteConfig);
    }

    var warningMode = info.WarningsAsErrors
        ? MSBuildTreatAllWarningsAs.Error
        : MSBuildTreatAllWarningsAs.Default;
    var buildConfig = new DotNetCoreBuildSettings {
        Configuration = info.Configuration,
        Verbosity = DotNetCoreVerbosity.Minimal,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .TreatAllWarningsAs(warningMode)
            .SetVersion(info.Version)
            // These two settings improve the experience with VS Code
            .HideDetailedSummary()
            .WithProperty("GenerateFullPaths", "true"),
    };
    DotNetCoreBuild(info.SolutionFile, buildConfig);
});

Task("Pack-Libs")
    .Description("Pack libraries")
    .IsDependentOn("Build")
    .Does<BuildInfo>(info =>
{
    var packSettings = new DotNetCorePackSettings {
        Configuration = info.Configuration,
        OutputDirectory = info.ArtifactsDirectory,
        NoBuild = true,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .SetVersion(info.Version),
    };
    foreach (var project in info.LibraryProjects) {
        DotNetCorePack(project, packSettings);
    }
});

Task("Pack-Apps")
    .Description("Pack applications")
    .IsDependentOn("Build")
    .Does<BuildInfo>(info =>
{
    foreach (var project in info.ApplicationProjects) {
        // dotnet publish cannot publish to multiple runtimes yet
        // https://github.com/dotnet/sdk/issues/6490
        // We get the RID and run it several time.
        var projectXml = System.Xml.Linq.XDocument.Load(project).Root;
        List<string> runtimes = projectXml.Elements("PropertyGroup")
            .Where(x => x.Element("RuntimeIdentifiers") != null)
            .SelectMany(x => x.Element("RuntimeIdentifiers").Value.Split(';'))
            .ToList();

        string singleRid = projectXml.Elements("PropertyGroup")
            .Select(x => x.Element("RuntimeIdentifier")?.Value)
            .FirstOrDefault();
        if (singleRid != null && !runtimes.Contains(singleRid)) {
            runtimes.Add(singleRid);
        }

        foreach (string runtime in runtimes) {
            Information("Packing {0} for {1}", project, runtime);
            var publishSettings = new DotNetCorePublishSettings {
                Configuration = info.Configuration,
                OutputDirectory = $"{info.ArtifactsDirectory}/{runtime}",
                Runtime = runtime,
                SelfContained = true,
                PublishSingleFile = true,
                PublishTrimmed = true,
                MSBuildSettings = new DotNetCoreMSBuildSettings()
                    .SetVersion(info.Version),
            };
            DotNetCorePublish(project, publishSettings);
        }
    }
});

public static DotNetCoreMSBuildSettings HideDetailedSummary(this DotNetCoreMSBuildSettings settings)
{
    if (settings == null)
        throw new ArgumentNullException(nameof(settings));

    settings.DetailedSummary = false;
    return settings;
}

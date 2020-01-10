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
        : MSBuildTreatAllWarningsAs.Message;
    var buildConfig = new DotNetCoreBuildSettings {
        Configuration = info.Configuration,
        Verbosity = DotNetCoreVerbosity.Minimal,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .TreatAllWarningsAs(warningMode)
            .WithProperty("Version", info.Version),
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
            .WithProperty("Version", info.Version),
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
    var publishSettings = new DotNetCorePublishSettings {
        Configuration = info.Configuration,
        OutputDirectory = info.ArtifactsDirectory,
        NoBuild = true,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("Version", info.Version),
    };
    foreach (var project in info.ApplicationProjects) {
        DotNetCorePublish(project, publishSettings);
    }
});

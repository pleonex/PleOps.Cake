Task("Build")
    .Description("Build the project")
    .Does<BuildInfo>(info =>
{
    var buildConfig = new DotNetCoreBuildSettings {
        Configuration = info.Configuration,
        Verbosity = DotNetCoreVerbosity.Minimal,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .TreatAllWarningsAs(info.WarningsAsErrors ? MSBuildTreatAllWarningsAs.Error : MSBuildTreatAllWarningsAs.Message)
            .WithProperty("Version", info.Version),
    };
    DotNetCoreBuild(info.SolutionFile, buildConfig);
});

Task("Pack")
    .Description("Pack libraries and executables")
    .IsDependentOn("Build")
    .Does<BuildInfo>(info =>
{
    if (DirectoryExists(info.ArtifactsDirectory)) {
        var deleteConfig = new DeleteDirectorySettings { Recursive = true };
        DeleteDirectory(info.ArtifactsDirectory, deleteConfig);
    }

    var packSettings = new DotNetCorePackSettings {
        Configuration = info.Configuration,
        OutputDirectory = info.ArtifactsDirectory,
        NoBuild = true,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("Version", info.Version),
    };
    foreach (var project in info.LibraryProjects) {
        DotNetCorePack($"./src/{project}/{project}.csproj", packSettings);
    }

    var publishSettings = new DotNetCorePublishSettings {
        Configuration = info.Configuration,
        OutputDirectory = info.ArtifactsDirectory,
        NoBuild = true,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty("Version", info.Version),
    };
    foreach (var project in info.ApplicationProjects) {
        DotNetCorePublish($"./src/{project}/{project}.csproj", publishSettings);
    }
});

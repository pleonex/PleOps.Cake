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

    var settings = new DotNetCorePackSettings {
        Configuration = "Release",
        OutputDirectory = "artifacts/",
        IncludeSymbols = true,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .TreatAllWarningsAs(info.WarningsAsErrors ? MSBuildTreatAllWarningsAs.Error : MSBuildTreatAllWarningsAs.Message)
            .WithProperty("SymbolPackageFormat", "snupkg")
    };
    DotNetCorePack("src/Yarhl.sln", settings);
});

Task("Upload-Artifacts")
    .Description("Upload artifacts to the CI pipeline")
    .IsDependentOn("Pack")
    .Does<BuildInfo>(info =>
{

});

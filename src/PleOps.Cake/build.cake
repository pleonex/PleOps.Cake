#load "setup.cake"
#tool dotnet:?package=ThirdLicense&version=1.1.0

Task("Build")
    .Description("Build the project")
    .Does<BuildInfo>(info =>
{
    // Since we are rebuilding, we clean old artifacts
    if (DirectoryExists(info.ArtifactsDirectory)) {
        CleanDirectory(info.ArtifactsDirectory);
    } else {
        CreateDirectory(info.ArtifactsDirectory);
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
    string changelog = "Check the project site";
    if (FileExists(info.ChangelogFile)) {
        // Get changelog and sanitize for XML NuSpec
        changelog = System.IO.File.ReadAllText(info.ChangelogFile);
        changelog = System.Security.SecurityElement.Escape(changelog);
    }

    var packSettings = new DotNetCorePackSettings {
        Configuration = info.Configuration,
        OutputDirectory = info.ArtifactsDirectory,
        NoBuild = true,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .SetVersion(info.Version)
            .WithProperty("PackageReleaseNotes", changelog),
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

        // Since we are not building in the RID expected path and by default
        // it builds only for the current runtime, we need to rebuild.
        // This makes debugger easier as the path is the same between arch.
        // We don't force self-contained, we let developer choose in the .csproj
        foreach (string runtime in runtimes) {
            string projectName = System.IO.Path.GetFileNameWithoutExtension(project);
            Information("Packing {0} for {1}", projectName, runtime);

            string outputDir = $"{info.ArtifactsDirectory}/{runtime}/{projectName}";
            var publishSettings = new DotNetCorePublishSettings {
                Configuration = info.Configuration,
                OutputDirectory = outputDir,
                Runtime = runtime,
                MSBuildSettings = new DotNetCoreMSBuildSettings()
                    .SetVersion(info.Version),
            };
            DotNetCorePublish(project, publishSettings);

            // Copy license and third-party licences
            string repoDir = System.IO.Path.GetDirectoryName(info.SolutionFile);
            CopyIfExists($"{repoDir}/../README.md", $"{outputDir}/README.md");
            CopyIfExists($"{repoDir}/../LICENSE", $"{outputDir}/LICENSE");
            CopyIfExists(info.ChangelogFile, $"{outputDir}/CHANGELOG.md");
            GenerateLicense(project, outputDir);

            Zip(
                $"{info.ArtifactsDirectory}/{runtime}",
                $"{info.ArtifactsDirectory}/{projectName}_{runtime}_v{info.Version}.zip");
        }
    }
});

public void GenerateLicense(string projectPath, string outputDir)
{
    Information("Generating third-party notice");

    string args = $"--project {projectPath}";
    args += $" --output {outputDir}/THIRD-PARTY-NOTICES.TXT";
    int code = StartProcess("tools/thirdlicense", args);
    if (code != 0) {
        throw new Exception($"ThirdLicense returned {code}");
    }
}

public void CopyIfExists(string file, string output)
{
    if (FileExists(file)) {
        CopyFile(file, output);
    } else {
        Warning($"File {file} does not exist");
    }
}

public static DotNetCoreMSBuildSettings HideDetailedSummary(this DotNetCoreMSBuildSettings settings)
{
    if (settings == null)
        throw new ArgumentNullException(nameof(settings));

    settings.DetailedSummary = false;
    return settings;
}

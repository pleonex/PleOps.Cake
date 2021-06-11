#load "setup.cake"
#tool dotnet:?package=ThirdLicense&version=1.1.0

Task("Build")
    .Description("Build the project")
    .IsDependentOn("Define-Project") // Must be defined by the user
    .IsDependentOn("Show-Info")
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
    if (FileExists(info.ChangelogNextFile)) {
        // Get changelog and sanitize for XML NuSpec
        changelog = System.IO.File.ReadAllText(info.ChangelogNextFile);
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
        foreach (string runtime in GetRuntimes(project)) {
            string projectName = System.IO.Path.GetFileNameWithoutExtension(project);
            Information("Packing {0} for {1}", projectName, runtime);

            // We don't use the property "OutputDirectory" because it removes the last
            // directory separator and the MSBuild convention from the default publish dir
            // is to provide the path WITH the directory separator. We pass the value
            // by property and we must ensure the folder exists.
            string outputDir = $"{info.ArtifactsDirectory}/tmp/{runtime}/{projectName}";
            outputDir = System.IO.Path.GetFullPath(outputDir);
            CreateDirectory(outputDir);

            // Since we are not building in the RID expected path and by default
            // it builds only for the current runtime, we need to rebuild.
            // This makes debugger easier as the path is the same between arch.
            // We don't force self-contained, we let developer choose in the .csproj
            var publishSettings = new DotNetCorePublishSettings {
                Configuration = info.Configuration,
                Runtime = runtime,
                MSBuildSettings = new DotNetCoreMSBuildSettings()
                    .SetVersion(info.Version)
                    .WithProperty("PublishDir", $"{outputDir}/"),
            };
            DotNetCorePublish(project, publishSettings);

            // Copy license and third-party licence note
            string repoDir = System.IO.Path.GetDirectoryName(info.SolutionFile);
            CopyIfExists($"{repoDir}/../README.md", $"{outputDir}/README.md");
            CopyIfExists($"{repoDir}/../LICENSE", $"{outputDir}/LICENSE");
            CopyIfExists(info.ChangelogFile, $"{outputDir}/CHANGELOG.md");
            GenerateLicense(project, outputDir);

            Zip(
                $"{info.ArtifactsDirectory}/tmp/{runtime}/{projectName}",
                $"{info.ArtifactsDirectory}/{projectName}_{runtime}_v{info.Version}.zip");
        }
    }
});

Task("Push-NuGets")
    .Description("Push the NuGet packages to the preview or stable feeds")
    .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType != BuildType.Development)
    .Does<BuildInfo>(info =>
{
    string feed = (info.BuildType == BuildType.Stable) ? info.StableNuGetFeed : info.PreviewNuGetFeed;
    string token = (info.BuildType == BuildType.Stable) ? info.StableNuGetFeedToken : info.PreviewNuGetFeedToken;
    var settings = new DotNetCoreNuGetPushSettings
    {
        Source = feed,
        ApiKey = token,
        SkipDuplicate = true,
    };
    DotNetCoreNuGetPush($"{info.ArtifactsDirectory}/*.nupkg", settings);
});

Task("Push-Apps")
    .Description("Push the applications to the GitHub release")
    .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType == BuildType.Stable)
    .WithCriteria<BuildInfo>((ctxt, info) => !string.IsNullOrEmpty(info.GitHubToken))
    .Does<BuildInfo>(info =>
{
    string tag = $"v{info.Version}";
    foreach (var project in info.ApplicationProjects) {
        foreach (string runtime in GetRuntimes(project)) {
            string projectName = System.IO.Path.GetFileNameWithoutExtension(project);
            GitReleaseManagerAddAssets(
                info.GitHubToken,
                info.RepositoryOwner,
                info.RepositoryName,
                tag,
                $"{info.ArtifactsDirectory}/{projectName}_{runtime}_v{info.Version}.zip");
        }
    }
});

public IEnumerable<string> GetRuntimes(string projectPath)
{
    // dotnet publish cannot publish to multiple runtimes yet
    // https://github.com/dotnet/sdk/issues/6490
    // We get the RID and run it several time.
    var projectXml = System.Xml.Linq.XDocument.Load(projectPath).Root;
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

    return runtimes;
}

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

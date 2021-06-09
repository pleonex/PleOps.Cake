#addin nuget:?package=Cake.Git&version=1.0.1
#tool dotnet:?package=GitVersion.Tool&version=5.6.10
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Reflection;

[AttributeUsage(AttributeTargets.Property)]
public sealed class LogIgnoreAttribute : Attribute
{
}

public class BuildInfo
{
    List<string> libraries;
    List<string> consoles;
    List<string> tests;

    public BuildInfo()
    {
        libraries = new List<string>();
        LibraryProjects = new ReadOnlyCollection<string>(libraries);

        consoles = new List<string>();
        ApplicationProjects = new ReadOnlyCollection<string>(consoles);

        tests = new List<string>();
        TestProjects = new ReadOnlyCollection<string>(tests);
    }

    public string SolutionFile { get; set; }

    public ReadOnlyCollection<string> LibraryProjects { get; }

    public ReadOnlyCollection<string> ApplicationProjects { get; }

    public ReadOnlyCollection<string> TestProjects { get; }

    public string Configuration { get; set; }

    public string Platform { get; set; }

    public string Version { get; set; }

    public BuildType BuildType { get; set; }

    public string WorkMilestone { get; set; }

    public string FutureMilestone { get; set; }

    public string ChangelogNextFile { get; set; }

    public string ChangelogFile { get; set; }

    [LogIgnore]
    public string GitHubToken { get; set; }

    public string PreviewNuGetFeed { get; set; }

    [LogIgnore]
    public string PreviewNuGetFeedToken { get; set; }

    public string StableNuGetFeed { get; set; }

    [LogIgnore]
    public string StableNuGetFeedToken { get; set; }

    public string RepositoryOwner { get; set; }

    public string RepositoryName { get; set; }

    public bool WarningsAsErrors { get; set; }

    public int CoverageTarget { get; set; }

    public string TestFilter { get; set; }

    public string RunSettingsFile { get; set; }

    public string DocFxFile { get; set; }

    public string ArtifactsDirectory { get; set; }

    public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    public bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public void AddLibraryProjects(params string[] names)
    {
        foreach (var name in names) {
            string path = System.IO.File.Exists(name) ? name : $"./src/{name}/{name}.csproj";
            libraries.Add(path);
        }
    }

    public void AddApplicationProjects(params string[] names)
    {
        foreach (var name in names) {
            string path = System.IO.File.Exists(name) ? name : $"./src/{name}/{name}.csproj";
            consoles.Add(path);
        }
    }

    public void AddTestProjects(params string[] names)
    {
        foreach (var name in names) {
            string path = System.IO.File.Exists(name) ? name : $"./src/{name}/{name}.csproj";
            tests.Add(path);
        }
    }
}

public enum BuildType {
    Development,
    Preview,
    Stable,
}

Setup<BuildInfo>(context =>
{
    var info = new BuildInfo {
        Configuration = Argument("configuration", "Debug"),
        Platform = Argument("platform", "Any CPU"),
        Version = Argument("version", string.Empty),
        WarningsAsErrors = Argument("warn-as-error", true),
        TestFilter = Argument("testFilter", string.Empty),
        ArtifactsDirectory = Argument("artifacts", "artifacts"),
        GitHubToken = EnvironmentVariable("GITHUB_TOKEN"),
        PreviewNuGetFeed = "https://api.nuget.org/v3/index.json",
        PreviewNuGetFeedToken = EnvironmentVariable("PREVIEW_NUGET_FEED_TOKEN"),
        StableNuGetFeed = "https://api.nuget.org/v3/index.json",
        StableNuGetFeedToken = EnvironmentVariable("STABLE_NUGET_FEED_TOKEN"),
        WorkMilestone = "vNext",
        FutureMilestone = "Future",
        ChangelogNextFile = Argument("changelog-next", "./CHANGELOG.NEXT.md"),
        ChangelogFile = Argument("changelog", "./CHANGELOG.md"),
        CoverageTarget = 100,
        DocFxFile = "./docs/docfx.json",
        RunSettingsFile = FindTestSettings(),
    };

    SetVersion(info);
    FindSolution(info);
    FindRepoInfo(info);

    return info;
});

void SetVersion(BuildInfo info)
{
    if (string.IsNullOrEmpty(info.Version)) {
        // Get the version using the tool GitVersion
        var version = GitVersion();
        info.Version = version.SemVer;
        info.BuildType = string.IsNullOrEmpty(version.PreReleaseLabel)
            ? BuildType.Stable
            : (version.PreReleaseLabel == "preview" ? BuildType.Preview : BuildType.Development);
    } else {
        info.BuildType = BuildType.Development;
    }

    Verbose("Version: " + info.Version);
    Verbose("Build type: " + info.BuildType);

    // Set the version in the pipeline of Azure Devops
    // https://github.com/Microsoft/azure-pipelines-tasks/blob/master/docs/authoring/commands.md
    if (AzurePipelines.IsRunningOnAzurePipelines) {
        Information("##vso[build.updatebuildnumber]" + info.Version);
    }
}

void FindSolution(BuildInfo info)
{
    var solutions = GetFiles("./src/*.sln");
    if (solutions.Count == 1) {
        info.SolutionFile = solutions.First().FullPath;
        Verbose("Found solution: " + info.SolutionFile);
    } else {
        Verbose("Couldn't find the solution file.");
    }
}

void FindRepoInfo(BuildInfo info)
{
    var branch = GitBranchCurrent(".");
    var origin = branch.Remotes.FirstOrDefault(b => b.Name == "origin");
    if (origin == null) {
        Verbose("Couldn't find origin remote to define repo info");
        return;
    }

    string[] remoteParts = origin?.Url.Split("/");
    info.RepositoryOwner = remoteParts[^2];
    info.RepositoryName = remoteParts[^1];

    // In SSH format we need to remove from ':'
    int sshStartPath = info.RepositoryOwner.IndexOf(':');
    if (sshStartPath != -1) {
        info.RepositoryOwner = info.RepositoryOwner.Substring(sshStartPath + 1);
    }
}

string FindTestSettings()
{
    var possiblePaths = new string[] {
        "./Tests.runsettings",
        "./src/Tests.runsettings",
    };

    foreach (string path in possiblePaths) {
        if (FileExists(path)) {
            return path;
        }
    }

    Verbose("Couldn't find the test setting file");
    return string.Empty;
}

Task("Show-Info")
    .IsDependentOn("Define-Project") // Must be defined by the user
    .Does<BuildInfo>(info =>
{
    PropertyInfo[] properties = info.GetType().GetProperties(
        BindingFlags.DeclaredOnly |
        BindingFlags.Public |
        BindingFlags.Instance);

    foreach (PropertyInfo property in properties) {
        object value = property.GetValue(info);

        bool ignore = Attribute.IsDefined(property, typeof(LogIgnoreAttribute));
        if (ignore) {
            bool isEmpty = (value == null) || string.IsNullOrEmpty(value.ToString());
            Information($"{property.Name} is " + (isEmpty ? "not set" : "set"));
            continue;
        }

        if (value is IEnumerable<string> stringList) {
            value = string.Join(", ", stringList);
        }

        Information($"{property.Name}: {value}");
    }
});

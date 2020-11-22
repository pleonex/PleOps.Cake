#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0
#addin nuget:?package=Cake.Git&version=0.22.0
#tool dotnet:?package=GitVersion.Tool&version=5.5.1
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

    public string ChangelogFile { get; set; }

    [LogIgnore]
    public string GitHubToken { get; set; }

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
            libraries.Add($"./src/{name}/{name}.csproj");
        }
    }

    public void AddApplicationProjects(params string[] names)
    {
        foreach (var name in names) {
            consoles.Add($"./src/{name}/{name}.csproj");
        }
    }

    public void AddTestProjects(params string[] names)
    {
        foreach (var name in names) {
            tests.Add($"./src/{name}/{name}.csproj");
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
        WorkMilestone = "vNext",
        ChangelogFile = Argument("changelog", $"{Argument("artifacts", "artifacts")}/CHANGELOG.md"),
        CoverageTarget = 100,
        DocFxFile = "./docs/docfx.json",
        RunSettingsFile = "./Tests.runsettings",
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

Task("Show-Info")
    .IsDependentOn("Define-Project") // Must be defined by the user
    .Does<BuildInfo>(info =>
{
    PropertyInfo[] properties = info.GetType().GetProperties(
        BindingFlags.DeclaredOnly |
        BindingFlags.Public |
        BindingFlags.Instance);

    foreach (PropertyInfo property in properties) {
        bool ignore = Attribute.IsDefined(property, typeof(LogIgnoreAttribute));
        if (ignore) {
            continue;
        }

        object value = property.GetValue(info);
        if (value is IEnumerable<string> stringList) {
            value = string.Join(", ", stringList);
        }

        Information($"{property.Name}: {value}");
    }
});

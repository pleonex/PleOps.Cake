#module nuget:?package=Cake.DotNetTool.Module&version=0.4.0
#tool dotnet:?package=GitVersion.Tool&version=5.5.1
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

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

    public string GitHubToken { get; set; }

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

    Information("Version: " + info.Version);
    Information("Build type: " + info.BuildType);

    // Set the version in the pipeline of Azure Devops
    // https://github.com/Microsoft/azure-pipelines-tasks/blob/master/docs/authoring/commands.md
    Information("##vso[build.updatebuildnumber]" + info.Version);
}

void FindSolution(BuildInfo info)
{
    var solutions = GetFiles("./src/*.sln");
    if (solutions.Count == 1) {
        info.SolutionFile = solutions.First().FullPath;
        Information("Solution file: " + info.SolutionFile);
    } else {
        Error("Couldn't find the solution file.");
    }
}

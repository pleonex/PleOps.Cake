#tool "nuget:?package=GitVersion.CommandLine&version=5.1.3"

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

    public bool WarningsAsErrors { get; set; }

    public string Tests { get; set; }

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

Setup<BuildInfo>(context =>
{
    var info = new BuildInfo {
        Configuration = Argument("configuration", "Release"),
        Platform = Argument("platform", "Any CPU"),
        WarningsAsErrors = Argument("warn-as-err", true),
        Tests = Argument("tests", string.Empty),
        ArtifactsDirectory = Argument("artifacts", "artifacts"),
    };

    SetVersion(info);
    FindSolution(info);

    return info;
});

void SetVersion(BuildInfo info)
{
    // Get the version using the tool GitVersion
    var version = GitVersion();
    info.Version = version.SemVer;
    Information("Version: " + info.Version);

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

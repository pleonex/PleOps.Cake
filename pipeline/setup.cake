#tool "nuget:?package=GitVersion.CommandLine&version=5.1.3"

public class BuildInfo
{
    public string SolutionFile { get; set; }

    public string Configuration { get; set; }

    public string Platform { get; set; }

    public string Version { get; set; }

    public bool WarningsAsErrors { get; set; }

    public string Tests { get; set; }

    public string ArtifactsDirectory { get; set; }
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

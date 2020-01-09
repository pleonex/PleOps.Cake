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
        Version = Argument("version", "1.0.0"),
        WarningsAsErrors = Argument("warn-as-err", true),
        Tests = Argument("tests", string.Empty),
        ArtifactsDirectory = Argument("artifacts", "artifacts"),
    };

    return info;
});

Task("Set-Version")
    .Does<BuildInfo>(info =>
{
    var version = GitVersion();
    Information("##vso[build.updatebuildnumber]" + version.SemVer);
});

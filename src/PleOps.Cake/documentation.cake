#load "setup.cake"

#tool nuget:?package=docfx.console&version=2.56.5
#addin nuget:?package=Cake.DocFx&version=0.13.1

Task("Build-Doc")
    .Does<BuildInfo>(info =>
{
    if (!FileExists(info.DocFxFile)) {
        Information("There isn't documentation.");
        return;
    }

    string docsDir = System.IO.Path.GetDirectoryName(info.DocFxFile);
    if (FileExists(info.ChangelogFile) && DirectoryExists($"{docsDir}/dev")) {
        CopyFile(info.ChangelogFile, $"{docsDir}/dev/Changelog.md");
    }

    DocFxMetadata(info.DocFxFile);

    var settings = new DocFxBuildSettings {
        OutputPath = info.ArtifactsDirectory,
        WarningsAsErrors = info.WarningsAsErrors,
    };
    DocFxBuild(info.DocFxFile, settings);

    Zip(
        $"{info.ArtifactsDirectory}/_site",
        $"{info.ArtifactsDirectory}/docs.zip");
});

Task("Push-Doc")
    .Description("Push the documentation to GitHub pages")
    .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType != BuildType.Development)
    .Does<BuildInfo>(info =>
{
    Warning("NOT IMPLEMENTED --> #14");
});

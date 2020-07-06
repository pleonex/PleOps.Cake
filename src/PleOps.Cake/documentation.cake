#load "setup.cake"

// Cannot update DocFX until the following bug is fixed:
// https://github.com/dotnet/docfx/issues/5785
#tool nuget:?package=docfx.console&version=2.51.0
#addin nuget:?package=Cake.DocFx&version=0.13.1

Task("Build-Doc")
    .IsDependentOn("Build")
    .Does<BuildInfo>(info =>
{
    if (!FileExists(info.DocFxFile)) {
        Information("There isn't documentation.");
        return;
    }

    DocFxMetadata(info.DocFxFile);

    var settings = new DocFxBuildSettings {
        OutputPath = info.ArtifactsDirectory,
        WarningsAsErrors = info.WarningsAsErrors,
    };
    DocFxBuild(info.DocFxFile, settings);
});

Task("Serve-Doc")
    .IsDependentOn("Build-Doc")
    .Does<BuildInfo>(info =>
{
    if (!FileExists(info.DocFxFile)) {
        throw new Exception("There isn't documentation.");
    }

    DocFxServe($"{info.ArtifactsDirectory}/_site");
});

#load "setup.cake"

// Cannot update DocFX until the following bug is fixed:
// https://github.com/dotnet/docfx/issues/5785
#tool nuget:?package=docfx.console&version=2.51.0
#addin nuget:?package=Cake.DocFx&version=0.13.1

Task("Build-Doc")
    .IsDependentOn("Build")
    .Does<BuildInfo>(info =>
{
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
    DocFxServe($"{info.ArtifactsDirectory}/_site");
});

Task("Deploy-Doc")
    .IsDependentOn("Build-Doc")
    .Does(() =>
{
    int retcode;

    // Clone or pull
    var repo_doc = Directory("doc-branch");
    if (!DirectoryExists(repo_doc)) {
        retcode = StartProcess(
            "git",
            $"clone git@github.com:SceneGate/Yarhl.git {repo_doc} -b gh-pages");
        if (retcode != 0) {
            throw new Exception("Cannot clone repository");
        }
    } else {
        retcode = StartProcess("git", new ProcessSettings {
            Arguments = "pull",
            WorkingDirectory = repo_doc
        });
        if (retcode != 0) {
            throw new Exception("Cannot pull repository");
        }
    }

    // Copy the content of the web
    CopyDirectory("docs/_site", repo_doc);

    // Commit and push
    retcode = StartProcess("git", new ProcessSettings {
        Arguments = "add --all",
        WorkingDirectory = repo_doc
    });
    if (retcode != 0) {
        throw new Exception("Cannot add files");
    }

    retcode = StartProcess("git", new ProcessSettings {
        Arguments = "commit -m \"Update doc from Cake\"",
        WorkingDirectory = repo_doc
    });
    if (retcode != 0) {
        throw new Exception("Cannot commit");
    }

    retcode = StartProcess("git", new ProcessSettings {
        Arguments = "push origin gh-pages",
        WorkingDirectory = repo_doc
    });
    if (retcode != 0) {
        throw new Exception("Cannot push");
    }
});

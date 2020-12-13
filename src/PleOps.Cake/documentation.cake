#load "setup.cake"

#tool nuget:?package=docfx.console&version=2.56.5
#addin nuget:?package=Cake.DocFx&version=0.13.1
#addin nuget:?package=Cake.Git&version=0.22.0

using System.Linq;
using LibGit2Sharp;

Task("Build-Doc")
    .Description("Build the documentation")
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
    // .WithCriteria<BuildInfo>((ctxt, info) => info.BuildType == BuildType.Stable)
    .Does<BuildInfo>(info =>
{
    // We don't depend on it so it can run as a different stage
    string docBuild = $"{info.ArtifactsDirectory}/_site";
    if (!DirectoryExists(docBuild)) {
        throw new Exception("Documentation is not build. Run 'Build-Doc' task first.");
    }

    // TODO: [#6] Implement multi-version documentation
    string pushWorktreeName = "push-gh-pages";
    string committerName = "PleOps.Cake Bot";
    string committerEmail = "ci@pleops.cake";

    using (Repository repo = new Repository(GitFindRootFromPath(".").FullPath)) {
        bool ownTree = false;
        Worktree tree = null;
        if (repo.Worktrees.Any(w => w.Name == pushWorktreeName)) {
            Information("Re-using worktree");
            tree = repo.Worktrees[pushWorktreeName];
        } else {
            Information("Creating worktree");

            // libgit2sharp does not clean the refs and it complains if you run it again later
            string refPath = $"{repo.Info.Path}/refs/heads/{pushWorktreeName}";
            if (FileExists(refPath)) {
                Information("Deleting old ref");
                DeleteFile(refPath);
            }

            CreateDirectory($"{info.ArtifactsDirectory}/tmp");
            tree = repo.Worktrees.Add(
                "gh-pages",
                pushWorktreeName,
                $"{info.ArtifactsDirectory}/tmp/gh-pages",
                false); // no lock since it's not a portable media
            ownTree = true;
        }

        Information($"Worktree at: {tree.WorktreeRepository.Info.WorkingDirectory} - Copying files");
        CopyFiles($"{docBuild}/*", tree.WorktreeRepository.Info.WorkingDirectory);

        Information("Staging and committing all");
        Commands.Stage(tree.WorktreeRepository, "*");
        tree.WorktreeRepository.Commit(
            $"Documentation update for {info.Version}",
            new Signature(committerName, committerEmail, DateTimeOffset.Now),
            new Signature(committerName, committerEmail, DateTimeOffset.Now),
            new CommitOptions());

        Information("Pushing");
        tree.WorktreeRepository.Network.Push(tree.WorktreeRepository.Head);

        if (ownTree) {
            Information("Pruning worktree");
            repo.Worktrees.Prune(tree);
        }
    }
});

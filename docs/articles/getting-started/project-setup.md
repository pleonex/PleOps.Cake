# Project setup

## New setup

Use the [project template](https://github.com/pleonex/template-csharp) to
initialize a new repository, or adapt an existing one copying its file and
structure. Then follow this checklist to adapt the template to your project.

### Project files

- [ ] Update README.md: title and description.
- [ ] Update LICENSE: copyright author
- [ ] Rename, edit, remove the project folders inside `src/` and the solution
      file.
- [ ] Edit the project name and URL in `src/Directory.Build.props`
- [ ] Add, remove, update dependencies in `src/Directory.Build.targets`
- [ ] Update the icons of the project at `docs/images/`.
- [ ] Update the `.csproj` files with the correct dependencies.

### Documentation

- [ ] Update the first steps doc at `docs/guides/First-Steps.md`.
- [ ] Update `docs/docfx.json` with the path to the API project files.
- [ ] Update `docs/global_metadata.json` with the properties of the project.
- [ ] Update `docs/toc.yml` with the project URL.

### Build and release workflows

- [ ] Create the milestone `vNext`.
- [ ] Update `launch.json` with the path of the console application (if any).
- [ ] Update `build.cake` to add the name of the projects in the `src` folder.
- [ ] Update `build.cake` with the stable and preview NuGet feeds.
- [ ] Update in the workflow, the variable `PREVIEW_NUGET_FEED` with the NuGet
      feed in Azure DevOps to use for preview releases or remove the step if
      using a different NuGet feed.
- [ ] Add a new secret `NUGET_PREVIEW_TOKEN` with the personal access token for
      the preview NuGet feed.
- [ ] Add a new secret `STABLE_NUGET_FEED_TOKEN` with the API key for the stable
      NuGet feed.

## Port existing project

In general follow these steps to take and adapt the files from the
[project template](https://github.com/pleonex/template-csharp).

1. Copy the folder `.devcontainer` and starts VS Code. When asked, re-open the
   folder inside the container. Congratulations, now you have a clean dev
   environment.

2. Compare the content of the example `README.md`, `LICENSE`,
   `CODE_OF_CONDUCT.md`, `CONTRIBUTING.md`, `SECURITY.md` files and make any
   desired modifications.

3. Copy and adapt the file `src/Directory.Build.props`. It defines the
   information for the NuGet packages and _SourceLink_. Remove the redundant
   information from your `.csproj`.

4. Follow the format of `src/Directory.Packages.props` to migrate to
   [_centralized NuGet packages_](https://github.com/NuGet/Home/wiki/Centrally-managing-NuGet-package-versions).
   Add each of your dependencies as `PackageVersion` and remove the version in
   each of your `.csproj`.

   - Add a dependency with `coverlet.collector` if you want to have code
     coverage in your test projects.

5. Compare your `.csproj` with the example provider. They should look similar.
   Remember to pack the icon in your public libraries and to define the
   `RuntimeIdentifier` in applications.

6. Copy and adapt `.editorconfig` to follow your project coding style
   guidelines. This file also defines some ignore rules. Pay attention to the
   end of the file where it ignores special rules for test projects, you may
   need to adjust the glob expression.

7. Copy `.config`, `Tests.runsettings`, `GitVersion.yml` and `build.cake`.
   Update the `Define-Project` task to list your project names and the preview
   and stable NuGet feeds.

   - You should be able to restore the build tool with `dotnet tool restore` and
     build and run the tests of your project with
     `dotnet cake --target=BuildTest`.

8. Copy, compare and adapt the DocFX configuration and TOC files. In the
   `docs/dev/Changelog.md` file the full changelog will be written.

9. Copy the workflow from `.github/workflows`. Adapt the value of
   `PREVIEW_NUGET_FEED` and create the two required secrets:
   `STABLE_NUGET_FEED_TOKEN` and `PREVIEW_NUGET_FEED_TOKEN`.

10. Copy `GitReleaseManager.yaml` and adapt to your issue labels.

11. Copy the GitHub templates for issues and pull requests:
    `.github/ISSUE_TEMPLATE` and `PULL_REQUEST_TEMPLATE.md`.

12. Copy the `.vscode` to have build and launch tasks for VS Code. Adapt the
    path to launch your applications (if any).

13. Make sure there is a `gh-pages` branch. You can create a new one with the
    following commands (**make sure you don't have pending changes to commit**):

    ```sh
    git checkout --orphan gh-pages
    git reset * .*
    git commit --allow-empty -m ":sparkles: Initial doc branch"
    git checkout -b main
    ```

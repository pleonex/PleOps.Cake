# Build system setup overview

The following steps will list the actions to setup your project just like the
following [project template](https://github.com/pleonex/template-csharp).

## Build system

1. Create or
   [copy](https://github.com/pleonex/template-csharp/tree/main/build/orchestrator)
   the build system project.
2. Setup your settings for the build context. Common changes are:
   - Disable _warnings as errors_: `context.WarningsAsErrors = false`
   - .NET solution file (if not in `src` folder):
     `context.DotNetContext.SolutionPath`
   - .NET platform (if not `Any CPU`): `context.DotNetContext.Platform`
   - .NET test configuration (if not in standard place):
     `context.DotNetContext.TestConfigPath`
   - .NET code coverage goal: `context.DotNetContext.CoverageTarget = 100`
   - Define production and preview NuGet feeds for deployment:
     `context.DotNetContext.PreviewNuGetFeed` and
     `context.DotNetContext.StableNuGetFeed` (leave empty to use nuget.org feed)
3. Install the external tools or copy the .NET tools manifest:
   [`.config/dotnet-tools.json`](https://github.com/pleonex/template-csharp/blob/main/.config/dotnet-tools.json).
4. Setup _GitVersion_ via
   [`GitVersion.yml`](https://github.com/pleonex/template-csharp/blob/main/GitVersion.yml).
5. Setup _GitReleaseManager_ via
   [`GitReleaseManager.yml`](https://github.com/pleonex/template-csharp/blob/main/GitReleaseManager.yaml).

## .NET projects

1. Copy and adapt the file `src/Directory.Build.props`. It defines the
   information for the NuGet packages and _SourceLink_. Remove the redundant
   information from your `.csproj`.

2. Follow the format of `src/Directory.Packages.props` to migrate to
   [_centralized NuGet packages_](https://github.com/NuGet/Home/wiki/Centrally-managing-NuGet-package-versions).
   Add each of your dependencies as `PackageVersion` and remove the version in
   each of your `.csproj`.

   - Add a dependency with `coverlet.collector` if you want to have code
     coverage in your test projects.

3. Compare your `.csproj` with the example provider. They should look similar.
   Remember to pack the icon in your public libraries.
4. Copy `Tests.runsettings`.

## Documentation

1. Copy, compare and adapt the DocFX configuration and TOC files. In the
   `docs/dev/Changelog.md` file the full changelog will be written.

## Continuous integration

1. Copy the workflow from `.github/workflows`. Adapt the value of
   `PREVIEW_NUGET_FEED` and create the two required secrets:
   `STABLE_NUGET_FEED_TOKEN` and `PREVIEW_NUGET_FEED_TOKEN`.

## Collaboration files

1. Copy and adapt `.editorconfig` to follow your project coding style
   guidelines. This file also defines some ignore rules. Pay attention to the
   end of the file where it ignores special rules for test projects, you may
   need to adjust the glob expression.

2. Compare the content of the example `README.md`, `LICENSE`,
   `CODE_OF_CONDUCT.md`, `CONTRIBUTING.md`, `SECURITY.md` files and make any
   desired modifications.

3. Copy the GitHub templates for issues and pull requests:
   `.github/ISSUE_TEMPLATE` and `PULL_REQUEST_TEMPLATE.md`.

4. Copy the `.vscode` to have build and launch tasks for VS Code. Adapt the path
   to launch your applications (if any).

# Build system setup overview

The following steps will list the actions to setup your project just like the
following [project template](https://github.com/pleonex/template-csharp).

## Build system

1. Create or
   [copy](https://github.com/pleonex/template-csharp/tree/main/build/orchestrator)
   the build system project.
2. Setup the build context. Frequent properties to adjust:
   - Disable _warnings as errors_: `context.WarningsAsErrors = false`
   - .NET solution file (if not in `src` folder):
     `context.DotNetContext.SolutionPath`
   - .NET platform (if not `Any CPU`): `context.DotNetContext.Platform`
   - .NET test configuration (if not `src/Tests.runsettings`):
     `context.DotNetContext.TestConfigPath`
   - .NET code coverage goal: `context.DotNetContext.CoverageTarget = 80`
   - Define production and preview NuGet feeds for deployment:
     `context.DotNetContext.PreviewNuGetFeed` and
     `context.DotNetContext.StableNuGetFeed` (default is nuget.org feed)
3. Install the external tools or copy the .NET tools manifest:
   [`.config/dotnet-tools.json`](https://github.com/pleonex/template-csharp/blob/main/.config/dotnet-tools.json).
4. Setup _GitVersion_ via
   [`GitVersion.yml`](https://github.com/pleonex/template-csharp/blob/main/GitVersion.yml).
5. Setup _GitReleaseManager_ via
   [`GitReleaseManager.yml`](https://github.com/pleonex/template-csharp/blob/main/GitReleaseManager.yaml).
6. Update
   [`.gitignore`](https://github.com/pleonex/template-csharp/blob/main/.gitignore)
   to ignore build outputs.

## .NET projects

1. Copy and adapt the file
   [`src/Directory.Build.props`](https://github.com/pleonex/template-csharp/blob/main/src/Directory.Build.props).
   It defines the information for the NuGet packages and _SourceLink_.
   1. Remove the redundant information from your `.csproj`.
   2. Include a README file for your public libraries / tools. In those .csproj,
      add
      `<None Include="../../README.md" Pack="true" PackagePath="$(PackageReadmeFile)" />`
      - If you don't want a README, comment the line tha sets
        `PackageReadmeFile` in `Directory.Build.props`
   3. Include an icon for your public libraries / tools. In those .csproj add
      `<None Include="../../docs/images/logo_128.png" Pack="true" PackagePath="$(PackageIcon)" Visible="false" />`
      - If you don't define the icon, comment the line tha sets `PackageIcon` in
        `Directory.Build.props`
2. Follow the format of
   [`src/Directory.Packages.props`](https://github.com/pleonex/template-csharp/blob/main/src/Directory.Packages.props)
   to migrate to
   [_centralized NuGet packages_](https://github.com/NuGet/Home/wiki/Centrally-managing-NuGet-package-versions).
   - Add each of your dependencies as `PackageVersion` and remove the version in
     each of your `.csproj`.
   - You can override a version from a `.csproj` with the attribute
     `VersionOverride`.
3. If you want code coverage:
   1. Add a dependency with `coverlet.collector` in your test projects.
   2. Create a file
      [`src/Tests.runsettings`](https://github.com/pleonex/template-csharp/blob/main/src/Tests.runsettings)
      to configure what projects to include / exclude.
4. Configure the projects to pack as NuGet by setting `IsPackable` to `True` in
   their .csproj.
5. Configure the projects to publish in the build system program in
   `BuildLifetime.Setup()`

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

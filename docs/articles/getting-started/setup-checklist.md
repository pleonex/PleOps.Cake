# Setup overview

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

1. Create or [copy](https://github.com/pleonex/template-csharp/tree/main/docs)
   the DocFX documentation project.
   - Our
     [template repository provides](https://github.com/pleonex/template-csharp/tree/main/docs/template)
     some adjustments over the _modern_ template.
   - You can include the GitHub icon in the _modern_ template via the `main.js`
     file.
   - Remember to update links in `docfx.json` and ToC files.
2. If you create the project in other folder than `docs/`, configure the _build
   context_.

## Continuous integration

1. Copy and adapt the workflow in
   [`.github/workflows`](https://github.com/pleonex/template-csharp/tree/main/.github/workflows).
2. Create secret variables with the NuGet tokens in the GitHub project settings
3. Pass your variable's name in the inputs `nuget_stable_token`,
   `nuget_preview_token` or `azure_nuget_token`
4. Review `build.yml` to remove / add OS platforms to run build and tests.
5. Enable GitHub Pages in the repository settings
   1. Select GitHub Actions as the source.

## Collaboration files

1. Create project information files:
   1. `README.md`
   2. `LICENSE`
   3. [`SECURITY.md`](https://github.com/pleonex/template-csharp/blob/main/SECURITY.md)
2. Create community guidelines:
   1. [`CONTRIBUTING.md`](https://github.com/pleonex/template-csharp/blob/main/CONTRIBUTING.md):
      explain how to create issues and pull requests.
   2. `CODE_OF_CONDUCT.md`: GitHub can help to create it.
3. Create IDE support files:
   1. [`.editorconfig`](https://github.com/pleonex/template-csharp/blob/main/.editorconfig):
      code styles and code warnings.
   2. [`.vscode/`](https://github.com/pleonex/template-csharp/tree/main/.vscode):
      VS Code support to build, run and debug the project.
4. Create templates for GitHub issues / PR:
   1. [`.github/ISSUE_TEMPLATE/`](https://github.com/pleonex/template-csharp/tree/main/.github/ISSUE_TEMPLATE):
      templates to create GitHub feature requests and bug reports.
   2. [`.github/PULL_REQUEST_TEMPLATE.md`](https://github.com/pleonex/template-csharp/blob/main/.github/pull_request_template.md):
      Pull Request template.

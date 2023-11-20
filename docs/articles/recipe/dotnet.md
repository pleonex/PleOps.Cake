# .NET project tasks

Tasks to build, test, bundle and deploy .NET projects defined in a solution
file.

> [!TIP]  
> If you have more than one solution file (`.sln`), run your _orchestrator_
> program one time for each solution (e.g. via custom command-line argument).
> The tasks will only work at one solution at a time.

## Context

It provides build state information for .NET tasks. These properties are defined
in the
[`DotNetBuildContext`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.DotNetBuildContext)
class.

| Property                | Default value                         | CLI argument             | Description                                     |
| ----------------------- | ------------------------------------- | ------------------------ | ----------------------------------------------- |
| `Configuration`         | `Debug`                               | `--dotnet-configuration` | Solution configuration for the build            |
| `Platform`              | `Any CPU`                             | `--dotnet-platform`      | Solution platform for the build                 |
| `SolutionPath`          | Set if only one `.sln` file in `src/` |                          | Path to solution file                           |
| `CoverageTarget`        | 80                                    |                          | Code coverage goal. It logs warning if not met  |
| `TestConfigPath`        | `./src/Tests.runsettings`             |                          | Optional test runsettings file                  |
| `NugetConfigPath`       |                                       |                          | Optional `nuget.config` file                    |
| `TestFilter`            |                                       | `--dotnet-test-filter`   | Optional test filter with `FullyQualifiedName~` |
| `PreviewNugetFeed`      | `https://api.nuget.org/v3/index.json` |                          | NuGet feed for preview deployments              |
| `StableNugetFeed`       | `https://api.nuget.org/v3/index.json` |                          | NuGet feed for production deployments           |
| `PreviewNuGetFeedToken` | Env var `PREVIEW_NUGET_FEED_TOKEN`    |                          | Token for the preview NuGet feed                |
| `StableNugetFeedToken`  | Env var `STABLE_NUGET_FEED_TOKEN`     |                          | Token for the production NuGet feed             |
| `ToolingVerbosity`      | From Cake log verbosity               |                          | Verbosity for MSBuild                           |

Additionally it provides the property `ApplicationProjects` to setup the
information of the application to _dotnet-publish_ and bundle. You need to fill
it manually.

## Tasks

Tasks are prefixed with `PleOps.Recipe.Dotnet`. The class
[`DotnetTasks`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks) contains
their full names.

### Restore dependencies

- Task name: `PleOps.Recipe.Dotnet.Restore`
- Type:
  [`RestoreDependenciesTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.RestoreDependenciesTask)
- Depends on: none
- Condition: none
- Build context: `PleOpsBuildContext`
  - Uses:
    - `WarningsAsErrors`
    - `DotNetContext.SolutionPath`
    - `DotNetContext.Configuration`
    - `DotNetContext.Platform`
    - `DotNetContext.NugetConfigPath`
    - `DotNetContext.ToolingVerbosity`
  - Changes: none

Restore the .NET dependencies of the projects listed in the solution file. Then,
if it's not an _incremental build_, run the MSBuild `Clean` target.

It may stop the build if there are warnings and `WarningsAsErrors` is enabled.

### Build

- Task name: `PleOps.Recipe.Dotnet.Build`
- Type: [`BuildTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.BuildTask)
- Depends on: [`Restore`](#restore-dependencies)
- Condition: none
- Build context: `PleOpsBuildContext`
  - Uses:
    - `WarningsAsErrors`
    - `Version`
    - `DotNetContext.SolutionPath`
    - `DotNetContext.Configuration`
    - `DotNetContext.Platform`
    - `DotNetContext.ToolingVerbosity`
  - Changes: none

Build .NET projects from a solution file with the given configuration and
platform. It passes the version when building.

It may stop the build if there are warnings and `WarningsAsErrors` is enabled.

### Run tests

- Task name: `PleOps.Recipe.Dotnet.Tests`
- Type: [`TestTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.TestTask)
- Depends on: [`Build`](#build) and [`RestoreTools`](./common.md#restore-tools)
- Condition: none
- Build context: `PleOpsBuildContext`
  - Uses:
    - `WarningsAsErrors`
    - `ArtifactsPath`
    - `TemporaryPath`
    - `DotNetContext.SolutionPath`
    - `DotNetContext.Configuration`
    - `DotNetContext.Platform`
    - `DotNetContext.TestConfigPath`
    - `DotNetContext.TestFilter`
    - `DotNetContext.CoverageTarget`
  - Changes: none

Run tests projects in the .NET solution with the given configuration and
platform. Then, creates a code coverage report in the artifacts path inside the
subfolder `code_coverage` in HTML and Cobertura format.

It may stop the build if the code coverage exists, it doesn't meet the code
coverage goal and `WarningsAsErrors` is enabled.

### Bundle NuGet packages

- Task name: `PleOps.Recipe.Dotnet.BundleNuGets`
- Type:
  [`BundleNuGetsTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.BundleNuGetsTask)
- Depends on: none
- Condition: none
- Build context: `PleOpsBuildContext`
  - Uses:
    - `ChangelogNextFile`
    - `Version`
    - `DotNetContext.SolutionPath`
    - `DotNetContext.Configuration`
    - `DotNetContext.Platform`
    - `DeliveriesContext.NuGetArtifactsPath`
  - Changes:
    - `DeliveriesContext.NuGetPackages`

Run `dotnet pack` on the solution to create NuGet packages from the projects.
This will ignore projects with the property `IsPackable` set to `false`.

The output directory will be `NuGetArtifactsPath`.

It will set properties with the current version and `PackageReleaseNotes` with
the content from `ChangelogNextFile`.

### Bundle applications

- Task name: `PleOps.Recipe.Dotnet.BundleApplications`
- Type:
  [`BundleApplicationsTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.BundleApplicationsTask)
- Depends on: [`RestoreTools`](./common.md#restore-tools)
- Condition: none
- Build context: `PleOpsBuildContext`
  - Uses:
    - `ArtifactsPath`
    - `TemporaryPath`
    - `RepositoryRootPath`
    - `ChangelogFile`
    - `Version`
    - `DotNetContext.ApplicationProjects`
    - `DotNetContext.Configuration`
  - Changes:
    - `DeliveriesContext.BinaryFiles`

It runs `dotnet publish` for each project configuration in
`ApplicationProjects`. Then zip them.

In the zip it will copy (if exists) the `README.md`, `LICENSE` and
`ChangelogFile` files.

It will also run the _ThirdLicense_ tool over the project to generate and
include `THIRD-PARTY-NOTICES.TXT`.

### Deploy NuGets

- Task name: `PleOps.Recipe.Dotnet.DeployNuGet`
- Type:
  [`DeployNuGetTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.DeployNuGetTask)
- Depends on: none
- Condition: if `BuildKind` is not `Development`
- Build context: `PleOpsBuildContext`
  - Uses:
    - `BuildKind`
    - `DotNetContext.StableNuGetFeed`
    - `DotNetContext.StableNuGetFeedToken`
    - `DotNetContext.PreviewNuGetFeed`
    - `DotNetContext.PreviewNuGetFeedToken`
    - `DeliveriesContext.NuGetPackages`
  - Changes: none

Push the NuGets packages listed in `NuGetPackages`. It will choose the _stable_
(production) or _preview_ feed depending on the build kind.

## Group tasks

These tasks do not perform any action itself. They group a set of other tasks.

- **Build**:
  - Task name: `BuildProject`
  - Type:
    [`BuildProjectTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.BuildProjectTask)
  - Depends on: `Restore`, `Build` and `Tests`
- **Bundle**:
  - Task name: `BundleProject`
  - Type:
    [`BundleProjectTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.BundleProjectTask)
  - Depends on: `BundleNuGets` and `BundleApplications`
- **Deploy**:
  - Task name: `DeployProject`
  - Type:
    [`DeployProjectTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.DeployProjectTask)
  - Depends on: `DeployNuget`

Dependencies:

```plain
PleOps.Recipe.Dotnet.BuildProject
├─PleOps.Recipe.Dotnet.Restore
├─PleOps.Recipe.Dotnet.Build
│ └─PleOps.Recipe.Dotnet.Restore
└─PleOps.Recipe.Dotnet.Tests
   ├─PleOps.Recipe.Dotnet.Build
   │ └─PleOps.Recipe.Dotnet.Restore
   └─PleOps.Recipe.Common.RestoreTools

PleOps.Recipe.Dotnet.BundleProject
├─PleOps.Recipe.Dotnet.BundleNuGets
└─PleOps.Recipe.Dotnet.BundleApplications
   └─PleOps.Recipe.Common.RestoreTools

PleOps.Recipe.Dotnet.DeployProject
└─PleOps.Recipe.Dotnet.DeployNuGet
```

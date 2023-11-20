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
- Build context: `PleOpsBuildContext`
  - Uses:
    - `DotNetContext.SolutionPath`
    - `DotNetContext.Configuration`
    - `DotNetContext.Platform`
    - `DotNetContext.NugetConfigPath`
    - `DotNetContext.ToolingVerbosity`
    - `WarningsAsErrors`
  - Changes: none

Restore the .NET dependencies of the projects listed in the solution file. Then,
if it's not an _incremental build_, run the MSBuild `Clean` target.

It may stop the build if there are warnings and `WarningsAsErrors` is enabled.

### Build

- Task name: `PleOps.Recipe.Dotnet.Build`
- Type: [`BuildTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.BuildTask)
- Depends on: [`Restore`](#restore-dependencies)
- Build context: `PleOpsBuildContext`
  - Uses:
    - `DotNetContext.SolutionPath`
    - `DotNetContext.Configuration`
    - `DotNetContext.Platform`
    - `DotNetContext.ToolingVerbosity`
    - `WarningsAsErrors`
    - `Version`
  - Changes: none

Build .NET projects from a solution file with the given configuration and
platform. It passes the version when building.

It may stop the build if there are warnings and `WarningsAsErrors` is enabled.

### Run tests

- Task name: `PleOps.Recipe.Dotnet.Tests`
- Type: [`TestTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.TestTask)
- Depends on: [`Build`](#build) and [`RestoreTools`](./common.md#restore-tools)
- Build context: `PleOpsBuildContext`
  - Uses:
    - `DotNetContext.SolutionPath`
    - `DotNetContext.Configuration`
    - `DotNetContext.Platform`
    - `DotNetContext.TestConfigPath`
    - `DotNetContext.TestFilter`
    - `DotNetContext.CoverageTarget`
    - `WarningsAsErrors`
    - `ArtifactsPath`
    - `TemporaryPath`
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
- Build context: `PleOpsBuildContext`
  - Uses:
  - Changes:

TODO: description.

### Bundle applications

- Task name: `PleOps.Recipe.Dotnet.BundleApplications`
- Type:
  [`BundleApplicationsTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.BundleApplicationsTask)
- Depends on: [`RestoreTools`](./common.md#restore-tools)
- Build context: `PleOpsBuildContext`
  - Uses:
  - Changes:

TODO: description.

### Deploy NuGets

- Task name: `PleOps.Recipe.Dotnet.DeployNuGet`
- Type:
  [`DeployNuGetTask`](xref:Cake.Frosting.PleOps.Recipe.Dotnet.DeployNuGetTask)
- Depends on: none
- Build context: `PleOpsBuildContext`
  - Uses:
  - Changes:

TODO: description.

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
    [`DeployProjectTask`](xref:xref:Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.DeployProjectTask)
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

# Common tasks

Generic tasks for the build.

## Tasks

Tasks are prefixed with `PleOps.Recipe.Common`.

### Restore tools

- Task name: `PleOps.Recipe.Common.RestoreTools`
- Type:
  [`RestoreToolsTask`](xref:Cake.Frosting.PleOps.Recipe.Common.RestoreToolsTask)
- Depends on: none
- Condition: none
- Build context: `PleOpsBuildContext`
  - Uses:
    - `DotNetContext.NugetConfigPath`
  - Changes: none

Run `dotnet tool restore` to restore the required build tools defined in
`.config/dotnet-tools.json`.

### Clean artifacts

- Task name: `PleOps.Recipe.Common.CleanArtifacts`
- Type:
  [`CleanArtifactsTask`](xref:Cake.Frosting.PleOps.Recipe.Common.CleanArtifactsTask)
- Depends on: none
- Condition: if is not `IsIncrementalBuild`
- Build context: `PleOpsBuildContext`
  - Uses:
    - `TemporaryPath`
    - `ArtifactsPath`
  - Changes: none

Remove files and directories from the artifacts and temporary folders.

### Set version from Git

- Task name: `PleOps.Recipe.Common.SetGitVersion`
- Type:
  [`SetGitVersionTask`](xref:Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask)
- Depends on: [`RestoreTool`](#restore-tools)
- Condition: none
- Build context: `PleOpsBuildContext`
  - Uses: none
  - Changes:
    - `Version`
    - `BuildKind`

Define the version and build type from the git history by using the `GitVersion`
program.

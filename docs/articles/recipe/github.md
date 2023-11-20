# GitHub tasks

Tasks to interact with the repository if it's hosted in GitHub.

## Context

It provides build state information for the GitHub tasks. These properties are
defined in the
[`GitHubBuildContext`](xref:Cake.Frosting.PleOps.Recipe.GitHub.GitHubBuildContext)
class.

| Property          | Default value               | CLI argument | Description                            |
| ----------------- | --------------------------- | ------------ | -------------------------------------- |
| `RepositoryOwner` | Env var `GITHUB_REPOSITORY` |              | Name of the repository owner           |
| `RepositoryName`  | Env var `GITHUB_REPOSITORY` |              | Name of the repository                 |
| `GitHubToken`     | Env var `GITHUB_TOKEN`      |              | GitHub token for reading release notes |

## Tasks

Tasks are prefixed with `PleOps.Recipe.GitHub`.

### Export release notes

- Task name: `PleOps.Recipe.GitHub.ExportReleaseNotes`
- Type:
  [`ExportReleaseNotesTask`](xref:Cake.Frosting.PleOps.Recipe.GitHub.ExportReleaseNotesTask)
- Depends on: [`RestoreTools`](./common.md#restore-tools)
- Condition: `GitHubToken`, `RepositoryOwner` and `RepositoryName` are not
  empty.
- Build context: `PleOpsBuildContext`
  - Uses:
    - `Version`
    - `ChangelogFile`
    - `ChangelogNextFile`
    - `GitHubContext.GitHubToken`
    - `GitHubContext.RepositoryOwner`
    - `GitHubContext.RepositoryName`
  - Changes: none

Export every release notes from GitHub releases of the repo into
`ChangelogFile`. Also exports the release notes from the release that matches
the current version (if any) into `ChangelogNextFile`.

### Upload release binaries

- Task name: `PleOps.Recipe.GitHub.UploadReleaseBinaries`
- Type:
  [`UploadReleaseBinaries`](xref:Cake.Frosting.PleOps.Recipe.GitHub.UploadReleaseBinariesTask)
- Depends on: none
- Condition: build type is `Stable` and `GitHubToken` is not empty.
- Build context: `PleOpsBuildContext`
  - Uses:
    - `Version`
    - `GitHubContext.GitHubToken`
    - `GitHubContext.RepositoryOwner`
    - `GitHubContext.RepositoryName`
    - `DeliveriesContext.BinaryFiles`
  - Changes: none

Upload the release artifact binaries into the GitHub release that matches the
current version.

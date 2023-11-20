# DocFX project tasks

Tasks to build and bundle DocFx documentation projects.

## Context

It provides build state information for the DocFx tasks. These properties are
defined in the
[`DocFxBuildContext`](xref:Cake.Frosting.PleOps.Recipe.DocFx.DocFxBuildContext)
class.

| Property           | Default value                  | CLI argument | Description                                     |
| ------------------ | ------------------------------ | ------------ | ----------------------------------------------- |
| `DocFxFile`        | `./docs/docfx.json`            |              | Path to DocFx project file                      |
| `ChangelogDocPath` | `./docs/articles/changelog.md` |              | Path to copy the changelog in the documentation |
| `ToolingVerbosity` | From Cake verbosity            |              | Verbosity for DocFx commands                    |

## Tasks

Tasks are prefixed with `PleOps.Recipe.DocFx`. The class
[`DocFxTasks`](xref:Cake.Frosting.PleOps.Recipe.DocFx.DocFxTasks) contains their
full names.

### Build

- Task name: `PleOps.Recipe.DocFx.Build`
- Type: [`BuildTask`](xref:Cake.Frosting.PleOps.Recipe.DocFx.BuildTask)
- Depends on: [`RestoreTools`](./common.md#restore-tools)
- Condition: none
- Build context: `PleOpsBuildContext`
  - Uses:
    - `ChangelogFile`
    - `WarningsAsErrors`
    - `DocFxContext.DocFxFile`
    - `DocFxContext.ChangelogDocPath`
    - `DocFxContext.ToolingVerbosity`
    - `DeliveriesContext.DocumentationPath`
  - Changes: none

Build the DocFx documentation project (if it exists). It copies first the
changelog file into the documentation designated path (if it exists).

The output folder goes into the artifacts folder
(`DeliveriesContext.DocumentationPath`).

The build may stop if there are warnings and `WarningsAsErrors` is enabled.

### Bundle

- Task name: `PleOps.Recipe.DocFx.Bundle`
- Type: [`BundleTask`](xref:Cake.Frosting.PleOps.Recipe.DocFx.BundleTask)
- Depends on: [`Build`](#build)
- Condition: none
- Build context: `PleOpsBuildContext`
  - Uses:
    - `ArtifactsPath`
    - `DeliveriesContext.DocumentationPath`
  - Changes: none

Zip the documentation folder into a zip file in the artifacts folder. The output
file name is `docs.zip`.

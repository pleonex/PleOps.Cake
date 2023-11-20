# Recipe overview

The recipe provides with a set of tasks for different project deliveries.

It's built on top of Cake Frosting and available as a NuGet:
`Cake.Frosting.PleOps.Recipe`.

## Build state

The tasks requires a _build state_ with build configuration and outputs:
`PleOpsBuildContext`. More information in [build context](./buildcontext.md).

## Tasks

```plain
PleOps.Recipe.Common.CleanArtifacts

PleOps.Recipe.Common.SetGitVersion
└─PleOps.Recipe.Common.RestoreTools

PleOps.Recipe.DocFx.Bundle
└─PleOps.Recipe.DocFx.Build
   └─PleOps.Recipe.Common.RestoreTools

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

PleOps.Recipe.GitHub.ExportReleaseNotes
└─PleOps.Recipe.Common.RestoreTools

PleOps.Recipe.GitHub.UploadReleaseBinaries
```

### Common

TODO

### .NET

Complete information in [.NET projects](./dotnet.md).

| Task name            | Description                                                   |
| -------------------- | ------------------------------------------------------------- |
| `Restore`            | Restore NuGet dependencies and run Clean target               |
| `Build`              | Build .NET projects                                           |
| `Tests`              | Run tests and create test coverage report                     |
| `BundleNuGets`       | Pack libraries and tools as NuGet packages                    |
| `BundleApplications` | Prepare (publish target) and bundle applications in ZIP files |
| `DeployNuGet`        | Push NuGets to the preview or production feeds                |
| `BuildProjectTask`   | Run tasks restore, build and tests                            |
| `BundleProjectTask`  | Run tasks _BundleNuGets_ and _BundleApplications_             |
| `DeployProjectTask`  | Run task _DeployNuGet_                                        |

### DocFX

TODO

### GitHub

TODO

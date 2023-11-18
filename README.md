# PleOps Cake recipe: a simple DevOps workflow [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/) ![Build and release](https://github.com/pleonex/PleOps.Cake/workflows/Build%20and%20release/badge.svg?branch=main&event=push)

Full automated build, test, stage and release pipeline for simple .NET projects
based on Cake. Check also the
[template repository](https://github.com/pleonex/template-csharp) to see the
pipeline in action!

Tech stack:

- Projects: C# / .NET
- Documentation: DocFX, GitHub page
- CI: GitHub Actions
- Release publication: NuGet feeds, GitHub

<!-- prettier-ignore -->
| Release | Package                                                           |
| ------- | ----------------------------------------------------------------- |
| Stable  | [![Nuget](https://img.shields.io/nuget/v/Cake.Frosting.PleOps.Recipe?label=nuget.org&logo=nuget)](https://www.nuget.org/packages/Cake.Frosting.PleOps.Recipe) |
| Preview | [Azure Artifacts](https://dev.azure.com/benito356/NetDevOpsTest/_packaging?_a=feed&feed=PleOps) |

## Requirements

- .NET 8.0 SDK

## Preview versions

To use a preview version, add a `nuget.config` file in the repository root
directory with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<!-- This file is only needed if you use preview versions of the recipe build system -->
<configuration>
  <packageSources>
    <clear/>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="PleOps-Preview" value="https://pkgs.dev.azure.com/benito356/NetDevOpsTest/_packaging/PleOps/nuget/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
    <packageSource key="PleOps-Preview">
      <package pattern="Cake.Frosting.PleOps.Recipe" />
    </packageSource>
  </packageSourceMapping>
</configuration>
```

## Documentation

Feel free to ask any question in the
[project Discussion site!](https://github.com/pleonex/PleOps.Cake/discussions)

Check the [documentation](https://www.pleonex.dev/PleOps.Cake/) for more
information. For reference, this is the general build and release pipeline.

![release diagram](./docs/articles/workflows/images/release_automation.png)

## Build

The project requires to build .NET 8.0 SDK.

To build, test and generate artifacts run:

```sh
# Build and run tests
dotnet run --project build/orchestrator -- --target=Default

# (Optional) Create bundles (nuget, zips, docs)
dotnet run --project build/orchestrator -- --target=Bundle
```

To build (and test) the recipe against the examples run:

```sh
dotnet run --project src/Cake.Frosting.PleOps.Samples.BuildSystem
```

## Release

Create a new GitHub release with a tag `v{Version}` (e.g. `v2.4`) and that's it!
This triggers a pipeline that builds and deploy the project.

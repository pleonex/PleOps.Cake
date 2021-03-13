# PleOps.Cake pipeline [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/) ![Build and release](https://github.com/pleonex/PleOps.Cake/workflows/Build%20and%20release/badge.svg?branch=develop&event=push)

Full automated build, test, stage and release pipeline for .NET projects based
on Cake. Check also the
[template repository](https://github.com/pleonex/template-csharp) to see the
pipeline in action!

<!-- prettier-ignore -->
| Release | Package                                                           |
| ------- | ----------------------------------------------------------------- |
| Stable  | [![Nuget](https://img.shields.io/nuget/v/PleOps.Cake?label=nuget.org&logo=nuget)](https://www.nuget.org/packages/PleOps.Cake) |
| Preview | [Azure Artifacts](https://dev.azure.com/benito356/NetDevOpsTest/_packaging?_a=feed&feed=PleOps) |

## Requirements

- .NET 5 SDK
- .NET Core 3.1 runtime
- [Linux and MacOS only] Mono 6

## Build system command

The following target are available for build, test and release.

- `BuildTest`: build the project and run its tests and quality assurance tools.

- `Generate-ReleaseNotes`: generate a release notes and full changelog by
  analyzing issues and PR of GitHub.

- `Stage-Artifacts`: run `BuildTest`, generate the release notes, build the
  documentation, pack the libraries in NuGet and stage the executables.

- `Push-Artifacts`: push the libraries, applications and documentation to the
  preview or stable feed.

## Documentation

Feel free to ask any question in the
[project Discussion site!](https://github.com/pleonex/PleOps.Cake/discussions)

Check the [documentation](https://www.pleonex.dev/PleOps.Cake/) for more
information. For reference, this is the general build and release pipeline.

![release diagram](./docs/guides/spec/release_automation.png)

## Build

The project requires to build .NET 5 SDK (Linux and MacOS require also Mono). If
you open the project with VS Code and you did install the
[VS Code Remote Containers](https://code.visualstudio.com/docs/remote/containers)
extension, you can have an already pre-configured development environment with
Docker or Podman.

To build, test and generate artifacts run:

```sh
# Only required the first time
dotnet tool restore

# Default target is Stage-Artifacts
dotnet cake
```

To just build and test quickly, run:

```sh
dotnet cake --target=BuildTest
```

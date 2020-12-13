# .NET DevOps Pipeline

![Build and release](https://github.com/pleonex/PleOps.Cake/workflows/Build%20and%20release/badge.svg?branch=develop&event=push)

Full automated build, test, stage and release pipeline for .NET projects based
on Cake. Check also the
[template repository](https://github.com/pleonex/template-csharp) to see the
pipeline in action!

<!-- prettier-ignore -->
| Release | Provider              | Package                                                          |
| ------- | --------------------- | ---------------------------------------------------------------- |
| Stable  | nuget.org             | ![Stable](https://img.shields.io/nuget/v/PleOps.Cake?style=flat) |
| Preview | Azure public artifact | ![Preview](https://feeds.dev.azure.com/benito356/339c91a8-9d6c-4082-8b1a-93c2ae76b637/_apis/public/Packaging/Feeds/f434f007-6349-4876-9d16-e00ec2460ec0/Packages/ed658fe0-1126-4ed5-9488-601f02d8756b/Badge) |

## Requirements

- .NET 5.0 SDK
- .NET Core 3.1 runtime

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

Check the documentation for more information.

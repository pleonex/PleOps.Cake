# .NET DevOps Pipeline

![Build and release](https://github.com/pleonex/PleOps.Cake/workflows/Build%20and%20release/badge.svg?branch=develop&event=push)

Full automated build, test, stage and release pipeline for .NET projects based
on Cake. Check also the
[template repository](https://github.com/pleonex/template-csharp).

| Release | Package |
| ------- | ------- |
| Stable  | TBD     |
| Preview | TBD     |

## Build system command

The following target are available for build, test and release.

- `BuildTest`: build the project and run its tests and quality assurance tools.

- `Stage-Artifacts`: run `BuildTest`, generate the release notes, build the
  documentation, pack the libraries in NuGet and stage the executables.

- `Create-PreviewRelease`: take the created artifacts, push them into the
  preview feeds and push the documentation into preview.

- `Create-OfficialRelease`: take the created artifacts, push them into the
  release feed, push the documentation and create the GitHub release.

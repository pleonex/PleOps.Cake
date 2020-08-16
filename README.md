# .NET DevOps Pipeline

This project is a playground for a complete implementation of a DevOps pipeline
for .NET projects.

## Build system command

The following target are available for build, test and release.

- `BuildTest`: build the project and run its tests and quality assurance tools.

- `Create-Artifacts`: run `BuildTest`, generate the release notes, build the
  documentation and pack the libraries and executable into NuGet package and
  self-contained applications.

- `Create-PreviewRelease`: take the created artifacts`, push them into the
  preview feeds and push the documentation into preview.

- `Create-OfficialRelease`: take the created artifacts, push them into the
  release feed, push the documentation and create the GitHub release.

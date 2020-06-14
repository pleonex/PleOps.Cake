# .NET DevOps Pipeline

This project is a playground for a complete implementation of a DevOps pipeline
for .NET projects.

## Build system command

The following target are available for build, test and release.

- `Build-Test`: build the project and run its tests and quality assurance tools.

- `Create-Artifacts`: run `Build-RunTests`, generate the release notes, build
  the documentation and pack the libraries and executable into NuGet package and
  self-contained applications.

- `Create-PreviewRelease`: run `Prepare-Release`, push the artifacts into the
  preview feeds, push the documentation into preview and tag the release.

- `Promote-Release`: repackage the NuGet without the _preview_ suffix, push into
  the release feed, push the documentation and create the GitHub release or
  create a tag.

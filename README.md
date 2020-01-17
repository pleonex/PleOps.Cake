# .NET DevOps Pipeline

This project is a playground for a complete implementation of a DevOps pipeline
for .NET projects.

## Build system command

The following target are available for build, test and release.

* `Build-RunTests`: build the project and run its tests and quality assurance tools.

* `Prepare-Release`: run `Build-RunTests`, build the documentation and pack the
  libraries and executable into NuGet package and self-contained applications.

* `Create-TestRelease`: run `Prepare-Release`, push the artifacts into the test
  feeds and push the documentation into the _preview_ version.

* `Draft-Release`: run `Create-TestRelease`, generate the release notes and create
  a draft release in GitHub.

* `Confirm-Release`: repackage the NuGet without the _preview_ suffix and with
  the full release notes content, push into the release feeds, confirm the GitHub
  release and push the documentation.

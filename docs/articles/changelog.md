## v0.8.0 (March 01, 2023)


As part of this release we had [1 issue](https://github.com/pleonex/PleOps.Cake/milestone/14?closed=1) closed.

### Migration steps

- Update `cake.tool` to `3.0` in _.config/dotnet-tools.json_. You can also run `dotnet tool update cake.tool`
- Update the PleOps.Cake version to 0.8.0 from `build.cake`
- If using VS Code Remote Containers, pull and rebuild the container
- Note that this version upgrades some third-party dependencies that use native libraries. They may no longer run in _older_ versions of Linux OS that use libc < 2.33 like Ubuntu Focal or Debian 11.

__enhancement__

- [__#61__](https://github.com/pleonex/PleOps.Cake/pull/61) :arrow_up: Upgrade to Cake 3.0 and bump dependencies

## v0.7.1 (July 31, 2022)


As part of this release we had [3 issues](https://github.com/pleonex/PleOps.Cake/milestone/13?closed=1) closed.

### Migration steps

- Update `dotnet-cake` to `2.2` in _.config/dotnet-tools.json_
- Update the PleOps.Cake version to 0.7.1 from build.cake
- If using VS Code Remote Containers, pull and rebuild the container

__bugs__

- [__#59__](https://github.com/pleonex/PleOps.Cake/pull/59) :umbrella: Test code coverage,  fix dependency for .NET dev container and generate release notes on CI devs builds
- [__#60__](https://github.com/pleonex/PleOps.Cake/pull/60) :bug: Skip release notes on dev builds

__enhancement__

- [__#58__](https://github.com/pleonex/PleOps.Cake/pull/58) Update dependencies

## v0.7.0 (February 25, 2022)


:sparkles: Migration to Cake 2.1!

As part of this release we had [2 issues](https://github.com/pleonex/PleOps.Cake/milestone/12?closed=1) closed.

### Migration steps

- Update `dotnet-cake` to `2.1` in _.config/dotnet-tools.json_
- Fix tokens for release notes (it seems a temporary issue in GitHub):
  1. Create a custom token with the `repo` permissions.
  2. Add the token to the secrets repository/organization variables 
  3. Replace `secrets.GITHUB_TOKEN` with `secrets.<VAR_NAME>` in _.github/workflows/build-and-release.yml_

__bug__

- [__#57__](https://github.com/pleonex/PleOps.Cake/pull/57) Replace GitHub token to access release notes

__enhancement__

- [__#56__](https://github.com/pleonex/PleOps.Cake/pull/56) Migrate to Cake v2

## v0.6.1 (December 04, 2021)


As part of this release we had [1 issue](https://github.com/pleonex/PleOps.Cake/milestone/11?closed=1) closed.

__bug__

- [__#55__](https://github.com/pleonex/PleOps.Cake/pull/55) Fix Push-Doc was not pushing the documentation

## v0.6.0 (November 26, 2021)


As part of this release we had [1 issue](https://github.com/pleonex/PleOps.Cake/milestone/10?closed=1) closed.

The dependencies to run the build system has changed:
- .NET 6 is now required to use the build system.
- .NET Core 3.1 is no longer required.
- Mono is still required for Linux and MacOS

__enhancement__

- [__#54__](https://github.com/pleonex/PleOps.Cake/pull/54) Update dependencies to support .NET 6

**Full Changelog**: https://github.com/pleonex/PleOps.Cake/compare/v0.5.1...v0.6.0
## v0.5.1 (October 12, 2021)


As part of this release we had [1 issue](https://github.com/pleonex/PleOps.Cake/milestone/9?closed=1) closed.

__enhancement__

- [__#53__](https://github.com/pleonex/PleOps.Cake/pull/53) Update to Cake 1.3 and dependencies
  - The latest version of GitReleaseManager allows to create a release without milestones.

## v0.5.0 (June 11, 2021)


As part of this release we had [3 issues](https://github.com/pleonex/PleOps.Cake/milestone/8?closed=1) closed.


__Bugs__

- [__#52__](https://github.com/pleonex/PleOps.Cake/pull/52) Second try to add directory separator in publish directory
- [__#50__](https://github.com/pleonex/PleOps.Cake/pull/50) Add directory separator to publish directory and small fix doc

__Enhancement__

- [__#51__](https://github.com/pleonex/PleOps.Cake/pull/51) Migrate branch name develop to main


## v0.4.3 (June 09, 2021)


As part of this release we had [5 issues](https://github.com/pleonex/PleOps.Cake/milestone/7?closed=1) closed.

This version upgrades DocFX to fix issues building the documentation with the latest VS.
**NOTE** For some reason, GitHub Action CI is failing to push documentation with .NET SDK 5.0.301. The version 5.0.300 seems to work fine.

__Bugs__

- [__#49__](https://github.com/pleonex/PleOps.Cake/pull/49) Downgrade .NET SDK to fix builds
- [__#47__](https://github.com/pleonex/PleOps.Cake/pull/47) Fix potential issue pushing doc branch
- [__#45__](https://github.com/pleonex/PleOps.Cake/pull/45) Bump DocFX and GitVersion and fix Build task dependencies

__Enhancement__

- [__#46__](https://github.com/pleonex/PleOps.Cake/pull/46) Update DocFX to 2.58 to fix build issues on Windows


## v0.4.2 (March 20, 2021)


As part of this release we had [1 issue](https://github.com/pleonex/PleOps.Cake/milestone/6?closed=1) closed.


__Bug__

- [__#44__](https://github.com/pleonex/PleOps.Cake/pull/44) Fix coverage value parsing


## v0.4.1 (March 13, 2021)


As part of this release we had [3 issues](https://github.com/pleonex/PleOps.Cake/milestone/5?closed=1) closed.
It introduces compatibility with Ubuntu 20.04 (LTS) which is the new image in Azure and GitHub build pipelines.

Migration steps:

- Upgrade Cake to version 1.1.0 by running: `dotnet tool update cake.tool`.
- Update the dockerfile for the dev container ([_.devcontainer/Dockerfile_](https://github.com/pleonex/template-csharp/blob/f1645ef92d432a1de90c1f507123a5016c83f761/.devcontainer/Dockerfile)) to use the official .NET 5 image.
- Update your GitHub workflow ([_.github/workflows/build-and-release.yml_](https://github.com/pleonex/template-csharp/blob/f1645ef92d432a1de90c1f507123a5016c83f761/.github/workflows/build-and-release.yml)) to use the latest .NET SDK.

__Enhancements__

- [__#43__](https://github.com/pleonex/PleOps.Cake/pull/43) Upgrade Cake and dependencies to support Ubuntu 20.04
- [__#42__](https://github.com/pleonex/PleOps.Cake/pull/42) Support test setting file from other paths or missing
- [__#35__](https://github.com/pleonex/PleOps.Cake/issues/35) Support Ubuntu 20.04 Focal


## v0.4.0 (December 23, 2020)


As part of this release we had [6 issues](https://github.com/pleonex/PleOps.Cake/milestone/3?closed=1) closed.
Support project sub-folders, fix development environment on latest Docker image and bump Cake dependency!

__Bugs__

- [__#38__](https://github.com/pleonex/PleOps.Cake/pull/38) Fix development environment after Docker image upgrade
- [__#36__](https://github.com/pleonex/PleOps.Cake/issues/36) Git dependency errors on latest dev environment

__Documentation__

- [__#39__](https://github.com/pleonex/PleOps.Cake/pull/39) Add GitHub templates and add missing step in diagram

__Enhancements__

- [__#41__](https://github.com/pleonex/PleOps.Cake/pull/41) Support projects in sub-folders and other locations and improve porting doc
- [__#40__](https://github.com/pleonex/PleOps.Cake/pull/40) Upgrade to Cake 1.0-rc2 and bump dependencies
- [__#7__](https://github.com/pleonex/PleOps.Cake/issues/7) Support projects in sub-folders


## v0.3.1 (December 14, 2020)


As part of this release we had [3 issues](https://github.com/pleonex/PleOps.Cake/milestone/4?closed=1) closed.


__Bug__

- [__#33__](https://github.com/pleonex/PleOps.Cake/pull/33) Trigger workflow on GitHub release publication and add badges

__Enhancements__

- [__#23__](https://github.com/pleonex/PleOps.Cake/issues/23) Close and rename vNext milestone from workflow


## v0.3.0 (December 13, 2020)


As part of this release we had [23 issues](https://github.com/pleonex/PleOps.Cake/milestone/1?closed=1) closed.

First implementation of a full automatized pipeline!

__Documentation__

- [__#29__](https://github.com/pleonex/PleOps.Cake/pull/29) Document project setup
- [__#20__](https://github.com/pleonex/PleOps.Cake/pull/20) Add DocFX documentation

__Enhancements__

- [__#31__](https://github.com/pleonex/PleOps.Cake/pull/31) Push documentation to gh-pages
- [__#28__](https://github.com/pleonex/PleOps.Cake/issues/28) Implement central management of NuGet package versions
- [__#24__](https://github.com/pleonex/PleOps.Cake/pull/24) Implement push artifacts on preview and stable releases
- [__#22__](https://github.com/pleonex/PleOps.Cake/pull/22) Add code analyzers
- [__#18__](https://github.com/pleonex/PleOps.Cake/pull/18) Implement automated release notes
- [__#17__](https://github.com/pleonex/PleOps.Cake/pull/17) Create dev environment
- [__#13__](https://github.com/pleonex/PleOps.Cake/pull/13) Migrate pipeline to GitHub
- [__#12__](https://github.com/pleonex/PleOps.Cake/issues/12) Create stable workflow
- [__#5__](https://github.com/pleonex/PleOps.Cake/issues/5) Implement release notes automation
- [__#3__](https://github.com/pleonex/PleOps.Cake/issues/3) Implement GitHub action
- [__#2__](https://github.com/pleonex/PleOps.Cake/issues/2) Include code analysis

# PleOps Cake ![logo](./docs/images/logo_48.png)

<!-- markdownlint-disable MD033 -->
<p align="center">
  <a href="https://www.nuget.org/packages/Cake.Frosting.PleOps.Recipe">
    <img alt="Stable version" src="https://img.shields.io/nuget/v/Cake.Frosting.PleOps.Recipe?label=nuget.org&logo=nuget" />
  </a>
  &nbsp;
  <a href="https://dev.azure.com/benito356/NetDevOpsTest/_packaging?_a=feed&feed=PleOps">
    <img alt="GitHub commits since latest release (by SemVer)" src="https://img.shields.io/github/commits-since/pleonex/PleOps.Cake/latest?sort=semver" />
  </a>
  &nbsp;
  <a href="https://github.com/pleonex/PleOps.Cake/workflows/Build%20and%20release">
    <img alt="Build and release" src="https://github.com/pleonex/PleOps.Cake/workflows/Build%20and%20release/badge.svg?branch=main&event=push" />
  </a>
  &nbsp;
  <a href="https://choosealicense.com/licenses/mit/">
    <img alt="MIT License" src="https://img.shields.io/badge/license-MIT-blue.svg?style=flat" />
  </a>
  &nbsp;
</p>

Complete DevOps workflow and best-practices for .NET projects based on
[Cake](https://cakebuild.net/).

- ♻️ DevOps best practices for a software project
- 🔧 Build, test and release tasks for .NET projects and documentation sites
- 📚 Documentation explaining the workflow
- 📋 [Template repository](https://github.com/pleonex/template-csharp) ready to
  use

## Tech stack

- **Projects**: C# / .NET
- **Documentation**: DocFX, GitHub page
- **CI**: GitHub Actions
- **Release deployment**: NuGet feeds, GitHub

## Get started

Check out the [documentation site](https://www.pleonex.dev/PleOps.Cake/) to
start learning how to use the library.

Feel free to ask any question in the
[project discussion](https://github.com/pleonex/PleOps.Cake/discussions).

## Usage

The project ships a NuGet library with [Cake Frosting](https://cakebuild.net/)
tasks:

- `Cake.Frosting.PleOps.Recipe`:
  ![Package in NuGet](https://img.shields.io/nuget/v/Cake.Frosting.PleOps.Recipe?label=nuget.org&logo=nuget)

To use it, create a new console application with the
[_Cake Frosting_ template](https://cakebuild.net/docs/getting-started/setting-up-a-new-frosting-project),
add a reference to this recipe NuGet and its tasks will be available to use.

```cs
return new CakeHost()
    .AddAssembly(typeof(Cake.Frosting.PleOps.Recipe.PleOpsBuildContext).Assembly)
    .UseContext<Cake.Frosting.PleOps.Recipe.PleOpsBuildContext>()
    .UseLifetime<BuildLifetime>()
    .Run(args);

[TaskName("Default")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.CleanArtifactsTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.BuildProjectTask))]
public sealed class DefaultTask : FrostingTask
{
}
```

> [!TIP]  
> Find a detailed setup guide in the
> [documentation site](<[docs/articles/getting-started/tutorial.md](https://www.pleonex.dev/PleOps.Cake/docs/getting-started/tutorial.html)>).

### Preview releases

Preview releases are in an
[Azure DevOps NuGet feed](https://dev.azure.com/benito356/NetDevOpsTest/_packaging?_a=feed&feed=PleOps).
Add a `nuget.config` file in the repository root directory with the following
content:

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

## Build

The project requires to build .NET 8.0 SDK.

To build, test and generate artifacts run:

```sh
# Build and run tests (with code coverage!)
dotnet run --project build/orchestrator

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

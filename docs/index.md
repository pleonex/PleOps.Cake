# PleOps Cake ![MIT license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)

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

Then just run the project to start the build system:

```bash
dotnet run --project build/orchestrator
```

Pushing commits will trigger a new continuous integration build with a similar
output as this:

![ci-output](./articles/getting-started/images/github-actions-summary.png)

> [!TIP]  
> Find a detailed setup guide in the
> [documentation site](articles/getting-started/tutorial.md).

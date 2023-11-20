# PleOps Cake

Complete DevOps workflow and best-practices for .NET projects based on
[Cake](https://cakebuild.net/).

- ♻️ DevOps best practices for a software project
- 🔧 Build, test and release tasks for .NET projects and documentation sites
- 📚 Documentation explaining the workflow
- [Template repository](https://github.com/pleonex/template-csharp) ready to use

## Tech stack

- Projects: C# / .NET
- Documentation: DocFX, GitHub page
- CI: GitHub Actions
- Release deployment: NuGet feeds, GitHub

## Usage

The project ships a NuGet library with [Cake Frosting](https://cakebuild.net/)
tasks: **`Cake.Frosting.PleOps.Recipe`**.

To use it, create a new console application with the
[_Cake Frosting_ template](https://cakebuild.net/docs/getting-started/setting-up-a-new-frosting-project),
add a reference to this recipe NuGet and its tasks will be available to use.

**More information in the
[setup guide](./articles/getting-started/tutorial.md).**

## Quick demo

You can check this workflow from the
[template repository](https://github.com/pleonex/template-csharp). Just clone /
download it and run its build system:

```bash
# Build and run tests (with code coverage!)
dotnet run --project build/orchestrator

# Create bundles (nuget, zips, docs)
dotnet run --project build/orchestrator -- --target=Bundle
```

Commits will trigger a new continuous integration build with a similar output as
this:

![ci-output](./articles/getting-started/images/github-actions-summary.png)

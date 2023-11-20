# Configure .NET projects

The build system works with a set of assumptions. It will try to find your
solution file in the `src` folder of your repository. Then it will build, test
and pack every project that it's included in the solution. To make this work, we
need to configure some files.

> [!NOTE]  
> There are a few extra steps that can help you to maintain a large .NET
> project. You can find more information in
> [setup .NET environment](./env-dotnet.md).

## Build settings for .NET projects

First, if you are not using the conventions from the recipe you may want to
adjust the _build context_. For instance, to set the location of the solution
file. You can do this from the `Setup` method of the `BuildLifetime` class we
just created. You can get the list of options in
[build context](../recipe/buildcontext.md).

## Configure test code coverage

You can get a report on code coverage from your unit tests. Add a package
reference to `coverlet.collector` in your test libraries. Then create a file
[`src/Tests.runsettings`](https://github.com/pleonex/template-csharp/blob/main/src/Tests.runsettings)
to configure what projects to include / exclude.

## Packing .NET libraries

The build system will run `dotnet pack` over your solution file. This will try
to pack every library. The benefit is that it will respect the `Platform`
setting from your solution file.

In order to work, open/create a top-level `src/Directory.Build.props` and set
inside a `PropertyGroup`: `IsPackable` to `false`. Then for each _public library
or tool_ that you are going to create a NuGet package, set `IsPackabe` to
`true`.

## Packing .NET apps

Unfortunately the previous _packing_ trick for libraries doesn't work for
applications. The .NET tooling requires to call `dotnet publish` for each
runtime identifier and target framework to use.

The build system will call `dotnet publish` with a specific configuration on
each configured projects. You will need to define the projects to _publish_
inside the `Setup` method of the `BuildLifetime` class of your build system:

```cs
context.DotNetContext.ApplicationProjects.Add(
    new ProjectPublicationInfo(
        "./src/MyConsole", // path to folder or .csproj
        new[] { "win-x64", "linux-x64", "osx-x64" }, // runtime identifiers
        "net8.0")); // target framework
```

## .NET ready

And voilà! 🎉 If you run this project it should be able to build and release
your .NET project.

**Build and run tests (with code coverage!)**:

```bash
# It runs the "Default" target
dotnet run --project build/orchestrator
```

**Bundle**:

```bash
dotnet run --project build/orchestrator -- --target=Bundle
```

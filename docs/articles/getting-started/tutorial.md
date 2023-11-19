# Setup guide

**_Welcome to PleOps.Cake! 🎉_**

We will cover how to integrate a full featured DevOps workflow into your .NET
projects. This includes:

- 🔧 .NET project building, testing and publishing to NuGet feeds
- 📃 Documentation project building and publishing to GitHub Pages
- 📦 Dependency management
- 📉 Git workflow for features, patches and releases
- 🆕 Versioning and release management
- 🤖 Continuous Integration (CI) pipelines
- 🏠 Local development environment and IDEs

At the end of this guide you will have a project ready to release with a
continuous integration system like GitHub Actions.

![Summary page of GitHub Actions building template-csharp repo](./images/github-actions-summary.png)

The guide will start explaining how to create the
[build system project](#build-system-project), then we will configure your
[.NET projects to it](#configure-net-projects) and a
[documentation page](#adding-documentation). It continues
[setting up continuous integration](#setting-up-continuous-integration) and it
ends with some optional but recommended
[collaboration files](#contribution-files).

To understand how the build system works, the workflow and tasks
[check-out the workflow](../workflows/pipeline.md) page.

Prepare your self a good cup of 🍵 or ☕ and let's dive into it! 🤿  
And if you have any question along the way don't hesitate to
[ask for help](https://github.com/pleonex/PleOps.Cake/discussions).

> [!TIP]  
> If you are starting a new project, consider cloning the
> [C# Template](https://github.com/pleonex/template-csharp) repository. It
> provides the skeleton of everything covered in this guide, you will just need
> to replace some placeholders. Get more information at
> [template setup](./setup-template.md).

> [!NOTE]  
> This guide details each file and step. There is a shorter step list in
> [setup overview](./setup-checklist.md).

## Pre-requisites

The build system is based on [Cake](https://cakebuild.net/) which uses the .NET
platform. To create and later run the build orchestrator you will need:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- An IDE compatible with C# / .NET: VS Code, Visual Studio, Rider

## Build system project

The build system project is a .NET console project that runs with Cake a set of
pre-defined tasks. You can create this project in several ways, this guide will
cover how to create it as a console application. You could also create it a
single file C# script, check-out [build script](./build-script.md) for more
information.

### Console project

First create a C# console app in your repository. The location doesn't matter,
we will use `build/orchestrator`:

```bash
dotnet new console -n BuildSystem -o build/orchestrator
```

Open the project (or change directory) and add a dependency with our Cake
recipe:
[`Cake.Frosting.PleOps.Recipe`](https://www.nuget.org/packages/Cake.Frosting.PleOps.Recipe).
This will provide a set of pre-defined tasks that will do almost everything we
need.

One useful setting for the project is to setup the `RunWorkingDirectory` inside
a `PropertyGroup`. So we can run this project from anywhere and the relative
paths will still resolve correctly. Set it to the root of the repository, for
instance:

```xml
<RunWorkingDirectory>$(MSBuildProjectDirectory)/../..</RunWorkingDirectory>
```

### Define the Cake runner

Now let's define the content of our program. It will run one command: create a
`CakeHost`, configure assemblies with tasks and run with the command-line
arguments.

```cs
return new CakeHost()
    .AddAssembly(typeof(Cake.Frosting.PleOps.Recipe.PleOpsBuildContext).Assembly)
    .UseContext<PleOpsBuildContext>()
    .UseLifetime<BuildLifetime>()
    .Run(args);
```

The _lifetime_ class will allow us to run code before starting to run tasks.
Here we configure our build system project. Define your own `BuildLifetime`
class in the same `Program.cs` (or a new file), and add the following class:

```cs
public sealed class BuildLifetime : FrostingLifetime<PleOpsBuildContext>
{
    public override void Setup(PleOpsBuildContext context, ISetupContext info)
    {
        // HERE you can set default values overridable by command-line

        // Update build parameters from command line arguments.
        context.ReadArguments();

        // HERE you can force values non-overridable.

        // Print the build info to use.
        context.Print();
    }

    public override void Teardown(PleOpsBuildContext context, ITeardownContext info)
    {
        // Save the artifacts info for the next execution (e.g. deploy job)
        context.DeliveriesContext.Save();
    }
}
```

### Entrypoint tasks

Next, in the same file (or new files) define your _entry-point_ tasks. These are
the tasks you will run from command-line that will execute in order a set of
tasks to achieve a goal (e.g. build and test). We will create three main tasks:

- `Default`. This task will also run when you don't specify a task name in
  command-line. It will perform the most common flow: build and run the tests.

```cs
[TaskName("Default")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.CleanArtifactsTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotNetTasks.BuildProjectTask))]
public sealed class DefaultTask : FrostingTask
{
}
```

- `Bundle`. It will take the output from the `Default` / Build tasks and create
  packages like NuGet or zip files. It will also build

```cs
[TaskName("Bundle")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.GitHub.ExportReleaseNotesTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotNetTasks.BundleProjectTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.DocFx.BuildTask))]
public sealed class BundleTask : FrostingTask
{
}
```

- `Deploy`. It will use the _bundles_ and deploy them, for instance to a NuGet
  feed or as an attachment to a GitHub release.

```cs
[TaskName("Deploy")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.DeployProjectTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.GitHub.UploadReleaseBinariesTask))]
public sealed class DeployTask : FrostingTask
{
}
```

### Third-party tools

The tasks from the _recipe_ uses a set of _third-party_ tools. They are _dotnet
tools_ and we can define their versions and restore with our own _tool manifest
file_.

First create a new _dotnet tool manifest_ by running in the top repository
folder:

```bash
dotnet new tool-manifest
```

Then install the latest version of the following tools (you can copy directly
the file from
[template repo](https://github.com/pleonex/template-csharp/blob/main/.config/dotnet-tools.json)):

```bash
# Generate the project version number from Git history
dotnet tool install gitversion.tool

# Create thirdparty notice files to include in your apps
dotnet tool install thirdlicense

# Generate an HTML overview of code coverage
dotnet tool install dotnet-reportgenerator-globaltool

# Documentation generator for .NET projects
dotnet tool install docfx

# Interact with GitHub releases (extract release notes and attach files)
dotnet tool install gitreleasemanager.tool
```

The _git release manager_ tool needs a configuration file. You can copy a
[default one](https://github.com/pleonex/template-csharp/blob/main/GitReleaseManager.yaml),
more information
[in their site](https://gittools.github.io/GitReleaseManager/docs/configuration/default-configuration).

_GitVersion_ also needs a configuration file. You can take
[our file](https://github.com/pleonex/template-csharp/blob/main/GitVersion.yml)
that matches the [proposed versioning strategy](../workflows/versioning.md) or
create a new one with your needs, more info
[in their web](https://gitversion.net/docs/).

You may need some of the default options to your project. Check the
[build context](../recipe/buildcontext.md) of the recipe for more information.

## Configure .NET projects

The build system works with a set of assumptions. It will try to find your
solution file in the `src` folder of your repository. Then it will build, test
and pack every project that it's included in the solution. To make this work, we
need to configure some files.

> [!NOTE]  
> There are a few extra steps that can help you to maintain a large .NET
> project. You can find more information in
> [setup .NET environment](./env-dotnet.md).

### Build settings for .NET projects

First, if you are not using the conventions from the recipe you may want to
adjust the _build context_. For instance, to set the location of the solution
file. You can do this from the `Setup` method of the `BuildLifetime` class we
just created. You can get the list of options in
[build context](../recipe/buildcontext.md).

### Configure test code coverage

You can get a report on code coverage from your unit tests. Add a package
reference to `coverlet.collector` in your test libraries. Then create a file
[`src/Tests.runsettings`](https://github.com/pleonex/template-csharp/blob/main/src/Tests.runsettings)
to configure what projects to include / exclude.

### Packing .NET libraries

The build system will run `dotnet pack` over your solution file. This will try
to pack every library. The benefit is that it will respect the `Platform`
setting from your solution file.

In order to work, open/create a top-level `src/Directory.Build.props` and set
inside a `PropertyGroup`: `IsPackable` to `false`. Then for each _public library
or tool_ that you are going to create a NuGet package, set `IsPackabe` to
`true`.

### Packing .NET apps

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

### .NET ready

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

## Building documentation

[DocFX](https://dotnet.github.io/docfx/index.html) is a static site generator
(like Jekyll or Hugo) that can also generate API documentation of .NET projects.
PleOps Cake provides tasks to build documentation projects with DocFx.

If you don't have one project already, follow their
[quick guide](https://dotnet.github.io/docfx/index.html) or
[take our template project](https://github.com/pleonex/template-csharp/tree/main/docs).

The
[template project also provides](https://github.com/pleonex/template-csharp/tree/main/docs/template)
some adjustments over the default _modern_ template. Check them out if you are
interested. For instance, you can put your GitHub icon in the `main.js` file.

The build system expects to find a `docfx.json` file in `docs/`. If you create
the project in other folder, adjust the build context in
`BuildLifetime.Setup()`:

```cs
context.DocFxContext.DocFxFile = "Documentation/DocFX/docfx.json";
```

You see the output by running:

```bash
dotnet docfx docs/docfx.json --serve
```

> [!IMPORTANT]  
> If you open the build output in your browser directly (`index.html`), the site
> will look like broken. The generated site requires an HTTP server to see
> properly. You can use the one from DocFX:
> `dotnet docfx serve build/artifacts/docs`

### Changelog

If you check the `Bundle` task we created, you will see we run first the task
`ExportReleaseNotesTask`. This task generates two files from the release
information in GitHub:

- `CHANGELOG.md`: it contains the release notes of every released version.
- `CHANGELOG.NEXT.md`: it contains the release notes of GitHub release that
  matches the current build version. This will only works when building from a
  git tag that matches the GitHub release.
  - This file is included in the NuGet packages in the property
    `PackageReleaseNotes`.

The name of these files can be configured through _build context_. At build
time, DocFX will take the `CHANGELOG.md` file and copy it into the documentation
project. You can adjust this behavior and path with the build context of DocFx
(`ChangelogDocPath`).

> [!NOTE]  
> If you don't use GitHub releases or prefer to write manually the release
> notes, you can not add the task `ExportReleaseNotesTask`. The documentation
> and NuGet pack will still look for those files in your repo.

## Setting up continuous integration

Nice, now we can fully build, test and bundle our projects locally. Time to
setup a continuous integration pipeline to do the same. According to the
[proposed workflow](../workflows/pipeline.md) there are three types of builds:

- **Development**: for instance locally or on a pull request validation
- **Preview**: publish the changes on an intermediary / staging feeds. It
  happens on commits / merges into the main branch (`main` / `develop`).
- **Production**: publish stable versions. It happens when we create a git tag
  (e.g. via GitHub release).

For our workflow, we will need to _stages_ / main jobs: **build** and
**deploy**. For readability, the template project proposed a _reusable workflow_
for each step and then the main file. You can
[check it and copy / adapt it as wanted](https://github.com/pleonex/template-csharp/tree/main/.github/workflows).

The build system knows if we are running in each build mode from the version
number (that it's generated from git history and branch names). When we run the
_Deploy_ target, it knows if it should go to _preview_ or _production_.

In order to run this workflow you will need to configure a couple of things:

- **NuGet tokens**: create a GitHub secret variable with the tokens to publish
  to your feeds.
  - Define your secrets and pass them to `nuget_preview_token` and
    `nuget_stable_token`.
  - If you are using Azure DevOps for one of the feeds, create a single variable
    and pass it to `azure_nuget_token` input. Define also `azure_nuget_feed`.
- **Enable GitHub pages**: if you want to publish your documentation via GitHub
  pages, you need to enable in your project settings:
  1. Go to _Settings_ > _Pages_
  2. In _Source_ select _GitHub Actions_
  3. Optionally set a domain and enforce HTTPS.

## Contribution files

While not directly related to the build system, these files will improve the
collaboration in your project. Consider adding them:

- `README.md`: include at least the main features, screenshots, getting started
  or links to documentation and build & release instructions.
- `LICENSE`: information about the license. Check
  [choose a license](https://choosealicense.com/).
- [`SECURITY.md`](https://github.com/pleonex/template-csharp/blob/main/SECURITY.md):
  information how to report vulnerabilities properly.
- Community:
  - [`CONTRIBUTING.md`](https://github.com/pleonex/template-csharp/blob/main/CONTRIBUTING.md):
    explain how to create issues and pull requests.
  - `CODE_OF_CONDUCT.md`: GitHub can help to create it.
- IDE support:
  - [`.editorconfig`](https://github.com/pleonex/template-csharp/blob/main/.editorconfig):
    code styles and code warnings.
  - [`.vscode/`](https://github.com/pleonex/template-csharp/tree/main/.vscode):
    VS Code support to build, run and debug the project.
- GitHub issues / PR:
  - [`.github/ISSUE_TEMPLATE/`](https://github.com/pleonex/template-csharp/tree/main/.github/ISSUE_TEMPLATE):
    templates to create GitHub feature requests and bug reports.
  - [`.github/PULL_REQUEST_TEMPLATE.md`](https://github.com/pleonex/template-csharp/blob/main/.github/pull_request_template.md):
    Pull Request template.

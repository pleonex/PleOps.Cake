# .NET environment

As a .NET project grows, it can be tricky to manage it. There are a few
improvements we can do to improve the maintenance. These sections are optional
and they don't affect to the build system.

## Shared project metadata

One interesting option in MSBuild (builder of .NET projects) is to share
properties across .csproj files. If you have a file `Directory.Build.props` in a
root folder, every .csproj projects in subfolders will share its content.

Let's create a file `src/Directory.Build.props` and fill some common info about
our project:

```xml
<Project>
  <PropertyGroup>
    <Product>PRODUCT_NAME</Product>
    <Authors>AUTHOR</Authors>
    <Company>COMPANY</Company>
    <Copyright>Copyright (C) YEAR AUTHOR</Copyright>
  </PropertyGroup>

  <!-- NuGet package common info -->
  <PropertyGroup>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageProjectUrl>https://github.com/pleonex/PleOps.Cake</PackageProjectUrl>
    <RepositoryUrl>https://github.com/pleonex/PleOps.Cake</RepositoryUrl>
    <PackageIcon>icon.png</PackageIcon>
    <PackageTags>net;csharp;devops;pipeline;github-actions</PackageTags>
    <PackageReadmeFile>README.md</PackageReadmeFile>
  </PropertyGroup>

  <!-- Code analyzers -->
  <PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
    <AnalysisLevel>latest</AnalysisLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="StyleCop.Analyzers" PrivateAssets="All"/>
    <PackageReference Include="SonarAnalyzer.CSharp" PrivateAssets="All"/>
    <PackageReference Include="Roslynator.Analyzers" PrivateAssets="All"/>
  </ItemGroup>
</Project>
```

## Centralize dependency management

An option for .NET projects is to configure
[_centralized NuGet packages_](https://github.com/NuGet/Home/wiki/Centrally-managing-NuGet-package-versions).
Instead of defining the version of the _PackageReferences_ in each .csproj, we
will define all of them in a common file. This will help that all your project
use the same version of a dependency.

Create a file `src/Directory.Packages.props` and fill it with your dependencies.
For instance:

```xml
<Project>
  <ItemGroup>
    <PackageVersion Include="nunit" Version="3.14.0" />
  </ItemGroup>
</Project>
```

Then in each .csproj, remove the `Version` attribute in `PackageReference`.

You can still override the version of a dependency in one .csproj by using
`VersionOverride` attribute.

To enable this feature, create/open a file `src/Directory.Build.props` and set
`ManagePackageVersionsCentrally` to `true` inside a `PropertyGroup`.

## SourceLink and deterministic builds

An interesting option for open-source libraries is to enable _source link_. It
allows to inspect the source code while debugging another project. This can also
be interested for internal libraries inside a company project.

_SourceLink_ references are enabled by default in .NET 8, but we define a set of
additional properties to generate a NuGet (a few kB bigger) that it's compatible
with more NuGet feeds (like Azure DevOps).

It's also recommended to enable _deterministic_ builds, which will generate an
identical build output if the code doesn't change. In practice this means the
binary won't have timestamps. It will help to troubleshoot issues in the future.

Set the following properties in `src/Directory.Builds.props`:

```xml
<PropertyGroup Condition="'$(GITHUB_ACTIONS)' == 'true'">
  <!-- Publish the repository URL in the nuget metadata for SourceLink -->
  <PublishRepositoryUrl>true</PublishRepositoryUrl>

  <!-- Embed auto-generated code for SourceLink -->
  <EmbedUntrackedSources>true</EmbedUntrackedSources>

  <!-- For SourceLink and debugging support we don't publish a symbol NuGet
        as some NuGet feeds like Azure DevOps does not provide a symbol server.
        Instead we embed the metadata (PDB) inside the DLLs and EXEs.
        We use this approach instead of providing the .pdb inside the NuGet
        as the latter has known issues with Visual Studio:
        https://github.com/dotnet/sourcelink/issues/628 -->
  <DebugType>embedded</DebugType>

  <!-- Enable deterministic builds -->
  <Deterministic>true</Deterministic>
  <ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>
</PropertyGroup>
```

> [!IMPORTANT]  
> If you are not using .NET 8.0 (yet), add a `PackageReference` (inside an
> `ItemGroup`) to your provider as explained in the
> [sourcelink](https://github.com/dotnet/sourcelink) README.

## README and icon

NuGet packages for libraries and dotnet tools supports providing a README file
and an icon. It will show in Visual Studio and your feed explorer (nuget.org)
when searching for the package.

To enable the feature, set `PackageIcon` and `PackageReadmeFile`. In the
previous `Directory.Build.props` template, we already set it for every project.

Then in each public library / dotnet tool .csproj add:

```xml
<ItemGroup>
  <None Include="../../docs/images/logo_128.png" Pack="true" PackagePath="$(PackageIcon)" Visible="false" />
  <None Include="../../README.md" Pack="true" PackagePath="$(PackageReadmeFile)" />
</ItemGroup>
```

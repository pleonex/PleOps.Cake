# Build context

Cake tasks can use a
[build state](https://cakebuild.net/docs/writing-builds/sharing-build-state) /
context to share information about the build configuration and status.

_PleOps Cake_ uses the class
[`PleOpsBuildContext`](xref:Cake.Frosting.PleOps.Recipe.PleOpsBuildContext) for
its recipe tasks. You can inherit from it to create your own _build state_ or
you can use it directly.

## Defining the context

The
[global build lifetime](https://cakebuild.net/docs/writing-builds/setup-and-teardown)
initializes the build context object. Tasks can consume it or any of its base
types.

Inside the `Setup` method of the lifetime, the design of `PleOpsBuildContext`
allows to set its properties in three stages:

1. Default values that arguments can override. Put the at the top of the method.
   - For instance a custom `artifacts` path or the value of warnings as errors.
     The user will still be able to change it via command-line.
2. Values from the command-line. Call `context.ReadArguments()`.
   - If you inherit the class, you can override this method to extend it.
   - This method will extend the call to the other context classes as well.
3. Hard-coded values. Put them at the bottom of the method.
   - They will override any default or command-line argument value.
   - For instance the NuGet feeds.

At the end of this method call `context.Print()`. It prints in the build output
the final value of the build context that the build will use. Sensitive
properties with the attribute `LogIgnore` will not appear here. Make sure to not
log them in other places.

## Generic context

It provides with some generic build state information. These properties are
defined in the
[`PleOpsBuildContext`](xref:Cake.Frosting.PleOps.Recipe.PleOpsBuildContext)
class.

| Property             | Default value         | CLI argument       | Description                                   |
| -------------------- | --------------------- | ------------------ | --------------------------------------------- |
| `Version`            | `0.0.1`               | `--version`        | Build version. Set by `SetGitVersionTask`     |
| `BuildKind`          | `Development`         |                    | Type of build. Set by `SetGitVersionTask`     |
| `IsIncrementalBuild` | `true` if local build | `--incremental`    | If set, it won't clean artifacts between runs |
| `ArtifactsPath`      | `./build/artifacts`   | `--artifacts`      | Path to store project artifacts               |
| `TemporaryPath`      | `./build/temp`        | `--temp`           | Path to write temporary build files           |
| `RepositoryRootPath` | From `git rev-parse`  |                    | Path to the git root directory                |
| `WarningsAsErrors`   | `true`                | `--warn-as-error`  | Fail on build warnings                        |
| `ChangelogNextFile`  | `./CHANGELOG.NEXT.md` | `--changelog-next` | Path to the changelog of the current release  |
| `ChangelogFile`      | `./CHANGELOG.md`      | `--changelog`      | Path to the full changelog file               |

This class contains also properties to contain the context of other
technologies. Check its documentation page for more details:

- `DotNetContext`: info in [.NET projects](./dotnet.md)
- `DocFxContext`: info in [DocFX projects](./docfx.md)
- `GitHubContext`: info in [GitHub tasks](./github.md)

## Deliveries context

It provides with information about the structure of the artifacts folder. These
properties are defined in the
[`DeliveriesContext`](xref:Cake.Frosting.PleOps.Recipe.DeliveriesContext) class.

| Property             | Default value                   | CLI argument | Description                              |
| -------------------- | ------------------------------- | ------------ | ---------------------------------------- |
| `NuGetArtifactsPath` | `$ArtifactsPath/nuget`          |              | Path to store the created NuGet packages |
| `DocumentationPath`  | `$ArtifactsPath/docs`           |              | Path to store the documentation folder   |
| `DeliveryInfoPath`   | `$ArtifactsPath/artifacts.json` |              | Path to the internal artifacts list JSON |

# Project setup from the template repository

Use the [project template](https://github.com/pleonex/template-csharp) to
initialize a new repository, or adapt an existing one copying its file and
structure. Then follow this checklist to adapt the template to your project.

## Build system

1. Update the build configuration in `build/orchestrator/Program.cs` with your
   project settings (e.g. code coverage or warnings as errors).

## .NET project

1. Rename, edit, remove the project folders inside `src/` and the solution file.
2. Edit the project name and URL in `src/Directory.Build.props`
3. Add, remove, update dependencies in `src/Directory.Build.targets`
4. Update the `.csproj` files with the correct dependencies.
5. Update `build/orchestrator/Program.cs` with the list of publishable .NET
   projects in `ApplicationProjects`.
6. Update `build/orchestrator/Program.cs` with the production and preview NuGet
   feeds, or remove to use nuget.org.

## Documentation

1. Update the icons of the project at `docs/images/`.
2. Update `docs/index.md` with project information
3. Update `docs/articles` with the desired documentation layout.
4. Update `docs/docfx.json` with the path to the API project files.
5. Update `docs/docfx.json` with the project metadata
6. Update `docs/toc.yml` and `docs/template/public/main.js` with the project
   URL.

## Continuous integration

1. Create secret variables with the NuGet tokens in the GitHub project settings
2. Pass your variable's name in the inputs `nuget_stable_token`,
   `nuget_preview_token` or `azure_nuget_token`
3. Review `build.yml` to remove / add OS platforms to run build and tests.
4. Enable GitHub Pages in the repository settings
   1. Select GitHub Actions as the source.

## Collaboration files

1. Update `README.md` title and description.
2. Update `LICENSE` with your desired license and copyright info
3. Update VS Code `.vscode/launch.json` with the path of the console application
   (if any).

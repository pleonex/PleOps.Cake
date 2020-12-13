# Project setup

Use the [project template](https://github.com/pleonex/template-csharp) to
initialize a new repository, or adapt an existing one copying its file and
structure. Then follow this checklist to adapt the template to your project.

## Setup

- [ ] Update README.md: title and description.
- [ ] Update LICENSE: copyright author
- [ ] Rename, edit, remove the project folders inside `src/` and the solution
      file.
- [ ] Edit the project name and URL in `src/Directory.Build.props`
- [ ] Add, remove, update dependencies in `src/Directory.Build.targets`
- [ ] Update the icons of the project at `docs/images/`.
- [ ] Update the `.csproj` files with the correct dependencies.

## Documentation

- [ ] Update the first steps doc at `docs/guides/First-Steps.md`.
- [ ] Update `docs/docfx.json` with the path to the API project files.
- [ ] Update `docs/global_metadata.json` with the properties of the project.
- [ ] Update `docs/toc.yml` with the project URL.

## Build and release workflows

- [ ] Create the milestone `vNext`.
- [ ] Update `launch.json` with the path of the console application (if any).
- [ ] Update `build.cake` to add the name of the projects in the `src` folder.
- [ ] Update `build.cake` with the stable and preview NuGet feeds.
- [ ] Update in the workflow, the variable `PREVIEW_NUGET_FEED` with the NuGet
      feed in Azure DevOps to use for preview releases or remove the step if
      using a different NuGet feed.
- [ ] Add a new secret `NUGET_PREVIEW_TOKEN` with the personal access token for
      the preview NuGet feed.
- [ ] Add a new secret `STABLE_NUGET_FEED_TOKEN` with the API key for the stable
      NuGet feed.

# Building documentation

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

> [!NOTE]  
> If you open the build output in your browser directly (`index.html`), the site
> will look like broken. The generated site requires an HTTP server to see
> properly. You can use the one from DocFX:
> `dotnet docfx serve build/artifacts/docs`

## Changelog

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

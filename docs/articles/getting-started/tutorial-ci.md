# Setting up continuous integration

Nice, now we can fully build, test and bundle our projects locally. Time to
setup a continuous integration pipeline to do the same.

## Workflow triggers

Almost everything is done already by our _Cake-based_ build system. We just need
to install SDKs and run program with the specific task (`Default`, `Bundle` or
`Deploy`). These tasks run at different stages / build types. From the
[proposed workflow](../workflows/pipeline.md) there are three types of builds:

- **Development**: for instance locally or on a pull request validation
  - It runs `Default` to build and test (validate) and `Bundle` to be able to
    download and manually test the deliveries.
- **Preview**: publish the changes on an intermediary / staging feeds. It
  happens on commits / merges into the main branch (`main` / `develop`).
  - Additionally it also runs `Deploy` to push to the preview feeds.
- **Production**: publish stable versions. It happens when we create a git tag
  (e.g. via GitHub release).
  - It also runs `Deploy` but the build system can detect it's for production
    and will run additional tasks.

The build system knows if we are running in each build mode from the version
number (that it's generated from git history and branch names). When we run the
_Deploy_ target, it knows if it should go to _preview_ or _production_.

## Workflow stages

For our workflow, we will need two _stages_ / main jobs: **build** and
**deploy**. For readability, the template project proposed a _reusable workflow_
for each step and then the main file. You can
[check it and copy / adapt it as wanted](https://github.com/pleonex/template-csharp/tree/main/.github/workflows).

- `build.yml`: we can run this steps on multiple OSes to validate the tests on
  them, but only one should upload the artifacts.
  - Clone the repository. Make sure to get all git history so we can generate
    the version number from it.
  - Install .NET SDK (we need it to run our build system).
  - Run the `Build` target.
  - Run the `Bundle` target.
  - Upload the artifacts to the CI for the next stage.
- `deploy.yml`
  - Download the artifacts
  - Upload the documentation to GitHub pages
  - Clone the repository (for the build system and version)
  - Setup .NET SDK (for the build system)
  - Authenticate to the NuGet feeds
  - Run the `Deploy` target.
- `build-and-release.yml`: it just call these two workflows depending on the
  trigger and git branch.

## Workflow requirements

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

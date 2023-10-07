using System.Reflection;
using Cake.Core;
using Cake.Frosting;
using PleOps.Cake.Frosting;

return new CakeHost()
    .AddAssembly(typeof(BuildContext).Assembly)
    .AddAssembly(Assembly.GetAssembly(typeof(Cake.Frosting.Issues.Recipe.IssuesTask)))
    .UseContext<BuildContext>()
    .UseLifetime<BuildLifetime>()
    .Run(args);

public sealed class BuildLifetime : FrostingLifetime<BuildContext>
{
    public override void Setup(BuildContext context, ISetupContext info)
    {
        // HERE you can set default values overridable by command-line
        context.SetGitVersion();

        // Update build parameters from command line arguments.
        context.ReadArguments();

        // HERE you can force values non-overridables.
        context.WarningsAsErrors = false;

        // Print the build info to use.
        context.Print();
    }

    public override void Teardown(BuildContext context, ITeardownContext info)
    {
    }
}


[TaskName("Default")]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.RestoreDependenciesTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.BuildTask))]
public sealed class DefaultTask : FrostingTask
{
}

[TaskName("CI-Build")]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Common.CleanArtifactsTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.GitHubRelease.ExportReleaseNotesTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.DotnetTasks.PrepareProjectBundlesTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.DocFx.DocFxTasks.PrepareProjectBundlesTask))]
[IsDependentOn(typeof(Cake.Frosting.Issues.Recipe.IssuesTask))]
public sealed class CIBuildTask : FrostingTask
{
}

[TaskName("CI-Deploy")]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.DotnetTasks.DeployProjectTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.GitHubRelease.UploadReleaseBinariesTask))]
public sealed class CIDeployTask : FrostingTask
{
}

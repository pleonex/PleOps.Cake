using System.Reflection;
using Cake.Core;
using Cake.Frosting;
using Cake.Frosting.Issues.Recipe;
using PleOps.Cake.Frosting;
using PleOps.Cake.Frosting.Common;

return new CakeHost()
    .AddAssembly(typeof(BuildContext).Assembly)
    .AddAssembly(Assembly.GetAssembly(typeof(IssuesTask)))
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
[IsDependentOn(typeof(CleanArtifactsTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.RestoreDependenciesTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.BuildTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.StageLibrariesTask))]
[IsDependentOn(typeof(IssuesTask))]
public sealed class DefaultTask : FrostingTask
{
}

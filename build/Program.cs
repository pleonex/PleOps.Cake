using Cake.Core;
using Cake.Frosting;
using PleOps.Cake.Frosting;

return new CakeHost()
    .AddAssembly(typeof(BuildContext).Assembly)
    .UseContext<BuildContext>()
    .UseLifetime<BuildLifetime>()
    .Run(args);

public sealed class BuildLifetime : FrostingLifetime<BuildContext>
{
    public override void Setup(BuildContext context, ISetupContext info)
    {
        // HERE: you can set default values overridable by command-line

        // Fill context from command line arguments.
        context.ReadArguments();

        // Set the projects version from GitVersion tool.
        context.SetGitVersion();

        // HERE: you can force values non-overridables.

        // Print the build info to use.
        context.Print();
    }

    public override void Teardown(BuildContext context, ITeardownContext info)
    {
    }
}


[TaskName("Default")]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.BuildTask))]
public sealed class DefaultTask : FrostingTask
{
}

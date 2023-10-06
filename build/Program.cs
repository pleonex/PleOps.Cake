using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using PleOps.Cake.Frosting;

return new CakeHost()
    .AddAssembly(typeof(BuildContext).Assembly)
    .UseContext<MyCustomContext>()
    .UseLifetime<BuildLifetime>()
    .Run(args);

public sealed class MyCustomContext : BuildContext
{
    public MyCustomContext(ICakeContext context)
        : base(context)
    {
        CustomSetting = "DefaultValue";
    }

    public string CustomSetting { get; set; }
}

public sealed class BuildLifetime : FrostingLifetime<MyCustomContext>
{
    public override void Setup(MyCustomContext context, ISetupContext info)
    {
        // HERE: you can set default values overridable by command-line
        context.CustomSetting = "LifetimeValue";

        // Fill context from command line arguments.
        context.ReadArguments();
        context.IfArgIsPresent("custom-setting", x => context.CustomSetting = x);

        // Set the projects version from GitVersion tool.
        context.SetGitVersion();

        // HERE: you can force values non-overridables.
        context.CustomSetting = "ForcedValue";

        // Print the build info to use.
        context.Print();
    }

    public override void Teardown(MyCustomContext context, ITeardownContext info)
    {
    }
}


[TaskName("Default")]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.BuildTask))]
public sealed class DefaultTask : FrostingTask
{
}

[TaskName("CustomTask")]
public sealed class CustomTask : FrostingTask<MyCustomContext>
{
    public override void Run(MyCustomContext context)
    {
        context.Log.Information("Custom setting: {0}", context.CustomSetting);
    }
}

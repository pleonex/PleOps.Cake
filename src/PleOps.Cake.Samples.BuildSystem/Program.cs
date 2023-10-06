using System.Reflection;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.Frosting.Issues.Recipe;
using PleOps.Cake.Frosting;
using PleOps.Cake.Frosting.Common;

return new CakeHost()
    .AddAssembly(typeof(BuildContext).Assembly)
    .AddAssembly(Assembly.GetAssembly(typeof(IssuesTask)))
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
        // HERE you can set default values overridable by command-line
        context.SetGitVersion();
        context.CustomSetting = "LifetimeValue";
        context.CSharpContext.ApplicationProjects.Add(new PleOps.Cake.Frosting.Dotnet.ProjectPublicationInfo(
            "./src/PleOps.Cake.Samples.PublicApp", new[] { "win-x64" }));
        context.CSharpContext.ApplicationProjects.Add(new PleOps.Cake.Frosting.Dotnet.ProjectPublicationInfo(
            "./src/PleOps.Cake.Samples.PublicApp2", new[] { "linux-x64", "osx-x64" }, "net7.0", "CustomApp"));

        // Update build parameters from command line arguments.
        context.ReadArguments();
        context.IfArgIsPresent("custom-setting", x => context.CustomSetting = x);

        // HERE you can force values non-overridables.
        context.CSharpContext.Configuration = "Samples";
        context.CustomSetting = "ForcedValue";
        context.WarningsAsErrors = false;

        // Print the build info to use.
        context.Print();
    }

    public override void Teardown(MyCustomContext context, ITeardownContext info)
    {
    }
}


[TaskName("Default")]
[IsDependentOn(typeof(CleanArtifactsTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.RestoreDependenciesTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.BuildTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.TestTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.StageLibrariesTask))]
[IsDependentOn(typeof(PleOps.Cake.Frosting.Dotnet.StageApplicationsTask))]
[IsDependentOn(typeof(IssuesTask))]
public sealed class DefaultTask : FrostingTask
{
}

public sealed class CustomTask : FrostingTask<MyCustomContext>
{
    public override void Run(MyCustomContext context)
    {
        context.Log.Information("Custom setting: {0}", context.CustomSetting);
    }
}

// Copyright (c) 2023 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System.Reflection;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.Frosting.PleOps.Recipe.Dotnet;

return new CakeHost()
    .AddAssembly(typeof(Cake.Frosting.PleOps.Recipe.BuildContext).Assembly)
    .AddAssembly(Assembly.GetAssembly(typeof(Cake.Frosting.Issues.Recipe.IssuesTask)))
    .UseContext<MyCustomContext>()
    .UseLifetime<BuildLifetime>()
    .Run(args);

public sealed class MyCustomContext : Cake.Frosting.PleOps.Recipe.BuildContext
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
        context.CSharpContext.ApplicationProjects.Add(new ProjectPublicationInfo(
            "./src/Cake.Frosting.PleOps.Samples.PublicApp", new[] { "win-x64" }));
        context.CSharpContext.ApplicationProjects.Add(new ProjectPublicationInfo(
            "./src/Cake.Frosting.PleOps.Samples.PublicApp2", new[] { "linux-x64", "osx-x64" }, "net7.0", "CustomApp"));

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
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.CleanArtifactsTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.GitHubRelease.ExportReleaseNotesTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.PrepareProjectBundlesTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.DocFx.DocFxTasks.PrepareProjectBundlesTask))]
[IsDependentOn(typeof(Cake.Frosting.Issues.Recipe.IssuesTask))]
public sealed class DefaultTask : FrostingTask
{
}

[TaskName("BuildTest")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.BuildTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.TestTask))]
public sealed class BuildTestTask : FrostingTask
{
}

public sealed class CustomTask : FrostingTask<MyCustomContext>
{
    public override void Run(MyCustomContext context)
    {
        context.Log.Information("Custom setting: {0}", context.CustomSetting);
    }
}

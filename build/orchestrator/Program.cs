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
using Cake.Frosting;
using Cake.Frosting.PleOps.Recipe;

return new CakeHost()
    .AddAssembly(typeof(Cake.Frosting.PleOps.Recipe.BuildContext).Assembly)
    .AddAssembly(Assembly.GetAssembly(typeof(Cake.Frosting.Issues.Recipe.IssuesTask)))
    .UseContext<BuildContext>()
    .UseLifetime<BuildLifetime>()
    .Run(args);

public sealed class BuildLifetime : FrostingLifetime<BuildContext>
{
    public override void Setup(BuildContext context, ISetupContext info)
    {
        // HERE you can set default values overridable by command-line

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
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.BuildTask))]
public sealed class DefaultTask : FrostingTask
{
}

[TaskName("CI-Build")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.CleanArtifactsTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.GitHubRelease.ExportReleaseNotesTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.PrepareProjectBundlesTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.DocFx.DocFxTasks.PrepareProjectBundlesTask))]
[IsDependentOn(typeof(Cake.Frosting.Issues.Recipe.IssuesTask))]
public sealed class CIBuildTask : FrostingTask
{
}

[TaskName("CI-Deploy")]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Common.SetGitVersionTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.DotnetTasks.DeployProjectTask))]
[IsDependentOn(typeof(Cake.Frosting.PleOps.Recipe.GitHubRelease.UploadReleaseBinariesTask))]
public sealed class CIDeployTask : FrostingTask
{
}

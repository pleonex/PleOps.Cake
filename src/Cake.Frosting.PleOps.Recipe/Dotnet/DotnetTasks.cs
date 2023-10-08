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
namespace Cake.Frosting.PleOps.Recipe.Dotnet;

using Cake.Frosting;

/// <summary>
/// Tasks to work with .NET projects.
/// </summary>
public static class DotnetTasks
{
    private const string ModuleName = nameof(Dotnet);

    /// <summary>
    /// Gets the name of the install SDK task.
    /// </summary>
    /// <remarks>
    /// The build orchestrator requires the .NET SDK as well as is a .NET project.
    /// It's assummed is already installed and running.
    /// </remarks>
    internal const string InstallSdkTaskName = "";

    /// <summary>
    /// Gets the name of the restore dependencies task.
    /// </summary>
    public const string RestoreTaskName = ModuleName + ".Restore";

    /// <summary>
    /// Gets the name of the build task.
    /// </summary>
    public const string BuildTaskName = ModuleName + ".Build";

    /// <summary>
    /// Gets the name of the test task.
    /// </summary>
    public const string TestTaskName = ModuleName + ".Tests";

    /// <summary>
    /// Gets the name of the bundle NuGets task.
    /// </summary>
    public const string BundleNuGetsTaskName = ModuleName + ".BundleNuGets";

    /// <summary>
    /// Gets the name of the bundle applications task.
    /// </summary>
    public const string BundleAppsTaskName = ModuleName + ".BundleApplications";

    /// <summary>
    /// Gets the name of the deploy NuGet task.
    /// </summary>
    public const string DeployNuGetTaskName = ModuleName + ".DeployNuGet";

    /// <summary>
    /// Run tasks to build, test and bundle projects.
    /// </summary>
    [TaskName(ModuleName + ".PrepareProjectBundles")]
    [IsDependentOn(typeof(RestoreDependenciesTask))]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(TestTask))]
    [IsDependentOn(typeof(BundleNuGetsTask))]
    [IsDependentOn(typeof(BundleApplicationsTask))]
    public class PrepareProjectBundlesTask : FrostingTask
    {
    }

    /// <summary>
    /// Run tasks to deploy the deliveries.
    /// </summary>
    [TaskName(ModuleName + ".DeployProject")]
    [IsDependentOn(typeof(DeployNuGetTask))]
    public class DeployProjectTask : FrostingTask
    {
    }
}

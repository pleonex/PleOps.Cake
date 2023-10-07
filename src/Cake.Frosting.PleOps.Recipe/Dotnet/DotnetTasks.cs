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

public static class DotnetTasks
{
    private const string ModuleName = nameof(Dotnet);

    // The build orchestrator (Cake) requires the .NET SDK as well.
    // It's assummed is already installed and running.
    public const string InstallSdkTaskName = "";

    public const string RestoreTaskName = ModuleName + ".Restore";

    public const string BuildTaskName = ModuleName + ".Build";

    public const string TestTaskName = ModuleName + ".Tests";

    public const string BundleLibsTaskName = ModuleName + ".BundleLibraries";

    public const string BundleAppsTaskName = ModuleName + ".BundleApplications";

    public const string DeployNuGetTaskName = ModuleName + ".DeployNuGet";

    [TaskName(ModuleName + ".PrepareProjectBundles")]
    [IsDependentOn(typeof(RestoreDependenciesTask))]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(TestTask))]
    [IsDependentOn(typeof(BundleLibrariesTask))]
    [IsDependentOn(typeof(BundleApplicationsTask))]
    public class PrepareProjectBundlesTask : FrostingTask
    {
    }

    [TaskName(ModuleName + ".DeployProject")]
    [IsDependentOn(typeof(DeployNuGetTask))]
    public class DeployProjectTask : FrostingTask
    {
    }
}

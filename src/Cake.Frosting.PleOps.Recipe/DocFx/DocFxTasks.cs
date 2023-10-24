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
namespace Cake.Frosting.PleOps.Recipe.DocFx;

using Cake.Frosting;

/// <summary>
/// Tasks to work with DocFx projects.
/// </summary>
/// <remarks>
/// Not supported actions:
/// - InstallSdkTaskName: done via dotnet manifest tool, task already exists.
/// - RestoreTaskName: themes if any would be restore via git submodules at clone time.
/// - TestTaskName: no need. Linting happens while building.
/// - DeployPreview: depends on hosting.
/// - DeployStable: depends on hosting.
/// </remarks>
public static class DocFxTasks
{
    /// <summary>
    /// Gets the module name.
    /// </summary>
    public const string ModuleName = nameof(DocFx);

    /// <summary>
    /// Gets the name of the build task.
    /// </summary>
    public const string BuildTaskName = ModuleName + ".Build";

    /// <summary>
    /// Gets the name of the bundle task.
    /// </summary>
    public const string BundleTaskName = ModuleName + ".Bundle";

    /// <summary>
    /// Run tasks to build, test and bundle projects.
    /// </summary>
    [TaskName(ModuleName + "PrepareProjectBundles")]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(BundleTask))]
    public class PrepareProjectBundlesTask : FrostingTask
    {
    }
}

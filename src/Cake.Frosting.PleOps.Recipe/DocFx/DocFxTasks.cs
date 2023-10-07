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

public static class DocFxTasks
{
    private const string ModuleName = nameof(DocFx);

    // In dotnet manifest via dotnet tools with the rest of tools
    public const string InstallSdkTaskName = "";

    // Themes if any would be restore via git submodules.
    public const string RestoreTaskName = "";

    public const string BuildTaskName = ModuleName + ".Build";

    // No need. Linting happens while building
    public const string TestTaskName = "";

    public const string BundleTaskName = ModuleName + ".Bundle";

    // No need.
    public const string DeployPreview = "";

    // No need. Way simpler via GitHub action.
    public const string DeployStable = "";

    [TaskName(ModuleName + "PrepareProjectBundles")]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(BundleTask))]
    public class PrepareProjectBundlesTask : FrostingTask
    {
    }
}

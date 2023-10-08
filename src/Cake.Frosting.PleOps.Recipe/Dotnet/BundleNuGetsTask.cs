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

using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Pack;
using Cake.Core.Diagnostics;
using Cake.Frosting;

/// <summary>
/// Task to bundle projects as NuGet packages.
/// </summary>
[TaskName(DotnetTasks.BundleNuGetsTaskName)]
[IsDependentOn(typeof(BuildTask))]
public class BundleNuGetsTask : FrostingTask<BuildContext>
{
    /// <inheritdoc />
    public override void Run(BuildContext context)
    {
        string changelog = "Check the project site";
        if (File.Exists(context.ChangelogNextFile)) {
            // Get changelog and sanitize for XML NuSpec
            changelog = File.ReadAllText(context.ChangelogNextFile);
            changelog = System.Security.SecurityElement.Escape(changelog);
        } else {
            context.Log.Information(
                "Changelog for next version does not exist: {0}",
                context.ChangelogNextFile);
        }

        var packSettings = new DotNetPackSettings {
            Configuration = context.DotNetContext.Configuration,
            OutputDirectory = context.DeliveriesContext.NuGetArtifactsPath,
            NoBuild = true,
            MSBuildSettings = new DotNetMSBuildSettings()
                .SetVersion(context.Version)
                .WithProperty("PackageReleaseNotes", changelog),
        };
        context.DotNetPack(context.DotNetContext.SolutionPath, packSettings);

        context.DeliveriesContext.NuGetPackages.Clear();
        if (Directory.Exists(context.DeliveriesContext.NuGetArtifactsPath)) {
            foreach (string nuget in Directory.EnumerateFiles(context.DeliveriesContext.NuGetArtifactsPath, "*.nupkg")) {
                context.DeliveriesContext.NuGetPackages.Add(nuget);
            }
        }
    }
}

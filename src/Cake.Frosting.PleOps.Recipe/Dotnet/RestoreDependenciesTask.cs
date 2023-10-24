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
using Cake.Common.Tools.DotNet.Clean;
using Cake.Common.Tools.DotNet.MSBuild;
using Cake.Common.Tools.DotNet.Restore;
using Cake.Core.Diagnostics;
using Cake.Frosting;

/// <summary>
/// Restore dependencies of .NET projects.
/// </summary>
[TaskName(DotnetTasks.RestoreTaskName)]
public class RestoreDependenciesTask : FrostingTask<PleOpsBuildContext>
{
    /// <inheritdoc />
    public override void Run(PleOpsBuildContext context)
    {
        if (!context.IsIncrementalBuild) {
            context.Log.Information("Cleaning .NET projects for selected configuration and platform");
            var cleanSettings = new DotNetCleanSettings {
                Verbosity = context.DotNetContext.ToolingVerbosity,
                Configuration = context.DotNetContext.Configuration,
                MSBuildSettings = new DotNetMSBuildSettings()
                    .WithProperty("Platform", context.DotNetContext.Platform),
            };
            context.DotNetClean(context.DotNetContext.SolutionPath, cleanSettings);
        }

        MSBuildTreatAllWarningsAs warningMode = context.WarningsAsErrors
            ? MSBuildTreatAllWarningsAs.Error
            : MSBuildTreatAllWarningsAs.Default;

        context.Log.Information("Restoring .NET project dependencies");
        var dotnetSettings = new DotNetRestoreSettings {
            MSBuildSettings = new DotNetMSBuildSettings()
                .SetConfiguration(context.DotNetContext.Configuration)
                .WithProperty("Platform", context.DotNetContext.Platform)
                .TreatAllWarningsAs(warningMode),
            Verbosity = context.DotNetContext.ToolingVerbosity,
        };

        if (!string.IsNullOrEmpty(context.DotNetContext.NugetConfigPath)) {
            dotnetSettings.ConfigFile = context.DotNetContext.NugetConfigPath;
        }

        context.DotNetRestore(context.DotNetContext.SolutionPath, dotnetSettings);
    }
}

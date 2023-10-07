﻿// Copyright (c) 2023 Benito Palacios Sánchez
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

[TaskName(DotnetTasks.RestoreTaskName)]
public class RestoreDependenciesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        if (!context.IsIncrementalBuild) {
            context.Log.Information("Cleaning .NET projects for selected configuration and platform");
            var cleanSettings = new DotNetCleanSettings {
                Verbosity = context.CSharpContext.ToolingVerbosity,
                Configuration = context.CSharpContext.Configuration,
                MSBuildSettings = new DotNetMSBuildSettings()
                    .WithProperty("Platform", context.CSharpContext.Platform),
            };
            context.DotNetClean(context.CSharpContext.SolutionPath, cleanSettings);
        }

        var warningMode = context.WarningsAsErrors
            ? MSBuildTreatAllWarningsAs.Error
            : MSBuildTreatAllWarningsAs.Default;

        context.Log.Information("Restoring .NET project dependencies");
        var dotnetSettings = new DotNetRestoreSettings {
            MSBuildSettings = new DotNetMSBuildSettings()
                .SetConfiguration(context.CSharpContext.Configuration)
                .WithProperty("Platform", context.CSharpContext.Platform)
                .TreatAllWarningsAs(warningMode),
            Verbosity = context.CSharpContext.ToolingVerbosity,
        };

        if (!string.IsNullOrEmpty(context.CSharpContext.NugetConfigPath)) {
            dotnetSettings.ConfigFile = context.CSharpContext.NugetConfigPath;
        }

        context.DotNetRestore(context.CSharpContext.SolutionPath, dotnetSettings);
    }
}
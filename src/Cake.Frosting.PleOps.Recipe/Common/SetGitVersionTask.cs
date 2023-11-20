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
namespace Cake.Frosting.PleOps.Recipe.Common;

using System;
using System.Collections.Generic;
using System.Text.Json;
using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Tools.GitVersion;
using Cake.Core;
using Cake.Core.Diagnostics;

/// <summary>
/// Cake Frosting task to set the project's version from the tool "GitVersion".
/// </summary>
/// <remarks>
/// It requires the dotnet tool: GitVersion.Tool.
/// </remarks>
[TaskName("PleOps.Recipe.Common.SetGitVersion")]
[TaskDescription("Set the project's version using GitVersion tool")]
[IsDependentOn(typeof(RestoreToolsTask))]
public class SetGitVersionTask : FrostingTask<PleOpsBuildContext>
{
    /// <summary>
    /// Run GitVersion tool to get the version.
    /// </summary>
    /// <param name="context">Build context.</param>
    /// <exception cref="Exception">GitVersion failed.</exception>
    /// <exception cref="FormatException">Invalid output from GitVersion.</exception>
    public override void Run(PleOpsBuildContext context)
    {
        // Use Cake GitVersion from the dotnet tool manifest
        // https://github.com/cake-build/cake/issues/3209
        GitVersion GetGitVersion()
        {
            int retcode = context.StartProcess(
                "dotnet",
                new Core.IO.ProcessSettings {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Silent = true,
                    Arguments = "gitversion /nofetch",
                    Timeout = 5_000, // in ms
                },
                out IEnumerable<string> stdOut,
                out IEnumerable<string> stdErr);
            if (retcode != 0) {
                throw new CakeException($"GitVersion returned {retcode}");
            }

            string allOutput = string.Concat(stdOut);
            context.Log.Debug("Output:");
            context.Log.Debug(allOutput);

            context.Log.Debug("Errors:");
            context.Log.Debug(string.Concat(stdErr));

            if (allOutput.Length == 0) {
                throw new FormatException("GitVersion did return nothing ???");
            }

            return JsonSerializer.Deserialize<GitVersion>(allOutput)
                ?? throw new FormatException($"Invalid GitVersion output:\n{allOutput}");
        }

        // Get the version using the tool GitVersion
        // For some reason, sometimes when using a C# script the first time
        // or after updating a NuGet version, it returns nothing.
        GitVersion version;
        try {
            version = GetGitVersion();
        } catch (Exception ex) {
            // A second attempt usually fixes it. Maybe git/dotnet is busy/locking some files...
            context.Log.Information(ex);
            context.Log.Information("Attempting a second time to run GitVersion");
            version = GetGitVersion();
        }

        context.Version = version.SemVer;

        if (string.IsNullOrEmpty(version.PreReleaseLabel)) {
            context.BuildKind = BuildKind.Stable;
        } else if (version.PreReleaseLabel == "preview") {
            context.BuildKind = BuildKind.Preview;
        } else {
            context.BuildKind = BuildKind.Development;
        }

        context.Log.Information("Version: {0}", context.Version);
        context.Log.Information("Build kind: {0}", context.BuildKind);

        // Set the version in the pipeline of Azure Devops
        if (context.AzurePipelines().IsRunningOnAzurePipelines) {
            context.AzurePipelines().Commands.UpdateBuildNumber(context.Version);
        }
    }
}

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

using System.Text;
using Cake.Common.Tools.DotNet;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.Frosting.PleOps.Recipe.Common;

#if CAKE_ISSUES
using Cake.Issues;
using Cake.Issues.DocFx;
#endif

/// <summary>
/// Task to build the DocFx documentation.
/// </summary>
[TaskName(DocFxTasks.BuildTaskName)]
[IsDependentOn(typeof(RestoreToolsTask))]
public class BuildTask : FrostingTask<PleOpsBuildContext>
{
    /// <inheritdoc />
    public override void Run(PleOpsBuildContext context)
    {
        if (!File.Exists(context.DocFxContext.DocFxFile)) {
            context.Log.Warning("There isn't documentation.");
            return;
        }

        if (File.Exists(context.ChangelogFile) && !string.IsNullOrEmpty(context.DocFxContext.ChangelogDocPath)) {
            string changelogDir = Path.GetDirectoryName(context.ChangelogFile)!;
            if (Directory.Exists(changelogDir)) {
                _ = Directory.CreateDirectory(changelogDir);
            }

            File.Copy(context.ChangelogFile, context.DocFxContext.ChangelogDocPath, true);
        }

#if CAKE_ISSUES
        string logPath = Path.Combine(context.TemporaryPath, "docfx_log.txt");
        if (File.Exists(logPath)) {
            File.Delete(logPath);
        }
#endif

        string docfxArgs = new StringBuilder()
            .AppendFormat(" \"{0}\"", context.DocFxContext.DocFxFile)
#if CAKE_ISSUES
            .AppendFormat(" --log \"{0}\"", logPath)
#endif
            .AppendFormat(" --logLevel {0}", context.DocFxContext.ToolingVerbosity)
            .AppendFormat(" {0}", context.WarningsAsErrors ? "--warningsAsErrors" : string.Empty)
            .AppendFormat(" --output \"{0}\"", context.DeliveriesContext.DocumentationPath)
            .ToString();

        context.DotNetTool("docfx" + docfxArgs);

#if CAKE_ISSUES
        // Parse the log file
        string docsDir = Path.GetFullPath(Path.GetDirectoryName(context.DocFxContext.DocFxFile)!);
        var issues = context.ReadIssues(
            context.DocFxIssuesFromFilePath(logPath, docsDir),
            context.IssuesContext.State.RepositoryRootDirectory);
        context.IssuesContext.State.AddIssues(issues);
#endif
    }
}

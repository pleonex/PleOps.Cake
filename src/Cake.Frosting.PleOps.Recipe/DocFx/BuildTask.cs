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

[TaskName(DocFxTasks.BuildTaskName)]
[IsDependentOn(typeof(RestoreToolsTask))]
public class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
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

        string outputDocs = Path.Combine(context.TemporaryPath, "docfx");
        string docfxArgs = new StringBuilder()
            .AppendFormat(" \"{0}\"", context.DocFxContext.DocFxFile)
            .AppendFormat(" --logLevel {0}", context.DocFxContext.ToolingVerbosity)
            .AppendFormat(" {0}", context.WarningsAsErrors ? "--warningsAsErrors" : string.Empty)
            .AppendFormat(" --output \"{0}\"", outputDocs)
            .ToString();

        context.DotNetTool("docfx" + docfxArgs);
    }
}

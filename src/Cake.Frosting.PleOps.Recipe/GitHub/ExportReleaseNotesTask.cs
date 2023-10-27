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
namespace Cake.Frosting.PleOps.Recipe.GitHub;

using System.Text;
using Cake.Common.Tools.DotNet;
using Cake.Core.Diagnostics;
using Cake.Frosting;

/// <summary>
/// Exports the release notes from the GitHub releases.
/// </summary>
[TaskName("PleOps.Recipe.GitHub.ExportReleaseNotes")]
[IsDependentOn(typeof(Common.RestoreToolsTask))]
public class ExportReleaseNotesTask : FrostingTask<PleOpsBuildContext>
{
    /// <inheritdoc />
    public override bool ShouldRun(PleOpsBuildContext context) =>
        !string.IsNullOrEmpty(context.GitHubContext.GitHubToken) &&
        !string.IsNullOrEmpty(context.GitHubContext.RepositoryOwner) &&
        !string.IsNullOrEmpty(context.GitHubContext.RepositoryName);

    /// <inheritdoc />
    public override void Run(PleOpsBuildContext context)
    {
        var argBuilder = new StringBuilder();

        // Export last release to embed in apps and libraries (if exists)
        try {
            string tagName = $"v{context.Version}";
            _ = argBuilder.Append("export ")
                .AppendFormat(" --tagName {0}", tagName)
                .AppendFormat(" --token {0}", context.GitHubContext.GitHubToken)
                .AppendFormat(" --owner {0}", context.GitHubContext.RepositoryOwner)
                .AppendFormat(" --repository {0}", context.GitHubContext.RepositoryName)
                .AppendFormat(" --fileOutputPath \"{0}\"", context.ChangelogNextFile);

            context.Log.Information("Exporting release notes for '{0}'", tagName);
            context.DotNetTool("dotnet-gitreleasemanager " + argBuilder.ToString());
        } catch (Exception ex) {
            context.Log.Warning("Cannot extract latest release notes:\n{0}", ex.ToString());
        }

        // Export full changelog for documentation
        try {
            _ = argBuilder.Clear()
                .Append("export ")
                .AppendFormat(" --token {0}", context.GitHubContext.GitHubToken)
                .AppendFormat(" --owner {0}", context.GitHubContext.RepositoryOwner)
                .AppendFormat(" --repository {0}", context.GitHubContext.RepositoryName)
                .AppendFormat(" --fileOutputPath \"{0}\"", context.ChangelogFile);

            context.Log.Information("Exporting release notes for all versions");
            context.DotNetTool("dotnet-gitreleasemanager " + argBuilder.ToString());
        } catch (Exception ex) {
            context.Log.Warning("Cannot extract release notes:\n{0}", ex.ToString());
        }
    }
}

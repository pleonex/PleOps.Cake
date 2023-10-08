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

using Cake.Common.Tools.GitReleaseManager;
using Cake.Common.Tools.GitReleaseManager.Export;
using Cake.Core.Diagnostics;
using Cake.Frosting;

[TaskName(nameof(GitHub) + ".ExportReleaseNotes")]
public class ExportReleaseNotesTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) =>
        (context.BuildKind is BuildKind.Preview or BuildKind.Stable)
        && !string.IsNullOrEmpty(context.GitHubContext.GitHubToken);

    public override void Run(BuildContext context)
    {
        // Export last release to embed in apps and libraries (if exists)
        try {
            string tagName = $"v{context.Version}";
            context.GitReleaseManagerExport(
                context.GitHubContext.GitHubToken,
                context.GitHubContext.RepositoryOwner,
                context.GitHubContext.RepositoryName,
                context.ChangelogNextFile,
                new GitReleaseManagerExportSettings { TagName = tagName });
        } catch (Exception e) {
            context.Log.Warning("Cannot extract latest release notes:\n{0}", e.ToString());
        }

        // Export full changelog for documentation
        context.GitReleaseManagerExport(
            context.GitHubContext.GitHubToken,
            context.GitHubContext.RepositoryOwner,
            context.GitHubContext.RepositoryName,
            context.ChangelogFile);
    }
}

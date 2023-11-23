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
/// Task to upload delivery binaries to the current GitHub release.
/// </summary>
/// <remarks>
/// The GitHub release should have as a tag "v{Version}".
/// This action only runs for stable builds if the GitHub token is present.
/// </remarks>
[TaskName("PleOps.Recipe.GitHub.UploadReleaseBinaries")]
[IsDependentOn(typeof(Common.RestoreToolsTask))]
public class UploadReleaseBinariesTask : FrostingTask<PleOpsBuildContext>
{
    /// <inheritdoc />
    public override bool ShouldRun(PleOpsBuildContext context) =>
        (context.BuildKind == BuildKind.Stable) &&
        !string.IsNullOrEmpty(context.GitHubContext.GitHubToken);

    /// <inheritdoc />
    public override void Run(PleOpsBuildContext context)
    {
        string tagName = $"v{context.Version}";
        string artifacts = string.Join(
            ",",
            context.DeliveriesContext.BinaryFiles.Select(x => $"\"{x}\""));

        context.Log.Information("Uploading assets: {0}", artifacts);
        string args = new StringBuilder().Append("addasset ")
            .AppendFormat(" --tagName {0}", tagName)
            .AppendFormat(" --token {0}", context.GitHubContext.GitHubToken)
            .AppendFormat(" --owner {0}", context.GitHubContext.RepositoryOwner)
            .AppendFormat(" --repository {0}", context.GitHubContext.RepositoryName)
            .AppendFormat(" --assets {0}", artifacts)
            .ToString();

        context.DotNetTool("dotnet-gitreleasemanager " + args);
    }
}

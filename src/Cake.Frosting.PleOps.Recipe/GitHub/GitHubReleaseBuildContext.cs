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

using Cake.Core;

/// <summary>
/// Build context for GitHub interactions.
/// </summary>
public class GitHubBuildContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubBuildContext"/> class.
    /// </summary>
    public GitHubBuildContext()
    {
        GitHubToken = string.Empty;
        RepositoryOwner = string.Empty;
        RepositoryName = string.Empty;
    }

    /// <summary>
    /// Gets or sets the owner of the repository.
    /// </summary>
    /// <remarks>
    /// This information is filled via 'GITHUB_REPOSITORY' environment variable
    /// set by GitHub action.
    /// </remarks>
    public string RepositoryOwner { get; set; }

    /// <summary>
    /// Gets or sets the name of the repository.
    /// </summary>
    /// <remarks>
    /// This information is filled via 'GITHUB_REPOSITORY' environment variable
    /// set by GitHub action.
    /// </remarks>
    public string RepositoryName { get; set; }

    /// <summary>
    /// Gets or sets the token for GitHub.
    /// </summary>
    [LogIgnore]
    public string GitHubToken { get; set; }

    /// <summary>
    /// Initializes from environment variables.
    /// </summary>
    /// <param name="context">Cake context extensions.</param>
    public void ReadArguments(PleOpsBuildContext context)
    {
        GitHubToken = context.Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        FindRepoInfo(context);
    }

    private void FindRepoInfo(ICakeContext context)
    {
        string? repo = context.Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
        string[]? parts = repo?.Split('/');
        if (repo is null || parts is not { Length: 2 }) {
            return;
        }

        RepositoryOwner = parts[0];
        RepositoryName = parts[1];
    }
}

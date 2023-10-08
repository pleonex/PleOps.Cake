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

using Cake.Core.Diagnostics;

/// <summary>
/// Build context for DocFx documentation projects.
/// </summary>
public class DocFxBuildContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocFxBuildContext"/> class.
    /// </summary>
    public DocFxBuildContext()
    {
        DocFxFile = Path.GetFullPath("./docs/docfx.json");
        ChangelogDocPath = Path.GetFullPath("./docs/dev/changelog.md");
        ToolingVerbosity = "Info";
    }

    /// <summary>
    /// Gets or setse the path to the docfx.json file.
    /// </summary>
    public string DocFxFile { get; set; }

    /// <summary>
    /// Gets or sets the path to put the changelog file in the documentation.
    /// </summary>
    public string ChangelogDocPath { get; set; }

    /// <summary>
    /// Gets or sets the tooling verbosity.
    /// </summary>
    public string ToolingVerbosity { get; set; }

    /// <summary>
    /// Initializes the information from the build context.
    /// </summary>
    /// <param name="context">Cake context.</param>
    /// <exception cref="NotSupportedException">Invalid cake verbosity.</exception>
    public void ReadArguments(BuildContext context)
    {
        // From Cake script verbosity "--verbosity X" flag
        ToolingVerbosity = context.Log.Verbosity switch {
            Verbosity.Normal => "Info",
            Verbosity.Quiet => "Error",
            Verbosity.Minimal => "Warning",
            Verbosity.Verbose => "Verbose",
            Verbosity.Diagnostic => "Verbose",
            _ => throw new NotSupportedException("Unknown Cake verbosity"),
        };
    }
}

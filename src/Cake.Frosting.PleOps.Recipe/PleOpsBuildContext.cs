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
namespace Cake.Frosting.PleOps.Recipe;

using System.Collections;
using System.IO;
using System.Reflection;
using Cake.Common;
using Cake.Common.Build;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.Frosting.PleOps.Recipe.DocFx;
using Cake.Frosting.PleOps.Recipe.Dotnet;
using Cake.Frosting.PleOps.Recipe.GitHub;

#if CAKE_ISSUES
using Cake.Frosting.Issues.Recipe;

public class BuildContext : FrostingContext, IIssuesContext
#endif

/// <summary>
/// Build context information to share across tasks.
/// </summary>
public class PleOpsBuildContext : FrostingContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PleOpsBuildContext"/> class.
    /// </summary>
    /// <param name="context">The context to pass to the base class.</param>
    public PleOpsBuildContext(ICakeContext context)
        : base(context)
    {
        Version = "0.0.1";
        BuildKind = BuildKind.Development;
        IsIncrementalBuild = this.BuildSystem().IsLocalBuild;
        WarningsAsErrors = true;
        ArtifactsPath = Path.GetFullPath("./build/artifacts");
        TemporaryPath = Path.GetFullPath("./build/temp");
        RepositoryRootPath = GetRepositoryRootPath();
        ChangelogNextFile = Path.GetFullPath("./CHANGELOG.NEXT.md");
        ChangelogFile = Path.GetFullPath("./CHANGELOG.md");

        DotNetContext = new DotNetBuildContext();
        DocFxContext = new DocFxBuildContext();
        GitHubContext = new GitHubBuildContext();
        DeliveriesContext = new DeliveriesContext();

#if CAKE_ISSUES
        IssuesContext = new CakeIssuesContext(this);
#endif
    }

    /// <summary>
    /// Gets or sets the build version.
    /// </summary>
    /// <remarks>
    /// GitVersion can set it.
    /// </remarks>
    [LogIgnore]
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets the kind of build to perform.
    /// </summary>
    /// <remarks>
    /// GitVersion can set it.
    /// </remarks>
    public BuildKind BuildKind { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to do an incremental build and
    /// skip cleaning existing artifacts and dependencies.
    /// </summary>
    public bool IsIncrementalBuild { get; set; }

    /// <summary>
    /// Gets or sets the path to directory to write the artifacts.
    /// </summary>
    public string ArtifactsPath { get; set; }

    /// <summary>
    /// Gets or sets a path to the build temporary directory.
    /// </summary>
    public string TemporaryPath { get; set; }

    /// <summary>
    /// Gets or sets the path to the project repository root.
    /// </summary>
    /// <remarks>
    /// Automatically obtained via command-line Git.
    /// </remarks>
    public string RepositoryRootPath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to fail on warnings reported.
    /// </summary>
    public bool WarningsAsErrors { get; set; }

    /// <summary>
    /// Gets or sets the path to the next release changelog file.
    /// </summary>
    public string ChangelogNextFile { get; set; }

    /// <summary>
    /// Gets or sets the path to the full changelog file.
    /// </summary>
    public string ChangelogFile { get; set; }

    /// <summary>
    /// Gets or sets the build context for .NET projects.
    /// </summary>
    public DotNetBuildContext DotNetContext { get; set; }

    /// <summary>
    /// Gets or sets the build context for DocFX projects.
    /// </summary>
    public DocFxBuildContext DocFxContext { get; set; }

    /// <summary>
    /// Gets or sets the build context for GitHub interactions.
    /// </summary>
    public GitHubBuildContext GitHubContext { get; set; }

    /// <summary>
    /// Gets or sets the build context for the project's deliveries.
    /// </summary>
    public DeliveriesContext DeliveriesContext { get; set; }

#if CAKE_ISSUES
    [LogIgnore]
    public CakeIssuesContext IssuesContext { get; }

    IIssuesParameters IIssuesContext.Parameters => IssuesContext.Parameters;

    IIssuesState IIssuesContext.State => IssuesContext.State;
#endif

    /// <summary>
    /// Initialize the build context with command-line arguments if present.
    /// </summary>
    public virtual void ReadArguments()
    {
        Arguments.SetIfPresent("artifacts", x => ArtifactsPath = Path.GetFullPath(x));
        Arguments.SetIfPresent("temp", x => TemporaryPath = Path.GetFullPath(x));
        Arguments.SetIfPresent("version", x => Version = x);
        Arguments.SetIfPresent("warn-as-error", x => WarningsAsErrors = x);
        Arguments.SetIfPresent("incremental", x => IsIncrementalBuild = x);
        Arguments.SetIfPresent("changelog-next", x => ChangelogNextFile = x);
        Arguments.SetIfPresent("changelog", x => ChangelogFile = x);

        DotNetContext.ReadArguments(this);
        DocFxContext.ReadArguments(this);
        GitHubContext.ReadArguments(this);
        DeliveriesContext.Initialize(this);

#if CAKE_ISSUES
        IssuesContext.ReadArguments(this);
#endif
    }

    /// <summary>
    /// Log the non-sensitive information of the build context.
    /// </summary>
    public void Print() => PrintObject(this, 0);

    private void PrintObject(object obj, int indentation = 0)
    {
        IEnumerable<PropertyInfo> properties = obj.GetType().GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance);

        // Ignore properties defined in cake assemblies
        if (indentation == 0) {
            properties = properties.Where(
                p => p.DeclaringType?.IsAssignableTo(typeof(PleOpsBuildContext)) ?? false);
        }

        foreach (PropertyInfo property in properties) {
            object? value = property.GetValue(obj);
            string spaces = new string(' ', indentation);

            bool ignore = Attribute.IsDefined(property, typeof(LogIgnoreAttribute));
            if (ignore) {
                bool isEmpty = (value == null) || string.IsNullOrEmpty(value.ToString());
                Log.Information($"{spaces}{property.Name} is " + (isEmpty ? "not set" : "set"));
                continue;
            }

            if (value is IEnumerable<string> stringList) {
                value = string.Join(", ", stringList);
            }

            if (value is not null and not ValueType and not string) {
                Log.Information($"{spaces}{property.Name}:");

                if (value is IEnumerable list) {
                    int i = 0;
                    foreach (object subValue in list) {
                        Log.Information($"{spaces}- [{i++}]");
                        PrintObject(subValue, indentation + 2);
                    }
                } else {
                    PrintObject(value, indentation + 2);
                }
            } else {
                Log.Information($"{spaces}{property.Name}: '{value}'");
            }
        }
    }

    private string GetRepositoryRootPath()
    {
        int exitCode = this.StartProcess(
            "git",
            new Cake.Core.IO.ProcessSettings {
                Arguments = "rev-parse --show-toplevel",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            },
            out IEnumerable<string> redirectedStandardOutput,
            out IEnumerable<string> redirectedErrorOutput);

        if (exitCode != 0) {
            Log.Warning(
                "Git command failed to obtain root repo path. " +
                $"Exit code: {exitCode}. " +
                $"Error output: {string.Join(System.Environment.NewLine, redirectedErrorOutput)}");
            Log.Warning(
                "The build system will use the current working directory as root. " +
                "Overwrite or ensure git is installed and in the PATH");
            return System.Environment.CurrentDirectory;
        }

        string[] output = redirectedStandardOutput.ToArray();
        if (output.Length != 1) {
            Log.Warning("Invalid output from git to obtain root path. Using current working directory");
            return System.Environment.CurrentDirectory;
        }

        return output[0];
    }
}

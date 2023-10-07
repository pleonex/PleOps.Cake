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
using System.Text.Json;
using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Tools.GitVersion;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;
using Cake.Frosting.Issues.Recipe;
using Cake.Frosting.PleOps.Recipe.DocFx;
using Cake.Frosting.PleOps.Recipe.Dotnet;
using Cake.Frosting.PleOps.Recipe.GitHubRelease;

public class BuildContext : FrostingContext, IIssuesContext
{
    public BuildContext(ICakeContext context)
        : base(context)
    {
        Version = "0.0.1";
        BuildKind = BuildKind.Development;
        IsIncrementalBuild = this.BuildSystem().IsLocalBuild;
        WarningsAsErrors = true;
        ArtifactsPath = Path.GetFullPath("./build/artifacts");
        TemporaryPath = Path.GetFullPath("./build/temp");
        ChangelogNextFile = Path.GetFullPath("./CHANGELOG.NEXT.md");
        ChangelogFile = Path.GetFullPath("./CHANGELOG.md");

        CSharpContext = new CSharpBuildContext();
        DocFxContext = new DocFxBuildContext();
        GitHubReleaseContext = new GitHubReleaseBuildContext();

        IssuesContext = new CakeIssuesContext(this);
    }

    public string Version { get; set; }

    public BuildKind BuildKind { get; set; }

    public bool IsIncrementalBuild { get; set; }

    public string ArtifactsPath { get; set; }

    public string TemporaryPath { get; set; }

    public bool WarningsAsErrors { get; set; }

    public string ChangelogNextFile { get; set; }

    public string ChangelogFile { get; set; }

    public CSharpBuildContext CSharpContext { get; set; }

    public DocFxBuildContext DocFxContext { get; set; }

    public GitHubReleaseBuildContext GitHubReleaseContext { get; set; }

    [LogIgnore]
    public CakeIssuesContext IssuesContext { get; }

    IIssuesParameters IIssuesContext.Parameters => IssuesContext.Parameters;

    IIssuesState IIssuesContext.State => IssuesContext.State;

    public void IfArgIsPresent(string argName, Action<string> setter)
    {
        if (Arguments.HasArgument(argName)) {
            setter(Arguments.GetArgument(argName));
        }
    }

    public void ReadArguments()
    {
        IfArgIsPresent("artifacts", x => ArtifactsPath = Path.GetFullPath(x));
        IfArgIsPresent("temp", x => TemporaryPath = Path.GetFullPath(x));
        IfArgIsPresent("version", x => Version = x);
        WarningsAsErrors = WarningsAsErrors || Arguments.HasArgument("warn-as-error");
        IsIncrementalBuild = IsIncrementalBuild || Arguments.HasArgument("incremental");
        IfArgIsPresent("changelog-next", x => ChangelogNextFile = x);
        IfArgIsPresent("changelog", x => ChangelogFile = x);

        IssuesContext.ReadArguments(this);
        CSharpContext.ReadArguments(this);
        DocFxContext.ReadArguments(this);
        GitHubReleaseContext.ReadArguments(this);
    }

    /// <summary>
    /// Set the project's version from the tool "GitVersion".
    /// </summary>
    /// <remarks>
    /// It requires the dotnet tool: GitVersion.Tool. Make sure to install it
    /// via 'dotnet tool install GitVersion.Tool' or using InstallTool().
    /// </remarks>
    public void SetGitVersion()
    {
        // Use Cake GitVersion from the dotnet tool manifest
        // https://github.com/cake-build/cake/issues/3209
        GitVersion GetGitVersion()
        {
            int retcode = this.StartProcess(
                "dotnet",
                new Cake.Core.IO.ProcessSettings {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Silent = true,
                    Arguments = "gitversion /nofetch"
                },
                out IEnumerable<string> output);
            if (retcode != 0) {
                throw new Exception($"GitVersion returned {retcode}");
            }

            string allOutput = string.Join(string.Empty, output);
            return JsonSerializer.Deserialize<GitVersion>(allOutput)
                ?? throw new FormatException($"Invalid GitVersion output:\n{allOutput}");
        }

        // Get the version using the tool GitVersion
        GitVersion version = GetGitVersion();

        Version = version.SemVer;

        if (string.IsNullOrEmpty(version.PreReleaseLabel)) {
            BuildKind = BuildKind.Stable;
        } else if (version.PreReleaseLabel == "preview") {
            BuildKind = BuildKind.Preview;
        } else {
            BuildKind = BuildKind.Development;
        }

        // Set the version in the pipeline of Azure Devops
        if (this.AzurePipelines().IsRunningOnAzurePipelines) {
            this.AzurePipelines().Commands.UpdateBuildNumber(Version);
        }
    }

    public void Print()
    {
        PrintObject(this, 0);
    }

    private void PrintObject(object obj, int indentation = 0)
    {
        IEnumerable<PropertyInfo> properties = obj.GetType().GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance);

        // Ignore properties defined in cake assemblies
        if (indentation == 0) {
            properties = properties.Where(
                p => p.DeclaringType?.IsAssignableTo(typeof(BuildContext)) ?? false);
        }

        foreach (PropertyInfo property in properties) {
            object? value = property.GetValue(obj);

            bool ignore = Attribute.IsDefined(property, typeof(LogIgnoreAttribute));
            if (ignore) {
                bool isEmpty = (value == null) || string.IsNullOrEmpty(value.ToString());
                Log.Information($"{property.Name} is " + (isEmpty ? "not set" : "set"));
                continue;
            }

            if (value is IEnumerable<string> stringList) {
                value = string.Join(", ", stringList);
            }

            string spaces = new string(' ', indentation);
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
}

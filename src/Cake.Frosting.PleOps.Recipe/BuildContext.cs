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
public class BuildContext : FrostingContext
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

    public string Version { get; set; }

    public BuildKind BuildKind { get; set; }

    public bool IsIncrementalBuild { get; set; }

    public string ArtifactsPath { get; set; }

    public string TemporaryPath { get; set; }

    public string RepositoryRootPath { get; set; }

    public bool WarningsAsErrors { get; set; }

    public string ChangelogNextFile { get; set; }

    public string ChangelogFile { get; set; }

    public DotNetBuildContext DotNetContext { get; set; }

    public DocFxBuildContext DocFxContext { get; set; }

    public GitHubBuildContext GitHubContext { get; set; }

    public DeliveriesContext DeliveriesContext { get; set; }

#if CAKE_ISSUES
    [LogIgnore]
    public CakeIssuesContext IssuesContext { get; }

    IIssuesParameters IIssuesContext.Parameters => IssuesContext.Parameters;

    IIssuesState IIssuesContext.State => IssuesContext.State;
#endif

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

        DotNetContext.ReadArguments(this);
        DocFxContext.ReadArguments(this);
        GitHubContext.ReadArguments(this);
        DeliveriesContext.Initialize(this);

#if CAKE_ISSUES
        IssuesContext.ReadArguments(this);
#endif
    }

    public void Print() => PrintObject(this, 0);

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

    private string GetRepositoryRootPath()
    {
        int exitCode = this.StartProcess(
            "git",
            new Cake.Core.IO.ProcessSettings {
                Arguments = "rev-parse --show-toplevel",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            },
            out IEnumerable<string> redirectedStandardOutput,
            out IEnumerable<string> redirectedErrorOutput);

        if (exitCode != 0) {
            Log.Warning(
                $"Git command failed to obtain root repo path. " +
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

namespace PleOps.Cake.Frosting;

using System.IO;
using System.Reflection;
using System.Text.Json;
using global::Cake.Common;
using global::Cake.Common.Build;
using global::Cake.Common.Tools.GitVersion;
using global::Cake.Core;
using global::Cake.Core.Diagnostics;
using global::Cake.Frosting;
using PleOps.Cake.Frosting.DocFx;
using PleOps.Cake.Frosting.Dotnet;
using PleOps.Cake.Frosting.GitHubRelease;

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
        ChangelogNextFile = Path.GetFullPath("./CHANGELOG.NEXT.md");
        ChangelogFile = Path.GetFullPath("./CHANGELOG.md");

        CSharpContext = new CSharpBuildContext();
        DocFxContext = new DocFxBuildContext();
        GitHubReleaseContext = new GitHubReleaseBuildContext();
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

        CSharpContext.ReadArguments(this);
        DocFxContext.ReadArguments(this);
        GitHubReleaseContext.ReadArguments(this);
    }

    /// <summary>
    /// 
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
                new global::Cake.Core.IO.ProcessSettings {
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
                PrintObject(value, indentation + 2);
            } else {
                Log.Information($"{spaces}{property.Name}: '{value}'");
            }
        }
    }
}

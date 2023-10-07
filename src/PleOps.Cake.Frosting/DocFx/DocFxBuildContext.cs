namespace PleOps.Cake.Frosting.DocFx;

using global::Cake.Core.Diagnostics;

public class DocFxBuildContext
{
    public DocFxBuildContext()
    {
        DocFxFile = Path.GetFullPath("./docs/docfx.json");
        ChangelogDocPath = Path.GetFullPath("./docs/dev/changelog.md");
    }

    public string DocFxFile { get; set; }

    public string ChangelogDocPath { get; set; }

    public string ToolingVerbosity { get; set; }

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

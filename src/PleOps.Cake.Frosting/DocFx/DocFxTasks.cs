namespace PleOps.Cake.Frosting.DocFx;

public static class DocFxTasks
{
    private const string ModuleName = "PleOps.Cake.DocFx";

    // In dotnet manifest via dotnet tools with the rest of tools
    public const string InstallSdkTaskName = "";

    // Themes if any would be restore via git submodules.
    public const string RestoreTaskName = "";

    public const string BuildTaskName = ModuleName + ".Build";

    // No need. Linting happens while building
    public const string TestTaskName = "";

    public const string StageTaskName = ModuleName + ".Stage";

    // TODO: Attach to CI
    public const string DeployPreview = ModuleName + ".DeployPreview";

    // No need. Way simpler via GitHub action.
    public const string DeployStable = "";
}

namespace PleOps.Cake.Frosting.Dotnet;

public static class DotnetTasks
{
    private const string ModuleName = "PleOps.Cake.DotNet";

    // The build orchestrator (Cake) requires the .NET SDK as well.
    // It's assummed is already installed and running.
    public const string InstallSdkTaskName = "";

    public const string RestoreTaskName = ModuleName + ".Restore";

    public const string BuildTaskName = ModuleName + ".Build";

    public const string TestTaskName = ModuleName + ".Tests";

    public const string StageLibsTaskName = ModuleName + ".StageLibraries";

    public const string StageAppsTaskName = ModuleName + ".StageApplications";

    public const string DeployPreviewPreview = ModuleName + ".DeployPreview";

    public const string DeployStable = ModuleName + ".DeployStable";
}

namespace PleOps.Cake.Frosting.Dotnet;

using global::Cake.Frosting;

public static class DotnetTasks
{
    private const string ModuleName = "PleOps.Cake.DotNet";

    // The build orchestrator (Cake) requires the .NET SDK as well.
    // It's assummed is already installed and running.
    public const string InstallSdkTaskName = "";

    public const string RestoreTaskName = ModuleName + ".Restore";

    public const string BuildTaskName = ModuleName + ".Build";

    public const string TestTaskName = ModuleName + ".Tests";

    public const string BundleLibsTaskName = ModuleName + ".BundleLibraries";

    public const string BundleAppsTaskName = ModuleName + ".BundleApplications";

    public const string DeployLibsTaskName = ModuleName + ".DeployLibraries";

    public const string DeployAppsTaskName = ModuleName + ".DeployApplications";

    [TaskName(ModuleName + ".PrepareProjectBundles")]
    [IsDependentOn(typeof(RestoreDependenciesTask))]
    [IsDependentOn(typeof(BuildTask))]
    [IsDependentOn(typeof(TestTask))]
    [IsDependentOn(typeof(BundleLibrariesTask))]
    [IsDependentOn(typeof(BundleApplicationsTask))]
    public class PrepareProjectBundlesTask : FrostingTask
    {
    }

    public const string DeployPreviewPreview = ModuleName + ".DeployPreview";

    public const string DeployStable = ModuleName + ".DeployStable";
}

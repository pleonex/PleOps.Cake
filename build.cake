#load "src/PleOps.Cake/targets.cake"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.AddLibraryProjects("PleOps.Cake");
    info.PreviewNuGetFeed = "https://pkgs.dev.azure.com/benito356/NetDevOpsTest/_packaging/PleOps/nuget/v3/index.json";
});

Task("Default")
    .IsDependentOn("Stage-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);

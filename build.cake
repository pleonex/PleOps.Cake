#load "src/PleOps.Cake/targets.cake"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.AddLibraryProjects("PleOps.Cake");
});

Task("Default")
    .IsDependentOn("Create-Artifacts");

string target = Argument("target", "Default");
RunTarget(target);

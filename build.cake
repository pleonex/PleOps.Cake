#load "pipeline/setup.cake"
#load "pipeline/build.cake"
// #load "pipeline/test.cake"
// #load "pipeline/release.cake"

string target = Argument("target", "Default");

Task("Fill-BuildInfo")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.AddLibraryProjects("MyLibrary");
    info.AddApplicationProjects("MyConsole");
});


Task("Post-Build")
    .Does<BuildInfo>(info =>
{
    Information("Post build ran");
});

Task("Default")
    .IsDependentOn("Fill-BuildInfo")
    .IsDependentOn("Build")
    .IsDependentOn("Post-Build")
    .IsDependentOn("Pack-Libs")
    .IsDependentOn("Pack-Apps");

RunTarget(target);

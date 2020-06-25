// #load "pipeline/targets.cake"
#load "nuget:?package=PleOps.Cake"

Task("Define-Project")
    .Description("Fill specific project information")
    .Does<BuildInfo>(info =>
{
    info.AddLibraryProjects("MyLibrary");
    info.AddApplicationProjects("MyConsole");
    info.AddTestProjects("MyTests");
});


string target = Argument("target", "Default");
RunTarget(target);

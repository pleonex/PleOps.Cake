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
namespace Cake.Frosting.PleOps.Recipe.Dotnet;

using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.NuGet.Push;
using Cake.Core.Diagnostics;
using Cake.Frosting;

[TaskName(DotnetTasks.DeployNuGetTaskName)]
public class DeployNuGetTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) =>
        context.BuildKind != BuildKind.Development;

    public override void Run(BuildContext context)
    {
        string feed = (context.BuildKind == BuildKind.Stable)
            ? context.DotNetContext.StableNuGetFeed
            : context.DotNetContext.PreviewNuGetFeed;

        string token = (context.BuildKind == BuildKind.Stable)
            ? context.DotNetContext.StableNuGetFeedToken
            : context.DotNetContext.PreviewNuGetFeedToken;

        var settings = new DotNetNuGetPushSettings {
            Source = feed,
            ApiKey = token,
            SkipDuplicate = true,
        };

        foreach (string package in context.DeliveriesContext.NuGetLibraries) {
            context.Log.Information("Pushing NuGet: {0}", package);
            context.DotNetNuGetPush(package, settings);
        }
    }
}

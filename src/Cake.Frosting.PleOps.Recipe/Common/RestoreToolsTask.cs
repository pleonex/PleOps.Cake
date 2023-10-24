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
namespace Cake.Frosting.PleOps.Recipe.Common;

using System.Text;
using Cake.Common;
using Cake.Core;
using Cake.Core.IO;

/// <summary>
/// Task to restore or update .NET tools available in the manifest.
/// </summary>
[TaskName(nameof(Common) + ".RestoreTools")]
public class RestoreToolsTask : FrostingTask<BuildContext>
{
    /// <inheritdoc />
    public override void Run(BuildContext context)
    {
        var argBuilder = new StringBuilder()
            .Append(" tool").Append(" restore");

        if (File.Exists(context.DotNetContext.NugetConfigPath)) {
            _ = argBuilder.AppendFormat(" --configfile \"{0}\"", context.DotNetContext.NugetConfigPath);
        }

        int retcode = context.StartProcess(
            "dotnet",
            new ProcessSettings {
                Arguments = argBuilder.ToString(),
            });

        if (retcode != 0) {
            throw new CakeException($"Cannot restore build tools: {retcode}");
        }
    }
}

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
namespace Cake.Frosting.PleOps.Recipe.DocFx;

using System.Linq;
using Cake.Common.IO;
using Cake.Frosting;

/// <summary>
/// Task to bundle the DocFx documentation in a zip file.
/// </summary>
[TaskName(DocFxTasks.BundleTaskName)]
[IsDependentOn(typeof(BuildTask))]
public class BundleTask : FrostingTask<BuildContext>
{
    /// <inheritdoc />
    public override void Run(BuildContext context)
    {
        context.Zip(
            Path.Combine(context.DeliveriesContext.DocumentationPath, "_site"),
            Path.Combine(context.DeliveriesContext.DocumentationPath, "docs.zip"));
    }
}

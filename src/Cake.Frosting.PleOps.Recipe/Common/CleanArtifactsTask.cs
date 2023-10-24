﻿// Copyright (c) 2023 Benito Palacios Sánchez
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

using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;

/// <summary>
/// Task to clean the artifact and temporary folders if it's not an incremental build.
/// </summary>
[TaskName(nameof(Common) + ".CleanArtifacts")]
public class CleanArtifactsTask : FrostingTask<PleOpsBuildContext>
{
    /// <inheritdoc />
    public override bool ShouldRun(PleOpsBuildContext context) =>
        !context.IsIncrementalBuild;

    /// <inheritdoc />
    public override void Run(PleOpsBuildContext context)
    {
        context.Log.Information("Removing artifacts directory");
        context.CleanDirectory(context.TemporaryPath, new CleanDirectorySettings { Force = true });
        context.CleanDirectory(context.ArtifactsPath, new CleanDirectorySettings { Force = true });
    }
}

﻿namespace PleOps.Cake.Frosting.Dotnet;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::Cake.Frosting;

[TaskName(DotnetTasks.BuildTaskName)]
public class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {

    }
}

namespace PleOps.Cake.Frosting.Dotnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public record ProjectPublicationInfo(
    string ProjectPath,
    string[] Runtimes,
    string? Framework = null,
    string? ProjectName = null);

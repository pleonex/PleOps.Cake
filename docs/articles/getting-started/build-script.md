# Orchestrator script

The [setup guide](./tutorial.md) explains how to create the build system as a C#
console project. You may see this as an overkill. Other technologies define the
build system in a simple script. We can also do that in C#.

[dotnet script](https://github.com/dotnet-script/dotnet-script) is a _dotnet
tool_ that allows to run files with C# code without having to create a full
project. You can replace the console application with a C# script with these
steps.

1. Install _dotnet script_: `dotnet tool install dotnet-script`
2. Copy the content of `Program.cs` into a file `build.csx` in the root of the
   repo.
3. At the top of the file define the NuGet dependencies:

   ```cs
   #r "nuget: Cake.Frosting.PleOps.Recipe, 1.0.0
   ```

4. Run the script:

   ```sh
   dotnet script ./build.csx --isolated-load-context
   ```

> [!NOTE]  
> The argument `--isolated-load-context` is required so the script runs with its
> own dependencies instead of re-using the ones from the tool _dotnet-script_.
> Otherwise you may have weird errors with _nuget_ as _dotnet-script_ and _Cake_
> may use different versions of the NuGet libraries.

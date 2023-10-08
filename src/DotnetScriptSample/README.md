# .NET script example

This is an example about how to create a build orchestrator using
[dotnet script](https://github.com/dotnet-script/dotnet-script) and
[Cake.Frosting](https://cakebuild.net/) with the _PleOps.Recipe_.

To use:

1. First install the .NET script tool: `dotnet tool install dotnet-script`.
2. Run the script:

   ```powershell
   dotnet script ./build.csx --isolated-load-context
   ```

> [!NOTE]  
> If you want to test a locally version of the PleOps.Recipe, compile the recipe
> and pass the argument `-s $PWD/build/artifacts` so it restore the nuget
> package from the artifacts instead of nuget.org.

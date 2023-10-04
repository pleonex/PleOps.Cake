namespace PleOps.Cake.Frosting.DocFx;

public class DocFxBuildContext
{
    public DocFxBuildContext()
    {
        DocFxFile = Path.GetFullPath("./docs/docfx.json");
    }

    public string DocFxFile { get; set; }

    public void ReadArguments(BuildContext _)
    {
    }
}

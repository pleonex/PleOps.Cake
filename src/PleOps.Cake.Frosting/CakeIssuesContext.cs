namespace PleOps.Cake.Frosting;

using global::Cake.Frosting.Issues.Recipe;

public class CakeIssuesContext
{
    public CakeIssuesContext(IIssuesContext baseContext)
    {
        Parameters = new IssuesParameters();
        State = new IssuesState(baseContext, RepositoryInfoProviderType.Cli);
    }

    public IIssuesParameters Parameters { get; }

    public IIssuesState State { get; }
}

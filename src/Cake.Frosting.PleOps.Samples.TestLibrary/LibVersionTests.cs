namespace Cake.Frosting.PleOps.Samples.TestLibrary;

using Cake.Frosting.PleOps.Samples.PublicLibrary;
using NUnit.Framework;

[TestFixture]
public class LibVersionTests
{
    [Test]
    public void TestPass()
    {
        Assert.That(LibVersion.GetVersion(), Is.Not.Null);
    }
}

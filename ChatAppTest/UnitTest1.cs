using Microsoft.VisualStudio.TestTools.UnitTesting;
using Avalonia.Diagnostics;

namespace ChatApp;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        long x = 2;
        Assert.AreEqual(x, 2, 0.001, "nada");
    }
}
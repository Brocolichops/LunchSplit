using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchSplit.Tests;

[TestClass]
public class SplitterTests
{
    [TestMethod]
    public void ComputeTip_NoTipMode_ReturnsZero()
    {
        var splitter = new Splitter();
        decimal subtotal = 100m;
        TipMode mode = TipMode.None;
        decimal TipInput = 0m;

        var tip = splitter.ComputeTip(subtotal, mode, TipInput);

        Assert.AreEqual(0m, tip);
    }
}

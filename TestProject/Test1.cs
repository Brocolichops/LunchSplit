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

    [TestMethod]
    public void ComputeTip_PercentTipMode_ReturnsCorrectPercent()
    {
        var splitter = new Splitter();
        decimal subTotal = 100m;
        TipMode mode = TipMode.Percent;
        decimal tipInput = 12m;

        var tip = splitter.ComputeTip(subTotal, mode, tipInput);

        Assert.AreEqual(12, tip);
    }
}

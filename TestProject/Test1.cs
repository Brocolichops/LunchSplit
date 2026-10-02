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


    [TestMethod]
    public void RoundShares_NoRounding_PreservesRawDecimals()
    {
        var rounder = new Rounder();

        var rawShares = new List<Share>
    {
        new Share { Name = "A", Amount = 10.123m },
        new Share { Name = "B", Amount = 20.456m }
    };

        var result = rounder.RoundShares(rawShares, RoundingMode.None);

        Assert.AreEqual(10.123m, result[0].Amount);
        Assert.AreEqual(20.456m, result[1].Amount);
    }

 

[TestMethod]
    public void RoundShares_BankersRounding_RoundsToNearestEven()
    {
        var rounder = new Rounder();

        var rawShares = new List<Share>
    {
        new Share { Name = "A", Amount = 10.325m },
        new Share { Name = "B", Amount = 10.335m }
    };

        var result = rounder.RoundShares(rawShares, RoundingMode.Bankers);

        Assert.AreEqual(10.32m, result[0].Amount);
        Assert.AreEqual(10.34m, result[1].Amount);
    }

    [TestMethod]
    public void RoundShares_RoundUp_AppliesCeilingToCents()
    {
        var rounder = new Rounder();

        var rawShares = new List<Share>
    {
        new Share { Name = "A", Amount = 10.331m }
    };

        var result = rounder.RoundShares(rawShares, RoundingMode.RoundUp);

        Assert.AreEqual(10.34m, result[0].Amount);
    }

    [TestMethod]
    public void RoundShares_RoundDown_AppliesFloorToCents()
    {
        var rounder = new Rounder();

        var rawShares = new List<Share>
    {
        new Share { Name = "A", Amount = 10.339m }
    };

        var result = rounder.RoundShares(rawShares, RoundingMode.RoundDown);

        Assert.AreEqual(10.33m, result[0].Amount);
    }


}

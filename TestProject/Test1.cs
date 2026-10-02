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

    [TestMethod]
    public void RoundShares_UnevenSplit_ReconcilesRemainder()
    {
        var rounder = new Rounder();

        var rawShares = new List<Share>
    {
        new Share { Name = "A", Amount = 3.3333m },
        new Share { Name = "B", Amount = 3.3333m },
        new Share { Name = "C", Amount = 3.3333m }
    };

        var result = rounder.RoundShares(rawShares, RoundingMode.Bankers);

        Assert.AreEqual(3.34m, result[0].Amount);
        Assert.AreEqual(3.33m, result[1].Amount);
        Assert.AreEqual(3.33m, result[2].Amount);

        Assert.AreEqual(10m, result[0].Amount + result[1].Amount + result[2].Amount);
    }

    [TestMethod]
    public void RoundShares_EmptyList_ReturnsEmpty()
    {
        var rounder = new Rounder();

        var rawShares = new List<Share>();

        var result = rounder.RoundShares(rawShares, RoundingMode.None);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void Validate_CompleteBillDetails_ReturnsOk()
    {
        var validator = new BillValidator();

        var bill = new Bill(
            Subtotal: 100m,
            Tax: 0.10m,
            TipMode: TipMode.None,
            TipInput: 0m
        );

        var attendees = new List<Attendee>
    {
        new Attendee { Name = "A", Weight = 1, Included = true }
    };

        var result = validator.Validate(bill, attendees);

        Assert.IsTrue(result.IsValid);
    }

    [TestMethod]
    public void Validate_EmptyAttendeeCollection_ReturnsFail()
    {
        var validator = new BillValidator();

        var bill = new Bill(
            Subtotal: 100m,
            Tax: 0.10m,
            TipMode: TipMode.None,
            TipInput: 0m
        );

        var attendees = new List<Attendee>(); 

        var result = validator.Validate(bill, attendees);

        Assert.IsFalse(result.IsValid);
    }

    [TestMethod]
    public void CalculateShares_EqualSplit_ApportionsEvenly()
    {
        var splitter = new Splitter();

        var attendees = new List<Attendee>
    {
        new Attendee { Name = "A", Weight = 1, Included = true },
        new Attendee { Name = "B", Weight = 1, Included = true },
        new Attendee { Name = "C", Weight = 1, Included = true }
    };

        var bill = new Bill(90m, 0.10m, TipMode.None, 0m);

        var result = splitter.CalculateShares(bill, attendees, RoundingMode.None);

        Assert.AreEqual(33m, result[0].Amount);
        Assert.AreEqual(33m, result[1].Amount);
        Assert.AreEqual(33m, result[2].Amount);
    }

    [TestMethod]
    public void CalculateShares_WeightedSplit_ApportionsByWeight()
    {
        var splitter = new Splitter();

        var attendees = new List<Attendee>
    {
        new Attendee { Name = "A", Weight = 1, Included = true },
        new Attendee { Name = "B", Weight = 2, Included = true },
        new Attendee { Name = "C", Weight = 3, Included = true }
    };

        var bill = new Bill(90m, 0.10m, TipMode.None, 0m);

        var result = splitter.CalculateShares(bill, attendees, RoundingMode.None);

        Assert.AreEqual(16.5m, result[0].Amount);
        Assert.AreEqual(33m, result[1].Amount);
        Assert.AreEqual(49.5m, result[2].Amount);
    }

    [TestMethod]
    public void CalculateShares_ExcludedAttendee_ApportionsZero()
    {
        var splitter = new Splitter();

        var attendees = new List<Attendee>
    {
        new Attendee { Name = "A", Weight = 1, Included = true },
        new Attendee { Name = "B", Weight = 1, Included = false }
    };

        var bill = new Bill(90m, 0.10m, TipMode.None, 0m);

        var result = splitter.CalculateShares(bill, attendees, RoundingMode.None);

        Assert.AreEqual(2, result.Count);

        Assert.AreEqual("A", result[0].Name);
        Assert.AreEqual(99m, result[0].Amount);   

        Assert.AreEqual("B", result[1].Name);
        Assert.AreEqual(0m, result[1].Amount);    
    }

    [TestMethod]
    public void CalculateShares_TaxAndPercentTip_ApportionsTotal()
    {
        var splitter = new Splitter();

        var attendees = new List<Attendee>
    {
        new Attendee { Name = "A", Weight = 1, Included = true },
        new Attendee { Name = "B", Weight = 1, Included = true }
    };

        var bill = new Bill(
            Subtotal: 100m,
            Tax: 0.13m,          // 13% tax
            TipMode: TipMode.Percent,
            TipInput: 15m        // 15% tip
        );

        var result = splitter.CalculateShares(bill, attendees, RoundingMode.None);

        Assert.AreEqual(2, result.Count);

        Assert.AreEqual(64m, result[0].Amount);
        Assert.AreEqual(64m, result[1].Amount);
    }

    [TestMethod]
    public void CalculateShares_TaxAndFixedTip_ApportionsTotal()
    {
        var splitter = new Splitter();

        var attendees = new List<Attendee>
    {
        new Attendee { Name = "A", Weight = 1, Included = true },
        new Attendee { Name = "B", Weight = 1, Included = true }
    };

        var bill = new Bill(
            Subtotal: 100m,
            Tax: 0.13m,          // 13% tax
            TipMode: TipMode.Fixed,
            TipInput: 20m        // $20 fixed tip
        );

        var result = splitter.CalculateShares(bill, attendees, RoundingMode.None);

        Assert.AreEqual(2, result.Count);

        Assert.AreEqual(66.5m, result[0].Amount);
        Assert.AreEqual(66.5m, result[1].Amount);
    }

    [TestMethod]
    public void Format_DefaultBillReceipt_ContainsLineItems()
    {
        var bill = new Bill(100m, 0.13m, TipMode.Percent, 15m);

        var shares = new List<Share>
    {
        new Share { Name = "A", Amount = 64m },
        new Share { Name = "B", Amount = 64m }
    };

        var formatter = new ReceiptFormatter();

        string receipt = formatter.Format(bill, shares);

        Assert.IsTrue(receipt.Contains("Subtotal: 100.00"));
        Assert.IsTrue(receipt.Contains("Tax: 13.00"));
        Assert.IsTrue(receipt.Contains("Tip: 15.00"));
        Assert.IsTrue(receipt.Contains("Total: 128.00"));

        Assert.IsTrue(receipt.Contains("A: 64.00"));
        Assert.IsTrue(receipt.Contains("B: 64.00"));
    }
    [TestMethod]
    public void Format_ReceiptOutput_ContainsRequiredMetadata()
    {
        var bill = new Bill(100m, 0.13m, TipMode.Percent, 15m);

        var shares = new List<Share>
    {
        new Share { Name = "A", Amount = 64m },
        new Share { Name = "B", Amount = 64m }
    };

        var formatter = new ReceiptFormatter();

        string receipt = formatter.Format(bill, shares);

        Assert.IsTrue(receipt.Contains("Date:"));
        Assert.IsTrue(receipt.Contains("Attendees: 2"));
    }

    [TestMethod]
    public void Format_CsvReceiptExporter_MatchesSchema()
    {
        var bill = new Bill(100m, 0.13m, TipMode.Percent, 15m);

        var shares = new List<Share>
    {
        new Share { Name = "A", Amount = 64m },
        new Share { Name = "B", Amount = 64m }
    };

        var exporter = new CsvReceiptExporter();

        string csv = exporter.Format(bill, shares);

        Assert.AreEqual(
            "Subtotal,Tax,Tip,Total\n100.00,13.00,15.00,128.00\nA,64.00\nB,64.00\n",
            csv
        );
    }

    [TestMethod]
    public void CalculateShares_PaymentRequest_ValidatesStripeMock()
    {
        var bill = new Bill(100m, 0.13m, TipMode.Percent, 15m);

        var attendees = new List<Attendee>
    {
        new Attendee { Name = "A", Weight = 1, Included = true },
        new Attendee { Name = "B", Weight = 1, Included = true }
    };

        var splitter = new Splitter();
        var shares = splitter.CalculateShares(bill, attendees, RoundingMode.None);

        var mockStripe = new MockStripeGateway();
        var processor = new PaymentRequestProcessor(mockStripe);

        processor.Process(shares);

        Assert.AreEqual(2, mockStripe.Charges.Count);
        Assert.AreEqual(64m, mockStripe.Charges[0].Amount);
        Assert.AreEqual(64m, mockStripe.Charges[1].Amount);
    }


}

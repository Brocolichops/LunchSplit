using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace LunchSplit;

public enum RoundingMode
{
    None,
    Bankers,
    RoundUp,
    RoundDown
}
public class Splitter
{
    public decimal ComputeTip(decimal subtotal, TipMode mode, decimal tipInput )
    {
        if (mode == TipMode.None)
        {
            return 0m;
        }else if (mode == TipMode.Percent)
        {
            return (subtotal * tipInput) / 100;
        }else
        {
            return tipInput;
        }
    }

    public List<Share> CalculateShares(Bill bill, List<Attendee> attendeeList, RoundingMode roundingMode)
    {
        var included = attendeeList.Where(a => a.Included).ToList();

        decimal taxAmount = bill.Subtotal * bill.Tax;
        decimal tipAmount = ComputeTip(bill.Subtotal, bill.TipMode, bill.TipInput);
        decimal total = bill.Subtotal + taxAmount + tipAmount;

        Dictionary<string, decimal> includedAmounts = new Dictionary<string, decimal>();

        bool allWeightsEqual = included.All(a => a.Weight == included[0].Weight);

        if (allWeightsEqual)
        {
            decimal eachShare = Math.Round(total / included.Count, 2, MidpointRounding.ToEven);

            foreach (var a in included)
                includedAmounts[a.Name] = eachShare;
        }
        else
        {
            decimal totalWeight = included.Sum(a => a.Weight);

            foreach (var a in included)
            {
                decimal portion = (a.Weight / totalWeight) * total;
                portion = Math.Round(portion, 2, MidpointRounding.ToEven);
                includedAmounts[a.Name] = portion;
            }
        }

        var result = new List<Share>();

        foreach (var attendee in attendeeList)
        {
            if (!attendee.Included)
            {
                result.Add(new Share
                {
                    Name = attendee.Name,
                    Amount = 0m
                });
            }
            else
            {
                result.Add(new Share
                {
                    Name = attendee.Name,
                    Amount = includedAmounts[attendee.Name]
                });
            }
        }

        return result;
    }
}
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

        bool allWeightsEqual = included.All(a => a.Weight == included[0].Weight);

        if (allWeightsEqual)
        {
            decimal eachShare = total / included.Count;
            eachShare = Math.Round(eachShare, 2, MidpointRounding.ToEven);

            return included.Select(a => new Share
            {
                Name = a.Name,
                Amount = eachShare
            }).ToList();
        }

        decimal totalWeight = included.Sum(a => a.Weight);

        return included.Select(a =>
        {
            decimal portion = (a.Weight / totalWeight) * total;
            portion = Math.Round(portion, 2, MidpointRounding.ToEven);

            return new Share
            {
                Name = a.Name,
                Amount = portion
            };
        }).ToList();
    }


}
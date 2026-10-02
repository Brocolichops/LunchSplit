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
        // Filter included attendees
        var included = attendeeList.Where(a => a.Included).ToList();

        // M4-17: Equal split among included attendees
        decimal tip = ComputeTip(bill.Subtotal, bill.TipMode, bill.TipInput);
        decimal total = bill.Subtotal + bill.Tax + tip;

        decimal eachShare = total / included.Count;

        var result = new List<Share>();

        foreach (var attendee in included)
        {
            result.Add(new Share
            {
                Name = attendee.Name,
                Amount = eachShare
            });
        }

        return result;
    }

}
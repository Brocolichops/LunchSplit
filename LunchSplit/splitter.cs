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
        throw new NotImplementedException();
    }
}
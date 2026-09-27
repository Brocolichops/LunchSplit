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
        return 0m;
    }

    public List<Share> CalculateShares(Bill bill, List<Attendee> attendeeList, RoundingMode roundingMode)
    {
        throw new NotImplementedException();
    }
}
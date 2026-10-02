using System;
using System.Collections.Generic;
using System.Text;

namespace LunchSplit;

public class Rounder
{
    public List<Share> RoundShares(List<Share> rawShares, RoundingMode mode)
    {
        if (mode == RoundingMode.None) return rawShares;

        throw new NotImplementedException();
    }
}


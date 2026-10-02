using System;
using System.Collections.Generic;
using System.Text;

namespace LunchSplit;

public class Rounder
{
    public List<Share> RoundShares(List<Share> rawShares, RoundingMode mode)
    {
        if (mode == RoundingMode.None) return rawShares;

        if (mode == RoundingMode.Bankers)
        {
            var result = new List<Share>();

            foreach(var share in rawShares)
            {
                decimal rounded = Math.Round(share.Amount, 2, MidpointRounding.ToEven);
                result.Add(new Share
                {
                    Name = share.Name,
                    Amount = rounded
                });
            }
            return result;
        }

        throw new NotImplementedException();
    }
}


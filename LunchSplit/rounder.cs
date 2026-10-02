using System;
using System.Collections.Generic;
using System.Text;

namespace LunchSplit;

public class Rounder
{
    public List<Share> RoundShares(List<Share> rawShares, RoundingMode mode)
    {
        if (rawShares.Count == 0)
            return rawShares;

        if (mode == RoundingMode.None)
            return rawShares;

        if (mode == RoundingMode.Bankers)
        {
            var result = new List<Share>();

            foreach (var share in rawShares)
            {
                decimal rounded = Math.Round(share.Amount, 2, MidpointRounding.ToEven);

                result.Add(new Share
                {
                    Name = share.Name,
                    Amount = rounded
                });
            }

            decimal rawTotal = 0m;
            decimal roundedTotal = 0m;

            foreach (var s in rawShares)
                rawTotal += s.Amount;

            foreach (var s in result)
                roundedTotal += s.Amount;

            rawTotal = Math.Round(rawTotal, 2, MidpointRounding.ToEven);

            decimal difference = rawTotal - roundedTotal;

            if (difference != 0)
                result[0].Amount += difference;

            return result;
        }

        if (mode == RoundingMode.RoundUp)
        {
            var result = new List<Share>();
            foreach (var share in rawShares)
            {
                decimal rounded = Math.Ceiling(share.Amount * 100) / 100;
                result.Add(new Share { Name = share.Name, Amount = rounded });
            }
            return result;
        }

        if (mode == RoundingMode.RoundDown)
        {
            var result = new List<Share>();
            foreach (var share in rawShares)
            {
                decimal rounded = Math.Floor(share.Amount * 100) / 100;
                result.Add(new Share { Name = share.Name, Amount = rounded });
            }
            return result;
        }

        throw new NotImplementedException();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace LunchSplit
{
    public class ReceiptFormatter
    {
        public string Format(Bill bill, List<Share> shares)
        {
            decimal taxAmount = bill.Subtotal * bill.Tax;
            decimal tipAmount = bill.TipMode switch
            {
                TipMode.None => 0m,
                TipMode.Percent => bill.Subtotal * (bill.TipInput / 100m),
                TipMode.Fixed => bill.TipInput,
                _ => 0m
            };

            decimal total = bill.Subtotal + taxAmount + tipAmount;

            var sb = new StringBuilder();

            sb.AppendLine($"Subtotal: {bill.Subtotal:F2}");
            sb.AppendLine($"Tax: {taxAmount:F2}");
            sb.AppendLine($"Tip: {tipAmount:F2}");
            sb.AppendLine($"Total: {total:F2}");
            sb.AppendLine();

            foreach (var share in shares)
            {
                sb.AppendLine($"{share.Name}: {share.Amount:F2}");
            }

            return sb.ToString();
        }
    }
}

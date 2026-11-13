using System.Globalization;
using System.Text;

namespace LunchSplit;

public sealed class CsvReceiptExporter
{
    public string Format(Bill bill, List<Share> shares)
    {
        ArgumentNullException.ThrowIfNull(shares);

        var taxAmount = bill.Subtotal * bill.Tax;
        var tipAmount = bill.TipMode switch
        {
            TipMode.None => 0m,
            TipMode.Percent => bill.Subtotal * (bill.TipInput / 100m),
            TipMode.Fixed => bill.TipInput,
            _ => 0m
        };
        var total = bill.Subtotal + taxAmount + tipAmount;
        var culture = CultureInfo.InvariantCulture;
        var builder = new StringBuilder();

        builder.Append("Subtotal,Tax,Tip,Total\n");
        builder.Append(string.Join(",",
            bill.Subtotal.ToString("F2", culture),
            taxAmount.ToString("F2", culture),
            tipAmount.ToString("F2", culture),
            total.ToString("F2", culture)));
        builder.Append('\n');

        foreach (var share in shares)
        {
            builder.Append($"{share.Name},{share.Amount.ToString("F2", culture)}\n");
        }

        return builder.ToString();
    }
}

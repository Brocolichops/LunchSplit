namespace LunchSplit;

public enum TipMode
{
    None,
    Percent,
    Fixed
};

public record Bill(
    decimal Subtotal, decimal Tax, TipMode TipMode, decimal TipInput
    );

public class Attendee
{
    public string Name { get; set; }
    public int Weight { get; set; }
    public bool Included { get; set; }
}

public class Share
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
}


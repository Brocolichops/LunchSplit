namespace LunchSplit;

public sealed class MockStripeGateway
{
    public List<Charge> Charges { get; } = [];

    public void Charge(decimal amount)
    {
        Charges.Add(new Charge(amount));
    }
}

public sealed record Charge(decimal Amount);

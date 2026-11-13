namespace LunchSplit;

public sealed class PaymentRequestProcessor
{
    private readonly MockStripeGateway gateway;

    public PaymentRequestProcessor(MockStripeGateway gateway)
    {
        this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
    }

    public void Process(IEnumerable<Share> shares)
    {
        ArgumentNullException.ThrowIfNull(shares);

        foreach (var share in shares)
        {
            if (share.Amount > 0)
            {
                gateway.Charge(share.Amount);
            }
        }
    }
}

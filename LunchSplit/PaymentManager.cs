using System.Net.Http;

namespace LunchSplit;

public sealed class PaymentManager
{
    private readonly IPaymentProcessor paymentProcessor;

    public PaymentManager(IPaymentProcessor paymentProcessor)
    {
        this.paymentProcessor = paymentProcessor ?? throw new ArgumentNullException(nameof(paymentProcessor));
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            return await paymentProcessor.ProcessPaymentAsync(request);
        }
        catch (HttpRequestException exception)
        {
            throw new PaymentGatewayTimeoutException(
                "The payment gateway request could not be completed.",
                exception);
        }
    }
}
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
        ArgumentNullException.ThrowIfNull(request);

        if (request.Amount <= 0)
        {
            return new PaymentResult(
                IsSuccess: false,
                ErrorCode: "INVALID_AMOUNT",
                ErrorMessage: "Payment amount must be greater than zero.");
        }

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

    public async Task<IReadOnlyList<PaymentResult>> ProcessBatchAsync(
        IEnumerable<PaymentRequest> requests)
    {
        ArgumentNullException.ThrowIfNull(requests);

        var results = new List<PaymentResult>();
        foreach (var request in requests)
        {
            results.Add(await ProcessPaymentAsync(request));
        }

        return results;
    }
}
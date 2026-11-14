using LunchSplit;
using Square;
using Square.Payments;

namespace LunchSplit.Infrastructure;

public sealed class SquarePaymentService : IPaymentProcessor
{
    private readonly SquareClient client;
    private readonly string locationId;

    public SquarePaymentService(
        string? accessToken = null,
        string? locationId = null,
        TimeSpan? timeout = null)
    {
        accessToken ??= Environment.GetEnvironmentVariable("SQUARE_SANDBOX_ACCESS_TOKEN");
        this.locationId = locationId
            ?? Environment.GetEnvironmentVariable("SQUARE_SANDBOX_LOCATION_ID")
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new InvalidOperationException(
                "SQUARE_SANDBOX_ACCESS_TOKEN must be configured before creating SquarePaymentService.");
        }

        if (string.IsNullOrWhiteSpace(this.locationId))
        {
            throw new InvalidOperationException(
                "SQUARE_SANDBOX_LOCATION_ID must be configured before creating SquarePaymentService.");
        }

        client = new SquareClient(
            accessToken,
            new ClientOptions
            {
                BaseUrl = SquareEnvironment.Sandbox,
                Timeout = timeout ?? TimeSpan.FromSeconds(5),
                MaxRetries = 0
            });
    }

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request), "Payment amount must be greater than zero.");
        }

        try
        {
            var response = await client.Payments.CreateAsync(new CreatePaymentRequest
            {
                SourceId = request.CardNonce,
                IdempotencyKey = request.IdempotencyKey,
                AmountMoney = new Money
                {
                    Amount = checked((long)decimal.Round(request.Amount * 100m, 0, MidpointRounding.ToEven)),
                    Currency = Currency.FromCustom(request.CurrencyCode)
                },
                LocationId = locationId,
                Autocomplete = true
            });

            return new PaymentResult(
                IsSuccess: response.Payment is not null,
                TransactionId: response.Payment?.Id,
                ErrorCode: response.Payment is null ? "PAYMENT_NOT_CREATED" : null,
                ErrorMessage: response.Payment is null ? "Square did not return a payment." : null);
        }
        catch (SquareApiException exception)
        {
            var error = exception.Errors?.FirstOrDefault();
            var errorCode = error is null ? null : error.Code.ToString();
            if (errorCode == "GENERIC_DECLINE")
            {
                errorCode = "CARD_DECLINED";
            }

            return new PaymentResult(
                IsSuccess: false,
                ErrorCode: errorCode,
                ErrorMessage: error?.Detail ?? exception.Message);
        }
    }

    public async Task<PaymentStatus> GetTransactionStatusAsync(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            throw new ArgumentException("A transaction ID is required.", nameof(transactionId));
        }

        try
        {
            var response = await client.Payments.GetAsync(new GetPaymentsRequest
            {
                PaymentId = transactionId
            });

            return new PaymentStatus(
                TransactionId: response.Payment?.Id ?? transactionId,
                Status: response.Payment?.Status?.ToString() ?? "UNKNOWN");
        }
        catch (SquareApiException exception)
        {
            var error = exception.Errors?.FirstOrDefault();
            return new PaymentStatus(
                TransactionId: transactionId,
                Status: "ERROR",
                ErrorCode: error is null ? null : error.Code.ToString(),
                ErrorMessage: error?.Detail ?? exception.Message);
        }
    }
}

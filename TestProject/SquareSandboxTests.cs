using LunchSplit;
using LunchSplit.Infrastructure;

namespace LunchSplit.Tests;

[TestClass]
public class SquareSandboxTests
{
    [TestMethod]
    [TestCategory("Integration")]
    public async Task M3_01_SandboxMagicNonce_CreatesCompletedPayment()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_ACCESS_TOKEN"))
            || string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_LOCATION_ID")))
        {
            Assert.Inconclusive("Square Sandbox environment variables are required for M3-01.");
        }

        var service = new SquarePaymentService();
        var request = new PaymentRequest(
            Amount: 1.00m,
            CurrencyCode: "CAD",
            IdempotencyKey: Guid.NewGuid().ToString(),
            CardNonce: "cnon:card-nonce-ok");

        var result = await service.ProcessPaymentAsync(request);

        Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.TransactionId));
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task M3_02_SandboxDeclinedNonce_ReturnsCardDeclined()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_ACCESS_TOKEN"))
            || string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_LOCATION_ID")))
        {
            Assert.Inconclusive("Square Sandbox environment variables are required for M3-02.");
        }

        var service = new SquarePaymentService();
        var request = new PaymentRequest(
            Amount: 1.00m,
            CurrencyCode: "CAD",
            IdempotencyKey: Guid.NewGuid().ToString(),
            CardNonce: "cnon:card-nonce-declined");

        var result = await service.ProcessPaymentAsync(request);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("CARD_DECLINED", result.ErrorCode);
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task M3_08_SandboxUnsupportedCurrency_ReturnsValidationError()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_ACCESS_TOKEN"))
            || string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_LOCATION_ID")))
        {
            Assert.Inconclusive("Square Sandbox environment variables are required for M3-08.");
        }

        var service = new SquarePaymentService();
        var request = new PaymentRequest(
            Amount: 1.00m,
            CurrencyCode: "EUR",
            IdempotencyKey: Guid.NewGuid().ToString(),
            CardNonce: "cnon:card-nonce-ok");

        var result = await service.ProcessPaymentAsync(request);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorCode));
    }
}

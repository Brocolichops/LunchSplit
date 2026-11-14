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

    [TestMethod]
    [TestCategory("Integration")]
    public async Task M3_09_DuplicateIdempotencyKey_ReturnsSameTransaction()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_ACCESS_TOKEN"))
            || string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_LOCATION_ID")))
        {
            Assert.Inconclusive("Square Sandbox environment variables are required for M3-09.");
        }

        var service = new SquarePaymentService();
        var request = new PaymentRequest(
            Amount: 1.00m,
            CurrencyCode: "CAD",
            IdempotencyKey: $"m3-09-{Guid.NewGuid()}",
            CardNonce: "cnon:card-nonce-ok");

        var firstResult = await service.ProcessPaymentAsync(request);
        var secondResult = await service.ProcessPaymentAsync(request);

        Assert.IsTrue(firstResult.IsSuccess, firstResult.ErrorMessage);
        Assert.IsTrue(secondResult.IsSuccess, secondResult.ErrorMessage);
        Assert.AreEqual(firstResult.TransactionId, secondResult.TransactionId);
    }

    [TestMethod]
    [TestCategory("Integration")]
    public async Task M3_10_SandboxPaymentStatus_ReturnsCompleted()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_ACCESS_TOKEN"))
            || string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SQUARE_SANDBOX_LOCATION_ID")))
        {
            Assert.Inconclusive("Square Sandbox environment variables are required for M3-10.");
        }

        var service = new SquarePaymentService();
        var paymentResult = await service.ProcessPaymentAsync(new PaymentRequest(
            Amount: 1.00m,
            CurrencyCode: "CAD",
            IdempotencyKey: $"m3-10-{Guid.NewGuid()}",
            CardNonce: "cnon:card-nonce-ok"));

        Assert.IsTrue(paymentResult.IsSuccess, paymentResult.ErrorMessage);
        var status = await service.GetTransactionStatusAsync(paymentResult.TransactionId!);

        Assert.AreEqual("COMPLETED", status.Status);
    }
}

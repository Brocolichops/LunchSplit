using System.Net;
using System.Net.Http;
using LunchSplit;
using Moq;

namespace LunchSplit.Tests;

[TestClass]
public class PaymentProcessorMoqTests
{
    [TestMethod]
    public async Task M3_03_GatewayTimeout_ThrowsPaymentGatewayTimeoutException()
    {
        var processor = new Mock<IPaymentProcessor>();
        processor
            .Setup(payment => payment.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ThrowsAsync(new HttpRequestException(
                "The gateway timed out.",
                null,
                HttpStatusCode.RequestTimeout));
        var manager = new PaymentManager(processor.Object);

        var request = new PaymentRequest(
            Amount: 10m,
            CurrencyCode: "CAD",
            IdempotencyKey: Guid.NewGuid().ToString(),
            CardNonce: "cnon:card-nonce-ok");

        await Assert.ThrowsExactlyAsync<PaymentGatewayTimeoutException>(
            () => manager.ProcessPaymentAsync(request));
    }

    [TestMethod]
    public async Task M3_04_ProcessPayment_ForwardsExactRequestAndInvokesOnce()
    {
        var processor = new Mock<IPaymentProcessor>();
        processor
            .Setup(payment => payment.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(new PaymentResult(true, "payment-123"));
        var manager = new PaymentManager(processor.Object);
        var request = new PaymentRequest(
            Amount: 42.50m,
            CurrencyCode: "CAD",
            IdempotencyKey: "m3-04-idempotency-key",
            CardNonce: "cnon:card-nonce-ok");

        var result = await manager.ProcessPaymentAsync(request);

        Assert.IsTrue(result.IsSuccess);
        processor.Verify(payment => payment.ProcessPaymentAsync(
            It.Is<PaymentRequest>(actual =>
                actual.Amount == 42.50m
                && actual.CurrencyCode == "CAD"
                && actual.IdempotencyKey == "m3-04-idempotency-key"
                && actual.CardNonce == "cnon:card-nonce-ok")), Times.Once);
    }

    [TestMethod]
    public async Task M3_05_ZeroDollarPayment_DoesNotInvokeProcessor()
    {
        var processor = new Mock<IPaymentProcessor>();
        var manager = new PaymentManager(processor.Object);
        var request = new PaymentRequest(
            Amount: 0m,
            CurrencyCode: "CAD",
            IdempotencyKey: "m3-05-idempotency-key",
            CardNonce: "cnon:card-nonce-ok");

        var result = await manager.ProcessPaymentAsync(request);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("INVALID_AMOUNT", result.ErrorCode);
        processor.Verify(payment => payment.ProcessPaymentAsync(It.IsAny<PaymentRequest>()), Times.Never);
    }

    [TestMethod]
    public async Task M3_06_FourPaymentBatch_InvokesProcessorExactlyFourTimes()
    {
        var processor = new Mock<IPaymentProcessor>();
        processor
            .Setup(payment => payment.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(new PaymentResult(true, "payment-id"));
        var manager = new PaymentManager(processor.Object);
        var requests = Enumerable.Range(1, 4)
            .Select(index => new PaymentRequest(
                Amount: index * 10m,
                CurrencyCode: "CAD",
                IdempotencyKey: $"m3-06-{index}",
                CardNonce: "cnon:card-nonce-ok"))
            .ToArray();

        var results = await manager.ProcessBatchAsync(requests);

        Assert.AreEqual(4, results.Count);
        processor.Verify(
            payment => payment.ProcessPaymentAsync(It.IsAny<PaymentRequest>()),
            Times.Exactly(4));
    }
}
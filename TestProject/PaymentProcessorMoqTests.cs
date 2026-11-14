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
}
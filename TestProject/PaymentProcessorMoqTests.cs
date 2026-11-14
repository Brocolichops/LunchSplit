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
}
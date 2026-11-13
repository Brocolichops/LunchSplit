namespace LunchSplit;

public sealed record PaymentRequest(
    decimal Amount,
    string CurrencyCode,
    string IdempotencyKey,
    string CardNonce);

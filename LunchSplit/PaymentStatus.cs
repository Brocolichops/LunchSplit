namespace LunchSplit;

public sealed record PaymentStatus(
    string TransactionId,
    string Status,
    string? ErrorCode = null,
    string? ErrorMessage = null);

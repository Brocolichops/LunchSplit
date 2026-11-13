namespace LunchSplit;

public sealed record PaymentResult(
    bool IsSuccess,
    string? TransactionId = null,
    string? ErrorCode = null,
    string? ErrorMessage = null);

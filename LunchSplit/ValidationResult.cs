using System;
using System.Collections.Generic;
using System.Text;

namespace LunchSplit;

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string ErrorMessage { get; private set; }

    private ValidationResult(bool valid, string message)
    {
        IsValid = valid;
        ErrorMessage = message;
    }

    public static ValidationResult Ok()
    {
        return new ValidationResult(true, string.Empty);
    }

    public static ValidationResult Fail(string message)
    {
        return new ValidationResult(false, message);
    }
}

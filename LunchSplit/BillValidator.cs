using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LunchSplit
{
    public class BillValidator
    {
        public ValidationResult Validate(Bill bill, List<Attendee> attendees)
        {

            if (attendees == null || attendees.Count == 0) return ValidationResult.Fail("No attendees");
            return ValidationResult.Ok();
        }
    }
}

using System;
using FluentValidation.Results;

namespace SFA.DAS.Notifications.Application.Exceptions
{
    public class CustomValidationException : ApplicationException
    {
        public CustomValidationException(ValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }

        public ValidationResult ValidationResult { get; set; }
    }
}

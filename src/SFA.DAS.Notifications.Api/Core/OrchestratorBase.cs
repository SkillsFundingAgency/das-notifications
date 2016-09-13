using System;
using FluentValidation.Results;

namespace SFA.DAS.Notifications.Api.Core
{
    public abstract class OrchestratorBase
    {
        protected static OrchestratorResponse GetOrchestratorResponse(string code, ValidationResult validationResult = null)
        {
            return new OrchestratorResponse
            {
                Code = code,
                ValidationResult = validationResult
            };
        }
    }
}

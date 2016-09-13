﻿using System;
using FluentValidation.Results;

namespace SFA.DAS.Notifications.Api.Core
{
    public class OrchestratorResponse
    {
        public string Code { get; set; }

        public ValidationResult ValidationResult { get; set; }
    }
}

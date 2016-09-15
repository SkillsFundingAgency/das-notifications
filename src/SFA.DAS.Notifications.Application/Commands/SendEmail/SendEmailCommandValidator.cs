﻿using System;
using FluentValidation;

namespace SFA.DAS.Notifications.Application.Commands.SendEmail
{
    public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
    {
        public SendEmailCommandValidator()
        {
            RuleFor(model => model.UserId).NotEmpty();
            RuleFor(model => model.TemplateId).NotEmpty();
            RuleFor(model => model.RecipientsAddress).NotEmpty();
            RuleFor(model => model.ReplyToAddress).NotEmpty();
        }
    }
}

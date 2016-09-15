using System;
using FluentValidation;

namespace SFA.DAS.Notifications.Application.Commands.SendEmail
{
    public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
    {
        public SendEmailCommandValidator()
        {
            RuleFor(model => model.SystemId).NotEmpty();
            RuleFor(model => model.Subject).NotEmpty();
            RuleFor(model => model.TemplateId).NotEmpty();
            RuleFor(model => model.RecipientsAddress).NotEmpty();
            RuleFor(model => model.ReplyToAddress).NotEmpty();
        }
    }
}

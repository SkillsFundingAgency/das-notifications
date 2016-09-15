using System;
using FluentValidation;
using SFA.DAS.Notifications.Application.Commands.SendEmail;

namespace SFA.DAS.Notifications.Application.Commands.SendSms
{
    public class SendSmsCommandValidator : AbstractValidator<SendSmsCommand>
    {
        public SendSmsCommandValidator()
        {
            RuleFor(model => model.SystemId).NotEmpty();
            RuleFor(model => model.TemplateId).NotEmpty();
            RuleFor(model => model.RecipientsNumber).NotEmpty();
        }
    }
}

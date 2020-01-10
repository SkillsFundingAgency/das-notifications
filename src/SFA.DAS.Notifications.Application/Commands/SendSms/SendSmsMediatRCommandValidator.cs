using FluentValidation;

namespace SFA.DAS.Notifications.Application.Commands.SendSms
{
    public class SendSmsMediatRCommandValidator : AbstractValidator<SendSmsMediatRCommand>
    {
        public SendSmsMediatRCommandValidator()
        {
            RuleFor(model => model.SystemId).NotEmpty();
            RuleFor(model => model.TemplateId).NotEmpty();
            RuleFor(model => model.RecipientsNumber).NotEmpty();
        }
    }
}

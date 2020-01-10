using FluentValidation;

namespace SFA.DAS.Notifications.Application.Commands.SendEmail
{
    public class SendEmailMediatRCommandValidator : AbstractValidator<SendEmailMediatRCommand>
    {
        public SendEmailMediatRCommandValidator()
        {
            RuleFor(model => model.SystemId).NotEmpty();
            RuleFor(model => model.Subject).NotEmpty();
            RuleFor(model => model.TemplateId).NotEmpty();
            RuleFor(model => model.RecipientsAddress).NotEmpty();
            RuleFor(model => model.ReplyToAddress).NotEmpty();
        }
    }
}

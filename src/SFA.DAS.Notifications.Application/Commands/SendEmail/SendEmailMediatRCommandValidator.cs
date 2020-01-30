using FluentValidation;

namespace SFA.DAS.Notifications.Application.Commands.SendEmail
{
    public class SendEmailMediatRCommandValidator : AbstractValidator<SendEmailMediatRCommand>
    {
        public SendEmailMediatRCommandValidator()
        {
            RuleFor(model => model.TemplateId).NotEmpty();
            RuleFor(model => model.RecipientsAddress).NotEmpty();
        }
    }
}

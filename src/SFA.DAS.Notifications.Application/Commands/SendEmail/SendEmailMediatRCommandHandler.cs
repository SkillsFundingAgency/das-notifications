using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Http;
using SFA.DAS.Notifications.Infrastructure;

namespace SFA.DAS.Notifications.Application.Commands.SendEmail
{
    public class SendEmailMediatRCommandHandler : AsyncRequestHandler<SendEmailMediatRCommand>
    {
        [QueueName]
#pragma warning disable IDE1006 // Naming Styles
        public string send_notifications { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        private readonly ILogger<SendEmailMediatRCommandHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly TemplateConfiguration _templateConfiguration;

        public SendEmailMediatRCommandHandler(
            ILogger<SendEmailMediatRCommandHandler> logger,
            IEmailService emailService,
            TemplateConfiguration templateConfiguration)
        {
            _logger = logger;
            _emailService = emailService;
            _templateConfiguration = templateConfiguration;
        }

        protected override async Task Handle(SendEmailMediatRCommand command, CancellationToken cancellationToken)
        {
            var messageId = Guid.NewGuid().ToString();

            _logger.Log(LogLevel.Information,$"Received command to send email to {command.RecipientsAddress} (message id: {messageId})");

            Validate(command);


            if (!IsGuid(command.TemplateId))
            {
                var emailServiceTemplate = _templateConfiguration.EmailServiceTemplates
                    .SingleOrDefault(
                        t => t.Id.Equals(command.TemplateId, StringComparison.InvariantCultureIgnoreCase)
                    );

                if (emailServiceTemplate == null)
                {
                    throw new ValidationException($"No template mapping could be found for {command.TemplateId}.");
                }
                if (string.IsNullOrEmpty(emailServiceTemplate.EmailServiceId))
                {
                    throw new NullReferenceException($"{nameof(Template.EmailServiceId)} for template {command.TemplateId} is null or empty");
                }
                command.TemplateId = emailServiceTemplate.EmailServiceId;
            }
            else
            {
                // Keep eye on this to make sure consumers migrate
                _logger.Log(LogLevel.Information, $"Request to send template {command.TemplateId} received using email service id");
            }

            try
            {

                await _emailService.SendAsync(new EmailMessage {
                    TemplateId = command.TemplateId,
                    RecipientsAddress = command.RecipientsAddress,
                    Tokens = command.Tokens,
                    Attachments = command.Attachments,
                    Reference = messageId
                });
            }
            catch (Exception ex)
            {
                var httpException = ex as HttpException;

                if (httpException != null && httpException.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    _logger.Log(LogLevel.Warning, ex, "Bad Request - Message will not be re-processed.");
                }
                else
                {
                    throw;
                }
            }

            _logger.Log(LogLevel.Debug, $"Published email message '{messageId}' to queue");
        }

        private void Validate(SendEmailMediatRCommand command)
        {
            var validator = new SendEmailMediatRCommandValidator();

            var validationResult = validator.Validate(command);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }

        private bool IsGuid(string value)
        {
            return Guid.TryParse(value, out _);
        }
    }
}

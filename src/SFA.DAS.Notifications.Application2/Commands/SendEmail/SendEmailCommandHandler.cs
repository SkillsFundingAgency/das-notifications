﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Application2.Interfaces;
using SFA.DAS.Notifications.Domain2.Configuration;
using SFA.DAS.Notifications.Domain2.Entities;
using SFA.DAS.Notifications.Domain2.Http;

namespace SFA.DAS.Notifications.Application2.Commands.SendEmail
{
    public class SendEmailCommandHandler : AsyncRequestHandler<SendEmailCommand>
    {
        [QueueName]
#pragma warning disable IDE1006 // Naming Styles
        public string send_notifications { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        private readonly ILog _logger;
        private readonly IEmailService _emailService;
        private readonly ITemplateConfigurationService _templateConfigurationService;

        public SendEmailCommandHandler(
            ITemplateConfigurationService templateConfigurationService,
            ILog logger,
            IEmailService emailService)
        {
            _templateConfigurationService = templateConfigurationService;
            _logger = logger;
            _emailService = emailService;
        }

        protected override async Task HandleCore(SendEmailCommand command)
        {
            var messageId = Guid.NewGuid().ToString();

            _logger.Info($"Received command to send email to {command.RecipientsAddress} (message id: {messageId})");

            Validate(command);


            if (!IsGuid(command.TemplateId))
            {
                TemplateConfiguration templateConfiguration = await _templateConfigurationService.GetAsync();
                Template emailServiceTemplate = templateConfiguration.EmailServiceTemplates
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
                _logger.Info($"Request to send template {command.TemplateId} received using email service id");
            }

            try
            {

                await _emailService.SendAsync(new EmailMessage {
                    TemplateId = command.TemplateId,
                    SystemId = command.SystemId,
                    Subject = command.Subject,
                    RecipientsAddress = command.RecipientsAddress,
                    ReplyToAddress = command.ReplyToAddress,
                    Tokens = command.Tokens,
                    Reference = messageId
                });
            }
            catch (Exception ex)
            {
                var httpException = ex as HttpException;

                if (httpException != null && httpException.StatusCode.Equals(HttpStatusCode.BadRequest))
                {
                    _logger.Warn(ex, "Bad Request - Message will not be re-processed.");
                }
                else
                {
                    throw;
                }
            }

            _logger.Debug($"Published email message '{messageId}' to queue");
        }

        private void Validate(SendEmailCommand command)
        {
            var validator = new SendEmailCommandValidator();

            var validationResult = validator.Validate(command);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }
        private bool IsGuid(string value)
        {
            Guid x;
            return Guid.TryParse(value, out x);
        }
    }
}

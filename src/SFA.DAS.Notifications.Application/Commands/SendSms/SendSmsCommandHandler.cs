using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Infrastructure;

namespace SFA.DAS.Notifications.Application.Commands.SendSms
{
    public class SendSmsCommandHandler : AsyncRequestHandler<SendSmsCommand>
    {
        [QueueName]
#pragma warning disable IDE1006 // Naming Styles
        public string send_notifications { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        private readonly ILogger _logger;
        private readonly TemplateConfiguration _templateConfiguration;
        private readonly ISmsService _smsService;

        public SendSmsCommandHandler(
            TemplateConfiguration templateConfiguration,
            ILogger logger, ISmsService smsService)
        {
            _templateConfiguration = templateConfiguration;
            _logger = logger;
            _smsService = smsService;
        }

        protected override async Task Handle(SendSmsCommand command, CancellationToken cancellationToken)
        {
            var messageId = Guid.NewGuid().ToString();

            _logger.Log(LogLevel.Information, $"Received command to send SMS to {command.RecipientsNumber} (message id: {messageId})");

            Validate(command);

            SmsTemplate template = _templateConfiguration.SmsServiceTemplates
                .SingleOrDefault(x => string.Equals(command.TemplateId, x.Id, StringComparison.InvariantCultureIgnoreCase));

            if (template == null)
            {
                throw new ValidationException($"No template mapping could be found for {command.TemplateId}.");
            }
            if (string.IsNullOrEmpty(template.ServiceId))
            {
                throw new NullReferenceException($"Configuration for template {command.TemplateId} has no ServiceId.");
            }
            command.TemplateId = template.ServiceId;

            _logger.Log(LogLevel.Debug, $"Stored SMS message '{messageId}' in data store");

            await _smsService.SendAsync(new SmsMessage
            {
                TemplateId = command.TemplateId,
                SystemId = command.SystemId,
                RecipientsNumber = command.RecipientsNumber,
                Tokens = command.Tokens,
                Reference = messageId
            });

            _logger.Log(LogLevel.Debug, $"Published SMS message '{messageId}' to queue");
        }

        private void Validate(SendSmsCommand command)
        {
            var validator = new SendSmsCommandValidator();

            var validationResult = validator.Validate(command);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }
    }
}

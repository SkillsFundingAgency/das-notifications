using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Application2.Interfaces;
using SFA.DAS.Notifications.Domain2.Configuration;
using SFA.DAS.Notifications.Domain2.Entities;

namespace SFA.DAS.Notifications.Application2.Commands.SendSms
{
    public class SendSmsCommandHandler : AsyncRequestHandler<SendSmsCommand>
    {
        [QueueName]
#pragma warning disable IDE1006 // Naming Styles
        public string send_notifications { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        private readonly ILog _logger;
        private readonly ITemplateConfigurationService _templateConfigurationService;
        private readonly ISmsService _smsService;

        public SendSmsCommandHandler(
            ITemplateConfigurationService templateConfigurationService,
            ILog logger, ISmsService smsService)
        {
            _templateConfigurationService = templateConfigurationService;
            _logger = logger;
            _smsService = smsService;
        }

        protected override async Task HandleCore(SendSmsCommand command)
        {
            var messageId = Guid.NewGuid().ToString();

            _logger.Info($"Received command to send SMS to {command.RecipientsNumber} (message id: {messageId})");

            Validate(command);

            var templateConfiguration = await _templateConfigurationService.GetAsync();
            SmsTemplate template = templateConfiguration.SmsServiceTemplates
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

            _logger.Debug($"Stored SMS message '{messageId}' in data store");

            await _smsService.SendAsync(new SmsMessage {
                TemplateId = command.TemplateId,
                SystemId = command.SystemId,
                RecipientsNumber = command.RecipientsNumber,
                Tokens = command.Tokens,
                Reference = messageId
            });

            _logger.Debug($"Published SMS message '{messageId}' to queue");
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

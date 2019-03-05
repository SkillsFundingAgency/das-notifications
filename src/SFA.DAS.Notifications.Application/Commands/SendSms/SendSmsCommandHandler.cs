using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Application.Messages;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Repositories;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.Notifications.Application.Commands.SendSms
{
    public class SendSmsCommandHandler : AsyncRequestHandler<SendSmsCommand>
    {
        [QueueName]
#pragma warning disable IDE1006 // Naming Styles
        public string send_notifications { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        private readonly ILog _logger;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ITemplateConfigurationService _templateConfigurationService;

        public SendSmsCommandHandler(
            INotificationsRepository notificationsRepository,
            IMessagePublisher messagePublisher,
            ITemplateConfigurationService templateConfigurationService,
            ILog logger)
        {
            _notificationsRepository = notificationsRepository;
            _messagePublisher = messagePublisher;
            _templateConfigurationService = templateConfigurationService;
            _logger = logger;
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

            await _notificationsRepository.Create(CreateMessageData(command, messageId));

            _logger.Debug($"Stored SMS message '{messageId}' in data store");

            await _messagePublisher.PublishAsync(new DispatchNotificationMessage {
                MessageId = messageId,
                Format = NotificationFormat.Sms
            });

            _logger.Debug($"Published SMS message '{messageId}' to queue");
        }

        private static Notification CreateMessageData(SendSmsCommand message, string messageId)
        {
            return new Notification {
                MessageId = messageId,
                SystemId = message.SystemId,
                Timestamp = DateTimeProvider.Current.UtcNow,
                Status = NotificationStatus.New,
                Format = NotificationFormat.Sms,
                TemplateId = message.TemplateId,
                Data = JsonConvert.SerializeObject(new NotificationSmsContent {
                    RecipientsNumber = message.RecipientsNumber,
                    Tokens = message.Tokens
                })
            };
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

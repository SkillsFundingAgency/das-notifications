using System;
using System.Threading.Tasks;
using FluentValidation.Results;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Messaging;
using SFA.DAS.Notifications.Application.Exceptions;
using SFA.DAS.Notifications.Application.Messages;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Repositories;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.Notifications.Application.Commands.SendSms
{
    public class SendSmsCommandHandler : AsyncRequestHandler<SendSmsCommand>
    {
        [QueueName]
        public string send_notifications { get; set; }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly INotificationsRepository _notificationsRepository;
        private readonly IMessagePublisher _messagePublisher;

        public SendSmsCommandHandler(INotificationsRepository notificationsRepository, IMessagePublisher messagePublisher)
        {
            if (notificationsRepository == null)
                throw new ArgumentNullException(nameof(notificationsRepository));
            if (messagePublisher == null)
                throw new ArgumentNullException(nameof(messagePublisher));

            _notificationsRepository = notificationsRepository;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(SendSmsCommand command)
        {
            var messageId = Guid.NewGuid().ToString();

            Logger.Debug($"Received command to send SMS to {command.RecipientsNumber} (message id: {messageId})");

            var validationResult = Validate(command);

            if (!validationResult.IsValid)
                throw new CustomValidationException(validationResult);

            await _notificationsRepository.Create(CreateMessageData(command, messageId));

            Logger.Debug($"Stored message '{messageId}' in data store");

            await _messagePublisher.PublishAsync(new DispatchNotificationMessage
            {
                MessageId = messageId,
                Format = NotificationFormat.Sms
            });

            Logger.Debug($"Published message '{messageId}' to queue");
        }

        private static Notification CreateMessageData(SendSmsCommand message, string messageId)
        {
            return new Notification
            {
                MessageId = messageId,
                SystemId = message.SystemId,
                Timestamp = DateTimeProvider.Current.UtcNow,
                Status = NotificationStatus.New,
                Format = NotificationFormat.Sms,
                TemplateId = message.TemplateId,
                Data = JsonConvert.SerializeObject(new NotificationSmsContent
                {
                    RecipientsNumber = message.RecipientsNumber,
                    Tokens = message.Tokens
                })
            };
        }

        private ValidationResult Validate(SendSmsCommand command)
        {
            var validator = new SendSmsCommandValidator();

            return validator.Validate(command);
        }
    }
}

using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Messaging;
using SFA.DAS.Notifications.Application.Messages;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Domain.Repositories;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.Notifications.Application.Commands.SendEmail
{
    public class SendEmailCommandHandler : AsyncRequestHandler<SendEmailCommand>
    {
        [QueueName]
        public string send_notifications { get; set; }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly INotificationsRepository _notificationsRepository;
        private readonly IMessagePublisher _messagePublisher;

        public SendEmailCommandHandler(INotificationsRepository notificationsRepository, IMessagePublisher messagePublisher)
        {
            if (notificationsRepository == null)
                throw new ArgumentNullException(nameof(notificationsRepository));
            if (messagePublisher == null)
                throw new ArgumentNullException(nameof(messagePublisher));

            _notificationsRepository = notificationsRepository;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(SendEmailCommand command)
        {
            var messageId = Guid.NewGuid().ToString();

            Logger.Info($"Received command to send email to {command.RecipientsAddress} (message id: {messageId})");

            Validate(command);

            await _notificationsRepository.Create(CreateMessageData(command, messageId));

            Logger.Debug($"Stored message '{messageId}' in data store");

            await _messagePublisher.PublishAsync(new DispatchNotificationMessage
            {
                MessageId = messageId,
                Format = NotificationFormat.Email
            });

            Logger.Debug($"Published message '{messageId}' to queue");
        }

        private static Notification CreateMessageData(SendEmailCommand message, string messageId)
        {
            return new Notification
            {
                MessageId = messageId,
                SystemId = message.SystemId,
                Timestamp = DateTimeProvider.Current.UtcNow,
                Status = NotificationStatus.New,
                Format = NotificationFormat.Email,
                TemplateId = message.TemplateId,
                Data = JsonConvert.SerializeObject(new NotificationEmailContent
                {
                    Subject = message.Subject,
                    RecipientsAddress = message.RecipientsAddress,
                    ReplyToAddress = message.ReplyToAddress,
                    Tokens = message.Tokens
                })
            };
        }

        private void Validate(SendEmailCommand command)
        {
            var validator = new SendEmailCommandValidator();

            var validationResult = validator.Validate(command);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }
    }
}

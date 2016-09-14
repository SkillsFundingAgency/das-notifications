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

        protected override async Task HandleCore(SendEmailCommand message)
        {
            var messageId = Guid.NewGuid().ToString();

            Logger.Debug($"Received message type {message.MessageType} to send to {message.RecipientsAddress} (message id: {messageId})");

            var validationResult = Validate(message);

            if (!validationResult.IsValid)
                throw new CustomValidationException(validationResult);

            await _notificationsRepository.Create(CreateMessageData(message, messageId));

            Logger.Debug($"Stored message '{messageId}' in data store");

            await _messagePublisher.PublishAsync(new QueuedNotificationMessage
            {
                MessageType = message.MessageType,
                MessageId = messageId
            });

            Logger.Debug($"Published message '{messageId}' to queue");
        }

        private static Notification CreateMessageData(SendEmailCommand message, string messageId)
        {
            return new Notification
            {
                MessageId = messageId,
                MessageType = message.MessageType,
                Content = new NotificationContent
                {
                    UserId = message.UserId,
                    Timestamp = DateTimeProvider.Current.UtcNow,
                    Format = NotificationFormat.Email,
                    TemplateId = message.TemplateId,
                    Data = JsonConvert.SerializeObject(new EmailMessageContent
                    {
                        RecipientsAddress = message.RecipientsAddress,
                        ReplyToAddress = message.ReplyToAddress,
                        Tokens = message.Tokens
                    })
                }
            };
        }

        private ValidationResult Validate(SendEmailCommand cmd)
        {
            var validator = new SendEmailCommandValidator();

            return validator.Validate(cmd);
        }
    }
}

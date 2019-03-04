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

namespace SFA.DAS.Notifications.Application.Commands.SendEmail
{
    public class SendEmailCommandHandler : AsyncRequestHandler<SendEmailCommand>
    {
        [QueueName]
#pragma warning disable IDE1006 // Naming Styles
        public string send_notifications { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        private readonly ILog _logger;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ITemplateConfigurationService _templateConfigurationService;

        public SendEmailCommandHandler(
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
                    throw new ValidationException($"No template mapping could be found for {command.TemplateId}");
                }
                if (string.IsNullOrEmpty(emailServiceTemplate.EmailServiceId))
                {
                    throw new NullReferenceException($"{nameof(Template.EmailServiceId)} for template {command.TemplateId} is null or empty");
                }
                command.TemplateId = emailServiceTemplate.Id;
            }
            else
            {
                // Keep eye on this to make sure consumers migrate
                _logger.Info($"Request to send template {command.TemplateId} received using email service id");
            }

            await _notificationsRepository.Create(CreateMessageData(command, messageId));

            _logger.Debug($"Stored email message '{messageId}' in data store");

            await _messagePublisher.PublishAsync(new DispatchNotificationMessage {
                MessageId = messageId,
                Format = NotificationFormat.Email
            });

            _logger.Debug($"Published email message '{messageId}' to queue");
        }

        private static Notification CreateMessageData(SendEmailCommand message, string messageId)
        {
            return new Notification {
                MessageId = messageId,
                SystemId = message.SystemId,
                Timestamp = DateTimeProvider.Current.UtcNow,
                Status = NotificationStatus.New,
                Format = NotificationFormat.Email,
                TemplateId = message.TemplateId,
                Data = JsonConvert.SerializeObject(new NotificationEmailContent {
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
        private bool IsGuid(string value)
        {
            Guid x;
            return Guid.TryParse(value, out x);
        }
    }
}

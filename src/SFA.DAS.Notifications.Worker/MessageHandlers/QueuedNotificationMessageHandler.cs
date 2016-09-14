using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Messaging;
using SFA.DAS.Notifications.Application.Commands;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Application.Messages;
using SFA.DAS.Notifications.Application.Queries.GetMessage;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Worker.MessageHandlers
{
    public class QueuedNotificationMessageHandler
    {
        [QueueName]
        public string send_notifications { get; set; }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;
        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IEmailService _emailService;

        public QueuedNotificationMessageHandler(IMediator mediator, IPollingMessageReceiver pollingMessageReceiver, IEmailService emailService)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (pollingMessageReceiver == null)
                throw new ArgumentNullException(nameof(pollingMessageReceiver));
            if (emailService == null)
                throw new ArgumentNullException(nameof(emailService));

            _mediator = mediator;
            _pollingMessageReceiver = pollingMessageReceiver;
            _emailService = emailService;
        }

        public async Task Handle()
        {
            Logger.Info($"Received message?");

            var message = await _pollingMessageReceiver.ReceiveAsAsync<QueuedNotificationMessage>();

            if (message?.Content != null)
            {
                try
                {
                    Logger.Info($"Received message {message.Content.MessageId}");

                    var savedMessage = await _mediator.SendAsync(new GetMessageQueryRequest
                    {
                        MessageType = message.Content.MessageType,
                        MessageId = message.Content.MessageId
                    });

                    var notificationFormat = savedMessage.Content?.Format;

                    switch (notificationFormat)
                    {
                        case NotificationFormat.Email:
                            var emailContent = JsonConvert.DeserializeObject<EmailMessageContent>(savedMessage.Content.Data);

                            await _emailService.SendAsync(new EmailMessage
                            {
                                MessageType = savedMessage.MessageType,
                                TemplateId = savedMessage.Content.TemplateId,
                                UserId = savedMessage.Content.UserId,
                                RecipientsAddress = emailContent.RecipientsAddress,
                                ReplyToAddress = emailContent.ReplyToAddress,
                                Data = emailContent.Data
                            });
                            break;
                        case NotificationFormat.Sms:
                            var smsContent = JsonConvert.DeserializeObject<SmsMessage>(savedMessage.Content.Data);

                            throw new NotImplementedException();
                        default:
                            throw new ArgumentOutOfRangeException(nameof(notificationFormat), "Unsupported notification format");
                    }

                    await message.CompleteAsync();

                    Logger.Info($"Finished processing message {message.Content.MessageId}");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Error processing message {message.Content.MessageId} - {ex.Message}");
                }
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Messaging;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Application.Messages;
using SFA.DAS.Notifications.Application.Queries.GetMessage;
using SFA.DAS.Notifications.Domain;

namespace SFA.DAS.Notifications.Worker
{
    public class QueuedMessageHandler
    {
        [QueueName]
        public string send_notifications { get; set; }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;
        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IEmailService _emailService;

        public QueuedMessageHandler(IMediator mediator, IPollingMessageReceiver pollingMessageReceiver, IEmailService emailService)
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
            var message = await _pollingMessageReceiver.ReceiveAsAsync<QueueMessage>();

            if (message?.Content != null)
            {
                Logger.Info($"Received message {message.Content.MessageId}");

                try
                {
                    var savedMessage = await _mediator.SendAsync(new GetMessageQueryRequest
                    {
                        MessageType = message.Content.MessageType,
                        MessageId = message.Content.MessageId
                    });

                    var messageFormat = savedMessage.Content?.MessageFormat;

                    if (messageFormat == MessageFormat.Email)
                    {
                        var emailContent = JsonConvert.DeserializeObject<EmailContent>(savedMessage.Content.Data);

                        await _emailService.SendAsync(new EmailMessage
                        {
                            MessageType = savedMessage.MessageType,
                            TemplateId = savedMessage.Content.TemplateId,
                            UserId = savedMessage.Content.UserId,
                            RecipientsAddress = emailContent.RecipientsAddress,
                            ReplyToAddress = emailContent.ReplyToAddress,
                            Data = emailContent.Data
                        });
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

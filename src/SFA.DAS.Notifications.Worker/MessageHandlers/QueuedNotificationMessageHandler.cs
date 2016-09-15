using System;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.Messaging;
using SFA.DAS.Notifications.Application.Commands;
using SFA.DAS.Notifications.Application.Commands.DispatchNotification;
using SFA.DAS.Notifications.Application.Messages;

namespace SFA.DAS.Notifications.Worker.MessageHandlers
{
    public class QueuedNotificationMessageHandler
    {
        [QueueName]
        public string send_notifications { get; set; }

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;
        private readonly IPollingMessageReceiver _pollingMessageReceiver;

        public QueuedNotificationMessageHandler(IMediator mediator, IPollingMessageReceiver pollingMessageReceiver)
        {
            _mediator = mediator;
            _pollingMessageReceiver = pollingMessageReceiver;
        }

        public async Task Handle()
        {
            var message = await _pollingMessageReceiver.ReceiveAsAsync<DispatchNotificationMessage>();

            if (message?.Content != null)
            {
                Logger.Info($"Received message {message.Content.MessageId}");

                try
                {
                    var command = new DispatchNotificationCommand
                    {
                        MessageType = message.Content.MessageType,
                        MessageId = message.Content.MessageId
                    };

                    await _mediator.SendAsync(command);


                    await message.CompleteAsync();

                    Logger.Info($"Finished processing message {message.Content.MessageId}");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Error processing message {message.Content.MessageId} - {ex.Message}");

                    await message.CompleteAsync(); //todo: abort message?
                }
            }
        }
    }
}

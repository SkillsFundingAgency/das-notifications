using System;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Application.Messages;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public class NotifyEmailService : IEmailService
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly INotifyHttpClientWrapper _clientWrapper;

        public NotifyEmailService(INotifyHttpClientWrapper clientWrapper)
        {
            if (clientWrapper == null)
                throw new ArgumentNullException(nameof(clientWrapper));
            _clientWrapper = clientWrapper;
        }

        public async Task SendAsync(EmailMessage message)
        {
            Logger.Info($"Sending {message.MessageType} to {message.RecipientsAddress}");

            var notifyMessage = new NotifyMessage
            {
                To = message.RecipientsAddress,
                Template = message.TemplateId,
                Personalisation = message.Data.ToDictionary(item => item.Key.ToLower(), item => item.Value)
            };

            await _clientWrapper.SendMessage(notifyMessage);
        }
    }
}
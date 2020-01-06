using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using Notify.Client;
using Notify.Exceptions;
using SFA.DAS.Notifications.Domain.Http;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public interface INotifyHttpClientWrapper
    {
        Task SendEmail(NotifyMessage content);
        Task SendSms(NotifyMessage content);
    }

    public class NotifyHttpClientWrapper : INotifyHttpClientWrapper
    {
        private readonly NotificationServiceConfiguration _configuration;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public NotifyHttpClientWrapper(NotificationServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmail(NotifyMessage content)
        {
            return SendMessage(content, CommunicationType.Email);
        }

        public Task SendSms(NotifyMessage content)
        {
            return SendMessage(content, CommunicationType.Sms);
        }

        private async Task SendMessage(NotifyMessage content, CommunicationType communicationType)
        {

            var notificationsClient = new NotificationClient(_configuration.NotificationServiceApiKey);

            // Needs to be a dictionary<string,dynamic> for the client.....
            var personalisationDictionary = content.Personalisation.ToDictionary(x => x.Key, x => x.Value as dynamic);
            try
            {
                Logger.Info($"Sending communication request to Gov Notify");
                if (communicationType == CommunicationType.Email)
                {
                    var response = await notificationsClient.SendEmailAsync(content.To, content.Template, personalisationDictionary, content.Reference);
                }
                else if (communicationType == CommunicationType.Sms)
                {
                    var response = await notificationsClient.SendSmsAsync(content.To, content.Template, personalisationDictionary, content.Reference);
                }
            }
            catch (NotifyClientException notifyClientException)
            {
                Logger.Error(notifyClientException, $"Error sending communication {communicationType.ToString()} to Gov Notify with Gov.Notify Client");
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Generic Error sending communication {communicationType.ToString()} to Gov Notify");
                throw;
            }
        }

        private enum CommunicationType
        {
            None,
            Email,
            Sms
        }
    }
}

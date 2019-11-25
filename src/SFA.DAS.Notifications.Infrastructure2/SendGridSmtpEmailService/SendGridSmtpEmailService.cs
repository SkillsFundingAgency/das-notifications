using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Configuration;
using SFA.DAS.Notifications.Application2.Interfaces;
using SFA.DAS.Notifications.Domain2.Entities;
using SFA.DAS.Notifications.Infrastructure2.Configuration;

namespace SFA.DAS.Notifications.Infrastructure2.SendGridSmtpEmailService
{
    public class SendGridSmtpEmailService : IEmailService
    {
        private readonly IConfigurationService _configurationService;

        public SendGridSmtpEmailService(IConfigurationService configurationService)
        {
            if (configurationService == null)
                throw new ArgumentNullException(nameof(configurationService));
            _configurationService = configurationService;
        }

        public async Task SendAsync(EmailMessage message)
        {
            var config = _configurationService.Get<NotificationServiceConfiguration>();

            using (var client = new SmtpClient())
            {
                client.Port = GetPortNumber(config.SmtpConfiguration.Port);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = config.SmtpConfiguration.ServerName;

                if (!string.IsNullOrEmpty(config.SmtpConfiguration.UserName) && !string.IsNullOrEmpty(config.SmtpConfiguration.Password))
                {
                    client.Credentials = new System.Net.NetworkCredential(config.SmtpConfiguration.UserName, config.SmtpConfiguration.Password);
                }

                var mail = new MailMessage(message.ReplyToAddress, message.RecipientsAddress)
                {
                    Subject = message.Subject,
                    Body = JsonConvert.SerializeObject(message)
                };
                await client.SendMailAsync(mail);
            }
        }

        private int GetPortNumber(string candidate)
        {
            int port;

            return int.TryParse(candidate, out port) ? port : 25;
        }
    }
}

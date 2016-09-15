using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Configuration;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.LocalEmailService
{
    public class LocalEmailService : IEmailService
    {
        private readonly IConfigurationService _configurationService;

        public LocalEmailService(IConfigurationService configurationService)
        {
            if (configurationService == null)
                throw new ArgumentNullException(nameof(configurationService));
            _configurationService = configurationService;
        }

        public async Task SendAsync(EmailMessage message)
        {
            var config = await _configurationService.GetAsync<NotificationServiceConfiguration>();

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
                    Subject = "email", //todo: email subject goes here
                    Body = JsonConvert.SerializeObject(message)
                };

                await client.SendMailAsync(mail);
            }
        }

        private int GetPortNumber(string candidate)
        {
            var port = 25;

            int.TryParse(candidate, out port);

            return port;
        }

        private string GetItemFromInput(Dictionary<string, string> items, string name)
        {
            foreach (var item in items.Where(item => string.Equals(name, item.Key, StringComparison.CurrentCultureIgnoreCase)))
            {
                return item.Value;
            }

            return string.Empty;
        }
    }
}
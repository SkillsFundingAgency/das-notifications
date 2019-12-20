using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.LocalEmailService
{
    public class LocalEmailService : IEmailService
    {
        private readonly SmtpConfiguration _configuration;

        public LocalEmailService(SmtpConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(EmailMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.Port = GetPortNumber(_configuration.Port);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = _configuration.ServerName;

                if (!string.IsNullOrEmpty(_configuration.UserName) && !string.IsNullOrEmpty(_configuration.Password))
                {
                    client.Credentials = new System.Net.NetworkCredential(_configuration.UserName, _configuration.Password);
                }

                var mail = new MailMessage(message.ReplyToAddress, message.RecipientsAddress) {
                    Subject = message.Subject,
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
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.Configuration;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.SendGridSmtpEmailService
{
    public class SendGridSmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SendGridSmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(EmailMessage message)
        {
            var config = JsonConvert.DeserializeObject<NotificationServiceConfiguration>(_configuration["SFA.DAS.Notifications_1.0"]);

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

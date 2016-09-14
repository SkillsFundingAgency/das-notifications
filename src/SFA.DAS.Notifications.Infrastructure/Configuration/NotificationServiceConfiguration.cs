using System;

namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public class NotificationServiceConfiguration
    {
        public SmtpConfiguration SmtpServer { get; set; }
        public AzureServiceBusMessageSubSystemConfiguration ServiceBusConfiguration { get; set; }
        public MessageStorageConfiguration MessageStorageConfiguration { get; set; }
        public NotifyEmailServiceConfiguration NotifyEmailServiceConfiguration { get; set; }
    }
}

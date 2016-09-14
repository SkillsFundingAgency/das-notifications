using System;

namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public class NotificationServiceConfiguration
    {
        public SmtpConfiguration SmtpConfiguration { get; set; }
        public AzureServiceBusMessageSubSystemConfiguration ServiceBusConfiguration { get; set; }
        public NotificationsStorageConfiguration NotificationsStorageConfiguration { get; set; }
        public NotifyEmailServiceConfiguration NotifyEmailServiceConfiguration { get; set; }
    }
}

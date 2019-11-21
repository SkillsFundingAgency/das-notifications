﻿namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public class NotificationServiceConfiguration
    {
        public SmtpConfiguration SmtpConfiguration { get; set; }
        public AzureServiceBusMessageServiceConfiguration AzureServiceBusMessageServiceConfiguration { get; set; }
        public NotificationsStorageConfiguration NotificationsStorageConfiguration { get; set; }
        public NotifyServiceConfiguration NotifyServiceConfiguration { get; set; }
        public NServiceBusConfiguration NServiceBusConfiguration { get; set; }
        public string EmailService { get; set; }
    }
}

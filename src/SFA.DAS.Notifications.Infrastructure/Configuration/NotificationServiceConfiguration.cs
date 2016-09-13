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

    public class SmtpConfiguration
    {
        public string ServerName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
    }

    public class AzureServiceBusMessageSubSystemConfiguration
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }

    public class MessageStorageConfiguration
    {
        public string TableName { get; set; }
    }

    public class NotifyEmailServiceConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ServiceId { get; set; }
        public string ApiKey { get; set; }
    }
}

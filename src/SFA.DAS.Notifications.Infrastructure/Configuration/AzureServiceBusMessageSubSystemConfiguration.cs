using System;

namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public class AzureServiceBusMessageSubSystemConfiguration
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}

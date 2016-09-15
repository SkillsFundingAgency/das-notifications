using System;

namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public class AzureServiceBusMessageServiceConfiguration
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}

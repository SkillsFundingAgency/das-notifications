namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public class NServiceBusConfiguration
    {
        public string ServiceBusConnectionString { get; set; }
        public string BlobStorageDataBusConnectionString { get; set; }
        public string NServiceBusLicense { get; set; }
    }
}
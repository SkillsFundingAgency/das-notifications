using System.Collections.Generic;

namespace SFA.DAS.Notifications.Infrastructure.Configuration
{
    public class NotifyServiceConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ServiceId { get; set; }
        public string ApiKey { get; set; }

        public List<NotifyServiceConsumerConfiguration> ConsumerConfiguration { get; set; }
    }
}

using SFA.DAS.Notifications.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Notifications.MessageHandlers.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            For<NServiceBusConfiguration>()
                .Use(c => c.GetInstance<NotificationServiceConfiguration>().NServiceBusConfiguration);
        }
    }
}

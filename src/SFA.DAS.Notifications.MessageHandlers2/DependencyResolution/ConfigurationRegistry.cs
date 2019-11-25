using SFA.DAS.AutoConfiguration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Notifications.MessageHandlers2.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            IncludeRegistry<AutoConfigurationRegistry>();
            For<NotificationServiceConfiguration>().Use(c => c.GetInstance<IAutoConfigurationService>().Get<NotificationServiceConfiguration>(NotificationConstants.ServiceName)).Singleton();
            For<NServiceBusConfiguration>().Use(c => c.GetInstance<NotificationServiceConfiguration>().NServiceBusConfiguration);
        }
    }
}
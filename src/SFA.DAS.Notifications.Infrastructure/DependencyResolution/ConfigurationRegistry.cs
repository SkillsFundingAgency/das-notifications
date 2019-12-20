using Microsoft.Extensions.Configuration;
using SFA.DAS.AutoConfiguration.DependencyResolution;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Notifications.Infrastructure.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        public ConfigurationRegistry()
        {
            IncludeRegistry<AutoConfigurationRegistry>();
            AddConfiguration<NotificationServiceConfiguration>(NotificationConfigurationKeys.Notifications);
            AddConfiguration<NotifyServiceConfiguration>(NotificationConfigurationKeys.NotifyServiceConfiguration);
            AddConfiguration<NServiceBusConfiguration>(NotificationConfigurationKeys.NServiceBusConfiguration);
            AddConfiguration<SmtpConfiguration>(NotificationConfigurationKeys.SmtpConfiguration);

            AddConfiguration<TemplateConfiguration>(NotificationConfigurationKeys.NotificationsTemplates);
        }

        private void AddConfiguration<T>(string key) where T : class
        {
            For<T>().Use(c => GetConfiguration<T>(c, key)).Singleton();
        }

        private T GetConfiguration<T>(IContext context, string name)
        {
            var configuration = context.GetInstance<IConfiguration>();
            var section = configuration.GetSection(name);
            var value = section.Get<T>();

            return value;
        }
    }
}

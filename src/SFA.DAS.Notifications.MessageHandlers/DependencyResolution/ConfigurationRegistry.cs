using System;
using MediatR;
using Microsoft.WindowsAzure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Notifications.MessageHandlers.DependencyResolution
{
    public class ConfigurationRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Notifications";
        private const string Version = "1.0";

        public ConfigurationRegistry()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var config = GetConfiguration(environment);


            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceName));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();

            For<NotificationServiceConfiguration>().Use(config);

            For<NServiceBusConfiguration>()
                .Use(c => c.GetInstance<NotificationServiceConfiguration>().NServiceBusConfiguration);
        }

        private NotificationServiceConfiguration GetConfiguration(string environment)
        {
            var configurationService = GetConfigurationService(environment);

            return configurationService.Get<NotificationServiceConfiguration>();
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
        }

        private static IConfigurationService GetConfigurationService(string environment)
        {
            var configurationRepository = GetConfigurationRepository();
            return new ConfigurationService(configurationRepository, new ConfigurationOptions(ServiceName, environment, Version));
        }
    }
}

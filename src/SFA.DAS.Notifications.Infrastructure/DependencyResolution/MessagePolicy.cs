﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Notifications.Application.Commands;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.Notifications.Infrastructure.DependencyResolution
{
    public class MessagePolicy : ConfiguredInstancePolicy
    {
        private readonly string _serviceName;

        public MessagePolicy(string serviceName)
        {
            _serviceName = serviceName;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var messagePublisher = instance?.Constructor?
                .GetParameters().FirstOrDefault(x => x.ParameterType == typeof(IMessagePublisher) || x.ParameterType == typeof(IPollingMessageReceiver));

            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            if (messagePublisher != null)
            {
                var queueName = instance
                    .SettableProperties()
                    .FirstOrDefault(
                        c => c.CustomAttributes
                            .FirstOrDefault(x => x.AttributeType.Name == nameof(QueueNameAttribute)) != null);

                if (queueName != null)
                {
                    var configurationService = new ConfigurationService(GetConfigurationRepository(), new ConfigurationOptions(_serviceName, environment, "1.0"));

                    var config = configurationService.Get<NotificationServiceConfiguration>().AzureServiceBusMessageServiceConfiguration;
                    if (string.IsNullOrEmpty(config.ConnectionString))
                    {
                        var queueFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        instance.Dependencies.AddForConstructorParameter(messagePublisher, new FileSystemMessageService(Path.Combine(queueFolder, queueName.Name)));
                    }
                    else
                    {
                        instance.Dependencies.AddForConstructorParameter(messagePublisher, new AzureServiceBusMessageService(config.ConnectionString, queueName.Name));
                    }
                }
            }
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            IConfigurationRepository configurationRepository;
            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }
            return configurationRepository;
        }
    }
}

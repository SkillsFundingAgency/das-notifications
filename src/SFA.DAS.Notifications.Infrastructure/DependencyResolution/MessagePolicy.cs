using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.FileSystem;
using SFA.DAS.Notifications.Application.Commands;
using StructureMap;
using StructureMap.Pipeline;

namespace SFA.DAS.Notifications.Infrastructure.DependencyResolution
{
    public class MessagePolicy : ConfiguredInstancePolicy
    {
        private readonly string _serviceName;
        private readonly IConfiguration _configuration;

        public MessagePolicy(string serviceName, IConfiguration configuration)
        {
            _serviceName = serviceName;
            _configuration = configuration;
        }

        protected override void apply(Type pluginType, IConfiguredInstance instance)
        {
            var messagePublisher = instance?.Constructor?
                .GetParameters().FirstOrDefault(x => x.ParameterType == typeof (IMessagePublisher) || x.ParameterType == typeof (IPollingMessageReceiver));

            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = _configuration["EnvironmentName"];
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
                    //var configurationService = new ConfigurationService(GetConfigurationRepository(), new ConfigurationOptions(_serviceName, environment, "1.0"));

                    //var config = configurationService.Get<NotificationServiceConfiguration>().AzureServiceBusMessageServiceConfiguration;
                    if (string.IsNullOrEmpty(_configuration.GetSection("NServiceBusConfiguration")["ServiceBusConnectionString"])) //todo lose these magic strings
                    {
                        var queueFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        instance.Dependencies.AddForConstructorParameter(messagePublisher, new FileSystemMessageService(Path.Combine(queueFolder, queueName.Name)));
                    }
                    else
                    {
                        instance.Dependencies.AddForConstructorParameter(messagePublisher, new AzureServiceBusMessageService(_configuration.GetSection("NServiceBusConfiguration")["ServiceBusConnectionString"], queueName.Name));
                    }
                }
            }
        }
    }
}

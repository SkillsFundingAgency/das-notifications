﻿using System;
using System.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using SFA.DAS.Notifications.Infrastructure.LocalEmailService;
using SFA.DAS.Notifications.Infrastructure.NotifyEmailService;
using SFA.DAS.Notifications.Infrastructure.SendGridSmtpEmailService;
using SFA.DAS.Notifications.MessageHandlers.Startup;
using StructureMap;

namespace SFA.DAS.Notifications.MessageHandlers.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = ConfigurationManager.AppSettings["EnvironmentName"];
            }

            For<IStartup>().Use<NServiceBusStartup>().Singleton();
            For<ILoggerFactory>().Use(() => new LoggerFactory().AddApplicationInsights(ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"], null).AddNLog()).Singleton();
            For<ILogger>().Use(c => c.GetInstance<ILoggerFactory>().CreateLogger(c.ParentType.ToString()));

            // Legacy Logger required in application layers
            For<ILog>().Use(x => new NLogLogger(x.ParentType, null, null)).AlwaysUnique();

            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(NotificationConstants.ServiceName));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();

            For<IConfigurationService>().Use(GetConfigurationService(environment));
            For<IConfigurationRepository>().Use(GetConfigurationRepository());
            For<ITemplateConfigurationService>().Use<TemplateConfigurationService>().Ctor<string>().Is(environment);
            ConfigureEmailService();
            For<ISmsService>().Use<NotifySmsService>();
        }

        private NotificationServiceConfiguration GetConfiguration(string environment)
        {
            var configurationService = GetConfigurationService(environment);
            return configurationService.Get<NotificationServiceConfiguration>();
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"]);
        }

        private static IConfigurationService GetConfigurationService(string environment)
        {
            var configurationRepository = GetConfigurationRepository();
            return new ConfigurationService(configurationRepository, new ConfigurationOptions(NotificationConstants.ServiceName, environment, NotificationConstants.Version));
        }

        private void ConfigureEmailService()
        {
            For<IEmailService>().AddInstances(x =>
            {
                x.Type<LocalEmailService>().Named("Local");
                x.Type<SendGridSmtpEmailService>().Named("SendGridSmtp");
                x.Type<NotifyEmailService>().Named("Notify");
            });

            For<IEmailService>().Use(x => x.GetInstance<IEmailService>(x.GetInstance<IConfigurationService>().Get<NotificationServiceConfiguration>().EmailService));
        }
    }
}
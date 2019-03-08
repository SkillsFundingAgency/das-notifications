using System;
using FluentValidation;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Repositories;
using SFA.DAS.Notifications.Infrastructure.AzureMessageNotificationRepository;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using SFA.DAS.Notifications.Infrastructure.LocalEmailService;
using SFA.DAS.Notifications.Infrastructure.GovNotifyService;
using SFA.DAS.Notifications.Infrastructure.SendGridSmtpEmailService;
using SFA.DAS.Notifications.Worker.MessageHandlers;
using StructureMap;

namespace SFA.DAS.Notifications.Worker.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Notifications";
        private const string Version = "1.0";
        private const string DevEnv = "LOCAL";

        public DefaultRegistry()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceName));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                    scan.ConnectImplementationsToTypesClosing(typeof (AbstractValidator<>));
                });

            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            var config = GetConfiguration(environment);

            For<INotificationsRepository>().Use<AzureNotificationRepository>().Ctor<NotificationServiceConfiguration>().Is(config);

            For<IConfigurationService>().Use(GetConfigurationService(environment));

            ConfigureEmailService();

            ConfigureSmsService();

            For<QueuedNotificationMessageHandler>().Use<QueuedNotificationMessageHandler>();
            For<IMediator>().Use<Mediator>();

            RegisterExecutionPolicies();
        }

        private void ConfigureEmailService()
        {
            For<IEmailService>().AddInstances(x =>
            {
                x.Type<LocalEmailService>().Named("Local");
                x.Type<SendGridSmtpEmailService>().Named("SendGridSmtp");
                x.Type<NotifyEmailService>().Named("Notify");
            });

            var emailServiceName = CloudConfigurationManager.GetSetting("EmailServiceName");

            For<IEmailService>().Use(emailServiceName);
        }

        private void ConfigureSmsService()
        {
            //todo: SMS configuration
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

        private void RegisterExecutionPolicies()
        {
            For<Infrastructure.ExecutionPolicies.ExecutionPolicy>()
                .Use<Infrastructure.ExecutionPolicies.SendEmailExecutionPolicy>()
                .Named(Infrastructure.ExecutionPolicies.SendEmailExecutionPolicy.Name);
        }
    }
}

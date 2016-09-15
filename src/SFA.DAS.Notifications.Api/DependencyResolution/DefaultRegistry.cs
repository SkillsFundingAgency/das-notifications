// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using FluentValidation;
using MediatR;
using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Repositories;
using SFA.DAS.Notifications.Infrastructure;
using SFA.DAS.Notifications.Infrastructure.AzureMessageNotificationRepository;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.Notifications.Api.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Notifications";
        private const string Version = "1.0";

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
            For<IMediator>().Use<Mediator>();

            var config = GetConfiguration(environment);

            For<INotificationsRepository>().Use<AzureNotificationRepository>().Ctor<NotificationServiceConfiguration>().Is(config);

            For<IConfigurationService>().Use(GetConfigurationService(environment));
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

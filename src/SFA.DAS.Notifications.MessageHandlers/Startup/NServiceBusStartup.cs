﻿using System.Data.Common;
using Azure.Storage.Blobs;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;
using StructureMap;

namespace SFA.DAS.Notifications.MessageHandlers.Startup
{
    public static class NServiceBusStartup
    {
        public static IServiceCollection AddNServiceBus(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            return services
                .AddSingleton(p =>
                {
                    var nservicebusConfiguration = configuration.GetSection(NotificationConfigurationKeys.NServiceBusConfiguration).Get<NServiceBusConfiguration>();
                    var container = p.GetService<IContainer>();

                    var endpointName = "SFA.DAS.Notifications.MessageHandlers";
                    var endpointConfiguration = new EndpointConfiguration(endpointName)
                        .UseErrorQueue($"{endpointName}-errors")
                        .UseInstallers()
                        .UseLicense(nservicebusConfiguration.NServiceBusLicense)
                        .UseMessageConventions()
                        .UseNewtonsoftJsonSerializer()
                        .UseNLogFactory()
                        .UseOutbox(true)
                        .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
                        .UseStructureMapBuilder(container)
                        .UseUnitOfWork();

                    var containerName = "databus";
                    var blobServiceClient = new BlobServiceClient(nservicebusConfiguration.BlobStorageDataBusConnectionString);
                    var dataBus = endpointConfiguration.UseDataBus<AzureDataBus>()
                                .Container(containerName)
                                .UseBlobServiceClient(blobServiceClient);

                    var conventions = endpointConfiguration.Conventions();
                    conventions.DefiningDataBusPropertiesAs(property =>
                    {
                        return property.Name.EndsWith("DataBus");
                    });

                    if (isDevelopment)
                    {
                        endpointConfiguration.UseLearningTransport();
                    }
                    else
                    {
                        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                        var ruleNameShortener = new RuleNameShortener();

                        var tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
                        transport.CustomTokenProvider(tokenProvider);
                        transport.ConnectionString(nservicebusConfiguration.ServiceBusConnectionString);
                        transport.RuleNameShortener(ruleNameShortener.Shorten);
                    }

                    var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

                    return endpoint;
                })
                .AddHostedService<NServiceBusHostedService>();
        }
    }
}
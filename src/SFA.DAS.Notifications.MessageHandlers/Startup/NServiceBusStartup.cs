﻿using Microsoft.Azure.ServiceBus.Primitives;
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
using System.Data.Common;

namespace SFA.DAS.Notifications.MessageHandlers.Startup
{
    public static class NServiceBusStartup
    {
        public static IServiceCollection AddNServiceBus(this IServiceCollection services, IConfiguration config, bool isDevelopment)
        {
            return services
                .AddSingleton(p =>
                {
                    var configuration = p.GetService<IConfiguration>();
                    var serviceBusConfiguration = configuration.GetNotificationSection<NServiceBusConfiguration>("NServiceBusConfiguration");
                    var container = p.GetService<IContainer>();
                    
                    var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Notifications.MessageHandlers")
                        .UseInstallers()
                        .UseLicense(serviceBusConfiguration.NServiceBusLicense)
                        .UseErrorQueue("errors")
                        .UseMessageConventions()
                        .UseNewtonsoftJsonSerializer()
                        .UseNLogFactory()
                        .UseOutbox()
                        .UseSqlServerPersistence(() => container.GetInstance<DbConnection>())
                        .UseStructureMapBuilder(container)
                        .UseUnitOfWork();

                    if (isDevelopment)
                    {
                        endpointConfiguration.UseLearningTransport();
                    }
                    else
                    {
                        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                        var ruleNameShortener = new RuleNameShortener();

                        var tokenProvider = TokenProvider.CreateManagedServiceIdentityTokenProvider();
                        transport.CustomTokenProvider(tokenProvider);
                        transport.ConnectionString(serviceBusConfiguration.ServiceBusConnectionString);
                        transport.RuleNameShortener(ruleNameShortener.Shorten);
                    }

                    var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

                    return endpoint;
                })
                .AddHostedService<NServiceBusHostedService>();
        }        
    }
}
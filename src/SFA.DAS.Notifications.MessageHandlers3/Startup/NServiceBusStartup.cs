using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using SFA.DAS.Notifications.MessageHandlers3.NServiceBus;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Hosting;

namespace SFA.DAS.Notifications.MessageHandlers3.Startup
{
    public static class NServiceBusStartup
    {
        public static IServiceCollection AddDasNServiceBus(this IServiceCollection services)
        {
            return services
                .AddSingleton(s =>
                {
                    var configuration = s.GetService<IConfiguration>();
                    var hostingEnvironment = s.GetService<IHostingEnvironment>();
                    var serviceBusConfiguration = configuration.GetNotificationSection<NServiceBusConfiguration>("NServiceBusConfiguration");
                    var isDevelopment = true;// hostingEnvironment.IsDevelopment(); todo fix development env variable / app setting
                    var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Notifications.MessageHandlers")
                        .UseAzureServiceBusTransport(isDevelopment, serviceBusConfiguration.ServiceBusConnectionString)
                        .UseErrorQueue("errors")
                        .UseInstallers()
                        .UseLicense(serviceBusConfiguration.NServiceBusLicense)
                        .UseNotificationsMessageConventions()
                        .UseNewtonsoftJsonSerializer()
                        .UseNLogFactory()
                        .UseServiceCollection(services)
                        ;

                    return Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
                })
                .AddHostedService<NServiceBusHostedService>();
        }

        public static EndpointConfiguration UseAzureServiceBusTransport(
            this EndpointConfiguration endpointConfiguration,
            bool isDevelopment,
            string connectionString
        )
        {
            if (isDevelopment)
            {
                var transport = endpointConfiguration.UseTransport<LearningTransport>();
                transport.Transactions(TransportTransactionMode.ReceiveOnly);
            }

            else
            {
                endpointConfiguration
                    .UseAzureServiceBusTransport(connectionString, r => { })
                ;
            }

            return endpointConfiguration;
        }
    }
}
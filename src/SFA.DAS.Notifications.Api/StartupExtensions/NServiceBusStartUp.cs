using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.Notifications.Api.StartupExtensions
{
    public static class NServiceBusStartUp
    {
        public static void StartNServiceBus(this UpdateableServiceProvider serviceProvider,
            IConfiguration configuration, bool configurationIsLocalOrDev)
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Notifications.Api")
                .UseErrorQueue()
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseOutbox(true)
                .UseServicesBuilder(serviceProvider)
                .UseSqlServerPersistence(() => new SqlConnection(configuration["DatabaseConnectionString"]))
                .UseUnitOfWork();
            
            if (configurationIsLocalOrDev)
            {
                endpointConfiguration.UseLearningTransport();
            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(configuration["NServiceBusConfiguration:ServiceBusConnectionString"], r => r.AddRouting());
            }

            var license = configuration["NServiceBusConfiguration:NServiceBusLicense"];
            if (!string.IsNullOrEmpty(license))
            {
                endpointConfiguration.License(license);
            }

            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

            serviceProvider.AddSingleton(p => endpoint)
                .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();
        }

        private const string NotificationsMessageHandler = "SFA.DAS.Notifications.MessageHandlers";
        private static void AddRouting(this RoutingSettings routingSettings)
        {
            routingSettings.RouteToEndpoint(typeof(SendEmailCommand), NotificationsMessageHandler);
            routingSettings.RouteToEndpoint(typeof(SendSmsCommand), NotificationsMessageHandler);
        }
    }
}
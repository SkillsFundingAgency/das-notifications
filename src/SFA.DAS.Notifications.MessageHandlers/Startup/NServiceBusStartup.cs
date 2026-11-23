using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using SFA.DAS.Notifications.Infrastructure.DependencyResolution;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;

namespace SFA.DAS.Notifications.MessageHandlers.Startup
{
    public static class NServiceBusStartup
    {
        public static IServiceCollection AddNServiceBus(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            var nservicebusConfiguration = configuration.GetSection(NotificationConfigurationKeys.NServiceBusConfiguration).Get<NServiceBusConfiguration>();
            var notificationConfiguration = configuration.GetSection(NotificationConfigurationKeys.Notifications).Get<NotificationServiceConfiguration>();

            var endpointName = "SFA.DAS.Notifications.MessageHandlers";
            var endpointConfiguration = new EndpointConfiguration(endpointName)
                .UseErrorQueue($"{endpointName}-errors")
                .UseInstallers()
                .UseLicense(nservicebusConfiguration.NServiceBusLicense)
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseOutbox(true)
                .UseSqlServerPersistence(() => DatabaseConnectionFactory.Create(notificationConfiguration.DatabaseConnectionString));

            if (isDevelopment)
            {
                endpointConfiguration.UseLearningTransport();
            }
            else
            {
                endpointConfiguration.UseAzureServiceBusTransport(nservicebusConfiguration.ServiceBusConnectionString);
            }

            var startableEndpoint = EndpointWithExternallyManagedServiceProvider.Create(endpointConfiguration, services);

            return services
                .AddSingleton(p => startableEndpoint.Start(p).GetAwaiter().GetResult())
                .AddHostedService<NServiceBusHostedService>();
        }
    }
}
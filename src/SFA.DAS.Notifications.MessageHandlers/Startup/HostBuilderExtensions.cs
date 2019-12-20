using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using StructureMap;
using System;

namespace SFA.DAS.Notifications.MessageHandlers.Startup
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigureDasAppConfiguration(this IHostBuilder hostBuilder, string[] args)
        {
            return hostBuilder.ConfigureAppConfiguration((context, builder) =>
            {
                builder.AddAzureTableStorage(NotificationConfigurationKeys.Notifications, NotificationConfigurationKeys.NotificationsTemplates)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args);
            });
        }

        public static IHostBuilder UseDasEnvironment(this IHostBuilder hostBuilder)
        {
            var environmentName = Environment.GetEnvironmentVariable(EnvironmentVariableNames.EnvironmentName);
            var mappedEnvironmentName = DasEnvironmentName.Map[environmentName];

            return hostBuilder.UseEnvironment(mappedEnvironmentName);
        }

        public static IHostBuilder UseStructureMap(this IHostBuilder builder)
        {
            return builder.UseServiceProviderFactory(new StructureMapServiceProviderFactory(null));
        }

        public static IHostBuilder UseApplicationInsights(this IHostBuilder builder)
        {
            builder.ConfigureLogging((c, b) => b.AddApplicationInsights(o => o.InstrumentationKey = c.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]));

            return builder;
        }
    }
}

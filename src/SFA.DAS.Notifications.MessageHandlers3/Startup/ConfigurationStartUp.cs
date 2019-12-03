using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Notifications.MessageHandlers3.Startup
{
    public static class ConfigurationKeys
    {
        public const string Notifications = "SFA.DAS.Notifications";
    }

    public static class ConfigurationStartup
    {
        public static IHostBuilder ConfigureDasAppConfiguration(this IHostBuilder builder, string[] args)
        {
            return builder.ConfigureAppConfiguration((c, b) =>
            {
                var buildProgress = b;
                buildProgress = buildProgress.AddJsonFile("appsettings.json", true, true);
                buildProgress = buildProgress.AddJsonFile($"appsettings.{c.HostingEnvironment.EnvironmentName}.json", true, true);
                buildProgress = buildProgress.AddEnvironmentVariables();
                buildProgress = buildProgress.AddAzureTableStorage(o =>
                {
                    o.EnvironmentNameEnvironmentVariableName = "LOCAL";//"ASPNETCORE_ENVIRONMENT";
                    o.ConfigurationKeys = new[] {
                        ConfigurationKeys.Notifications
                    };
                });
                buildProgress = buildProgress.AddCommandLine(args);
            });
        }
    }
}

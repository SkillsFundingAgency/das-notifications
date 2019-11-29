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
            return builder.ConfigureAppConfiguration((c, b) => b
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{c.HostingEnvironment.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables()
                .AddAzureTableStorage(o =>
                {
                    o.EnvironmentNameEnvironmentVariableName = "ASPNETCORE_ENVIRONMENT";
                    o.ConfigurationKeys = new[]
                    {
                        ConfigurationKeys.Notifications
                    };
                })
                .AddCommandLine(args));
        }
    }
}

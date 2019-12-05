using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Hosting;

namespace SFA.DAS.Notifications.MessageHandlers.Startup
{
    public static class WebJobStartup
    {
        public static IHostBuilder ConfigureDasWebJobs(this IHostBuilder builder)
        {
            builder.ConfigureWebJobs(b => b.AddAzureStorageCoreServices()
                .AddExecutionContextBinding()
                .AddTimers());

            return builder;
        }
    }
}
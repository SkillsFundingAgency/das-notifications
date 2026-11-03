using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.Notifications.MessageHandlers.DependencyResolution;
using SFA.DAS.Notifications.MessageHandlers.Startup;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Notifications.MessageHandlers
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder();

            try
            {
                hostBuilder
                    .UseDasEnvironment()
                    .ConfigureDasAppConfiguration(args)
                    .ConfigureLogging((context, b) =>
                    {
                        b.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
                    })
                    .UseConsoleLifetime()
                    .UseDefaultServiceProvider(o =>
                    {
                        o.ValidateOnBuild = true;
                        o.ValidateScopes = true;
                    })
                    .ConfigureServices((c, s) => s
                        .AddApplicationServices(c.Configuration)
                        .AddNServiceBus(c.Configuration, c.HostingEnvironment.IsDevelopment()));

                using (var host = hostBuilder.Build())
                {
                    await host.RunAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
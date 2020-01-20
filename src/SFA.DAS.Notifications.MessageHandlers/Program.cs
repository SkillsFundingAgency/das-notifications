using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using SFA.DAS.Notifications.MessageHandlers.DependencyResolution;
using SFA.DAS.Notifications.MessageHandlers.Startup;
using StructureMap;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Notifications.MessageHandlers
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder();

            try
            {
                hostBuilder
                    .UseDasEnvironment()
                    .UseApplicationInsights()
                    .ConfigureDasAppConfiguration(args)
                    .ConfigureLogging((c, b) =>
                    {
                        b.AddNLog();
                        if (!string.IsNullOrWhiteSpace(c.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]))
                        {
                            b.Services.AddApplicationInsightsTelemetry(c.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
                        }
                    })
                    .UseConsoleLifetime()
                    .UseStructureMap()
                    .ConfigureServices((c, s) => s
                        .AddMemoryCache()
                        .AddNServiceBus(c.Configuration, c.HostingEnvironment.IsDevelopment()))
                    .ConfigureContainer<Registry>(IoC.Initialize);

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
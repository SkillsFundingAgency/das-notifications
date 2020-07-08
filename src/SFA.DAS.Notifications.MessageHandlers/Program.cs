using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                    .ConfigureDasAppConfiguration(args)
                    .ConfigureLogging((context, b) =>
                    {
                        b.AddNLog(context.HostingEnvironment.IsDevelopment() ? "nlog.development.config" : "nlog.config");
                        b.AddApplicationInsightsWebJobs(o => o.InstrumentationKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);
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
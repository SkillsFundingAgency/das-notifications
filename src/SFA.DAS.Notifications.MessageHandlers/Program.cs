using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using SFA.DAS.Notifications.MessageHandlers.DependencyResolution;
using SFA.DAS.Notifications.MessageHandlers.Startup;

//using SFA.DAS.Notifications.MessageHandlers3.StartupJobs;

namespace SFA.DAS.Notifications.MessageHandlers
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var host = CreateHostBuilder(args).Build())
            {
                await host.StartAsync();
                await host.WaitForShutdownAsync();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            new HostBuilder()
                .ConfigureDasWebJobs()
                .ConfigureDasAppConfiguration(args)
                .ConfigureLogging(b => b.AddNLog())
                //.UseApplicationInsights()
                //.UseDasEnvironment()
                .UseConsoleLifetime()
                //.ConfigureServices((c, s) => s.AddApplicationServices(c))
                //.ConfigureServices((c, s) => s.AddHashingServices(c))
                //.ConfigureServices((c, s) => s.AddCommitmentsApi(c))
                //.ConfigureServices((c, s) => s.AddProviderServices(c))
                .ConfigureServices((c, s) => s.AddDefaultServices(c))
                .ConfigureServices((c, s) => s.AddDasNServiceBus());
    }
}
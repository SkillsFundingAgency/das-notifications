using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Notifications.MessageHandlers.DependencyResolution;
using IStartup = SFA.DAS.Notifications.MessageHandlers.Startup.IStartup;

namespace SFA.DAS.Notifications.MessageHandlers
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using (var container = IoC.Initialize())
            {
                try
                {
                    ServicePointManager.DefaultConnectionLimit = 50;

                    var config = new JobHostConfiguration {JobActivator = new StructureMapJobActivator(container)};

                    var environmentService = container.GetInstance<IEnvironmentService>();
                    var loggerFactory = container.GetInstance<ILoggerFactory>();
                    var startup = container.GetInstance<IStartup>();

                    if (environmentService.IsCurrent(DasEnv.LOCAL))
                    {
                        config.UseDevelopmentSettings();
                    }

                    config.LoggerFactory = loggerFactory;

                    var jobHost = new JobHost(config);

                    await startup.StartAsync();
                    await jobHost.CallAsync(typeof(Program).GetMethod(nameof(Block)));

                    jobHost.RunAndBlock();

                    await startup.StopAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        [NoAutomaticTrigger]
        public static async Task Block(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(3000, cancellationToken);
            }
        }
    }
}

using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using NLog.Web;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Notifications.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Info("Starting up host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                global::NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseNServiceBusContainer()
                .UseStartup<Startup>()
                .UseNLog();
    }
}

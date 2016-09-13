using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using NLog;
using SFA.DAS.Notifications.Infrastructure.DependencyResolution;
using StructureMap;

namespace SFA.DAS.Notifications.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private Container _container;

        public override void Run()
        {
            Logger.Info("Running");
            Trace.TraceInformation("SFA.DAS.Notifications.Worker is running");

            _container = new Container(c =>
            {
                c.Policies.Add(new MessagePolicy("SFA.DAS.Notifications"));
                c.AddRegistry<DefaultRegistry>();
            });

            try
            {
                RunAsync(_cancellationTokenSource.Token).Wait();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            Logger.Info("Starting");

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.NotificationService.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Logger.Info("Stopping");

            Trace.TraceInformation("SFA.DAS.NotificationService.Worker is stopping");

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SFA.DAS.NotificationService.Worker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var handler = _container.GetInstance<QueuedMessageHandler>();

            while (!cancellationToken.IsCancellationRequested)
            {
                Logger.Debug("Polling");
                try
                {
                    await handler.Handle();

                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, ex.Message);
                }
            }
        }
    }
}

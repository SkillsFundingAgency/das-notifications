using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using NLog;
using NLog.Targets;
using SFA.DAS.Notifications.Infrastructure.DependencyResolution;
using SFA.DAS.Notifications.Worker.DependencyResolution;
using SFA.DAS.Notifications.Worker.MessageHandlers;
using StructureMap;

namespace SFA.DAS.Notifications.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private const string ServiceName = "SFA.DAS.Notifications";
        private static RedisTarget _redisTarget; // Required to ensure assembly is copied to output.

        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private Container _container;

        public override void Run()
        {
            Logger.Info("Running");
            Trace.TraceInformation(ServiceName + " is running");

            _container = new Container(c =>
            {
                c.Policies.Add(new MessagePolicy(ServiceName));
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

            var result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.Notifications.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Logger.Info("Stopping");

            Trace.TraceInformation("SFA.DAS.Notifications.Worker is stopping");

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation(ServiceName + " has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var handler = _container.GetInstance<QueuedNotificationMessageHandler>();

            Logger.Debug("Started polling");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await handler.Handle();

                    await Task.Delay(1000, cancellationToken);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, ex.Message);
                }
            }
        }
    }
}

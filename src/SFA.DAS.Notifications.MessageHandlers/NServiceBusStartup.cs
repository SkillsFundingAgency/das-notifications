using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Configuration.StructureMap;
using StructureMap;
using IStartup = SFA.DAS.Notifications.MessageHandlers.Startup.IStartup;

namespace SFA.DAS.Notifications.MessageHandlers
{
    public class NServiceBusStartup : IStartup
    {
        private readonly IContainer _container;
        private readonly IEnvironmentService _environmentService;
        private readonly NServiceBusConfiguration _config;
        private IEndpointInstance _endpoint;

        public NServiceBusStartup(IContainer container, IEnvironmentService environmentService, NServiceBusConfiguration config)
        {
            _container = container;
            _environmentService = environmentService;
            _config = config;
        }

        public async Task StartAsync()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Notifications.MessageHandlers")
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(_config.NServiceBusLicense)
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseStructureMapBuilder(_container);

                if (_environmentService.IsCurrent(DasEnv.LOCAL))
                {
                    endpointConfiguration.UseLearningTransport();
                }
                else
                {
                    endpointConfiguration.UseAzureServiceBusTransport(_config.ServiceBusConnectionString);
                }

            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }

        public Task StopAsync()
        {
            return _endpoint.Stop();
        }
    }
}

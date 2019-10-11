using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using NServiceBus;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using SFA.DAS.Notifications.MessageHandlers.Extensions;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NLog;
using SFA.DAS.NServiceBus.StructureMap;
using StructureMap;
using IStartup = SFA.DAS.Notifications.MessageHandlers.Startup.IStartup;

namespace SFA.DAS.Notifications.MessageHandlers
{
    public class NServiceBusStartup : IStartup
    {
        private readonly IContainer _container;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly NServiceBusConfiguration _config;
        private IEndpointInstance _endpoint;

        public NServiceBusStartup(IContainer container, IHostingEnvironment hostingEnvironment, NServiceBusConfiguration config)
        {
            _container = container;
            _hostingEnvironment = hostingEnvironment;
            _config = config;
        }

        public async Task StartAsync()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.Notifications.MessageHandlers")
                .UseAzureServiceBusTransport(() => _config.ServiceBusConnectionString,
                    _hostingEnvironment.IsDevelopment())
                .UseErrorQueue()
                .UseInstallers()
                .UseLicense(_config.NServiceBusLicense)
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseNLogFactory()
                .UseStructureMapBuilder(_container);
                
            _endpoint = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }

        public Task StopAsync()
        {
            return _endpoint.Stop();
        }
    }
}

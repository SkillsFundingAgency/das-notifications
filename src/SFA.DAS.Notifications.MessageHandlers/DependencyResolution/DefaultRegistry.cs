using System.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using SFA.DAS.Notifications.MessageHandlers.Startup;
using StructureMap;

namespace SFA.DAS.Notifications.MessageHandlers.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            For<IStartup>().Use<NServiceBusStartup>().Singleton();
            For<ILoggerFactory>().Use(() => new LoggerFactory().AddApplicationInsights(ConfigurationManager.AppSettings["APPINSIGHTS_INSTRUMENTATIONKEY"], null).AddNLog()).Singleton();
            For<ILogger>().Use(c => c.GetInstance<ILoggerFactory>().CreateLogger(c.ParentType));

            Scan(
                scan =>
                {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(NotificationConstants.ServiceName));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }
    }
}

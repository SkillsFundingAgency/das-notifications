using MediatR;
using StructureMap;
using System;

namespace SFA.DAS.Notifications.Infrastructure.DependencyResolution
{
    public class MediatorRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Notifications";

        public MediatorRegistry()
        {
            For<IMediator>().Use<Mediator>();
            For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);

            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceName));
                scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>));
                scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
            });
        }
    }
}

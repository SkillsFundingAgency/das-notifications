using SFA.DAS.Notifications.Infrastructure.DependencyResolution;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.StructureMap;
using StructureMap;

namespace SFA.DAS.Notifications.MessageHandlers.DependencyResolution
{
    public static class IoC
    {
        public static void Initialize(Registry registry)
        {
            registry.IncludeRegistry<MediatorRegistry>();
            registry.IncludeRegistry<NServiceBusUnitOfWorkRegistry>();
            registry.IncludeRegistry<DefaultRegistry>();
            registry.IncludeRegistry<DataRegistry>();
            registry.IncludeRegistry<ConfigurationRegistry>();
        }
    }
}

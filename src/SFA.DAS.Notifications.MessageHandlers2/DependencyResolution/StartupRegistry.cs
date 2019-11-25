using SFA.DAS.Notifications.MessageHandlers2.Startup;
using StructureMap;

namespace SFA.DAS.Notifications.MessageHandlers2.DependencyResolution
{
    public class StartupRegistry : Registry
    {
        public StartupRegistry()
        {
            Scan(s =>
            {
                s.AssembliesAndExecutablesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.Convention<CompositeDecorator<DefaultStartup, IStartup>>();
            });
        }
    }
}

﻿using StructureMap;

namespace SFA.DAS.Notifications.MessageHandlers2.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.Policies.Add(new MessagePolicy(NotificationConstants.ServiceName));
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<StartupRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}

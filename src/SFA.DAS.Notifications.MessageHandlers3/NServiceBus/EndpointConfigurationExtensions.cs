﻿using Microsoft.Extensions.DependencyInjection;
using NServiceBus;

namespace SFA.DAS.Notifications.MessageHandlers3.NServiceBus
{
    public static class EndpointConfigurationExtensions
    {
        //todo: move into sfa.das.nservicebus (with cleanupjob registry)
        //todo: name? UseCoreDependencyInjection?
        public static EndpointConfiguration UseServiceCollection(this EndpointConfiguration config, IServiceCollection services)
        {
            config.UseContainer<ServicesBuilder>(c => c.ExistingServices(services));
            return config;
        }

        public static EndpointConfiguration UsePortalMessageConventions(this EndpointConfiguration config)
        {
            var conventions = config.Conventions();
            conventions.DefiningEventsAs(t => t.Namespace != null &&
            (t.Namespace.StartsWith("SFA.DAS.CommitmentsV2.Messages.Events")
            || t.Namespace.StartsWith("SFA.DAS.Commitments.Events")
            || t.Namespace.StartsWith("SFA.DAS.Reservations.Messages")
            || t.Namespace.StartsWith("SFA.DAS.ProviderRelationships.Messages.Events")));
            return config;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.Notifications.Api2.Orchestrators;

namespace SFA.DAS.Notifications.Api2.DependencyResolution
{
    public static class DefaultServices
    {
        public static IServiceCollection AddDefaultServices(this IServiceCollection services)
        {
            services.AddTransient<INotificationOrchestrator, NotificationOrchestrator>();
            //services.AddTransient<IMessageSession>() todo

            return services;
        }
    }
}

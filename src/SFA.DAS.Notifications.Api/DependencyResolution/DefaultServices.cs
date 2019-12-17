using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Notifications.Api.Orchestrators;

namespace SFA.DAS.Notifications.Api.DependencyResolution
{
    public static class DefaultServices
    {
        public static IServiceCollection AddDefaultServices(this IServiceCollection services)
        {
            services.AddTransient<INotificationOrchestrator, NotificationOrchestrator>();

            return services;
        }
    }
}

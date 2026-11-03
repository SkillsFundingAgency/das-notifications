using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using SFA.DAS.Notifications.Infrastructure.DependencyResolution;
using SFA.DAS.Notifications.Infrastructure.ExecutionPolicies;
using SFA.DAS.Notifications.Infrastructure.NotifyEmailService;

namespace SFA.DAS.Notifications.MessageHandlers.DependencyResolution
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailService, NotifyEmailService>();
            services.AddTransient<ISmsService, NotifySmsService>();
            services.AddTransient<INotifyClientWrapper, NotifyClientWrapper>();
            services.AddTransient<ExecutionPolicy, SendMessageExecutionPolicy>();

            services.AddConfigurationServices(configuration);
            services.AddDataServices();
            services.AddMediatorServices();

            return services;
        }
    }
}
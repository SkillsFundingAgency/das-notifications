using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.DependencyResolution
{
    public static class InfrastructureServiceCollectionExtensions
    {
        private const string ServiceName = "SFA.DAS.Notifications";

        public static IServiceCollection AddConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfiguration<NotificationServiceConfiguration>(configuration, NotificationConfigurationKeys.Notifications);
            services.AddConfiguration<NServiceBusConfiguration>(configuration, NotificationConfigurationKeys.NServiceBusConfiguration);
            services.AddConfiguration<SmtpConfiguration>(configuration, NotificationConfigurationKeys.SmtpConfiguration);
            services.AddConfiguration<TemplateConfiguration>(configuration, NotificationConfigurationKeys.NotificationsTemplates);

            return services;
        }

        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            services.AddTransient<DbConnection>(sp =>
                DatabaseConnectionFactory.Create(sp.GetRequiredService<NotificationServiceConfiguration>().DatabaseConnectionString));

            return services;
        }

        public static IServiceCollection AddMediatorServices(this IServiceCollection services)
        {
            var assemblies = Directory.GetFiles(AppContext.BaseDirectory, $"{ServiceName}*.dll")
                .Select(file => Assembly.Load(AssemblyName.GetAssemblyName(file)))
                .ToArray();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

            return services;
        }

        private static void AddConfiguration<T>(this IServiceCollection services, IConfiguration configuration, string key) where T : class
        {
            services.AddSingleton<T>(_ => configuration.GetSection(key).Get<T>());
        }
    }
}
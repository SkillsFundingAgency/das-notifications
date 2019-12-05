using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Notifications.Application.Commands.SendEmail;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using SFA.DAS.Notifications.Infrastructure.ExecutionPolicies;
using SFA.DAS.Notifications.Infrastructure.NotifyEmailService;

namespace SFA.DAS.Notifications.MessageHandlers.DependencyResolution
{
    public static class DefaultServices
    {
        public static IServiceCollection AddDefaultServices(this IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            services.AddTransient<IMediator, Mediator>();
            services.AddTransient<SingleInstanceFactory>(sp => t => sp.GetService(t));
            services.AddTransient<MultiInstanceFactory>(sp => t => sp.GetServices(t));
            //'Microsoft.Extensions.Logging.ILogger'
            //services.AddTransient<ILogger, Logger>()
            services.AddTransient<IAsyncRequestHandler<SendEmailCommand, Unit>, SendEmailCommandHandler>();
            services.AddTransient<ITemplateConfigurationService, TemplateConfigurationService>();
            services.AddTransient<IEmailService, NotifyEmailService>();
            services.AddTransient<INotifyHttpClientWrapper, NotifyHttpClientWrapper>();
            services.AddTransient<ExecutionPolicy, SendMessageExecutionPolicy>();

            return services;
        }
    }
}

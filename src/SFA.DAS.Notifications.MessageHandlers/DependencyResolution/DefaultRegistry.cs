using MediatR;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Configuration;
using SFA.DAS.Notifications.Infrastructure.Configuration;
using SFA.DAS.Notifications.Infrastructure.ExecutionPolicies;
using SFA.DAS.Notifications.Infrastructure.NotifyEmailService;
using StructureMap;

namespace SFA.DAS.Notifications.MessageHandlers.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            //Use Local Email Service with SMTP Configuration for Testing.
            For<IEmailService>().Use<NotifyEmailService>();
            For<ISmsService>().Use<NotifySmsService>();
            For<INotifyClientWrapper>().Use<NotifyClientWrapper>();
            For<ExecutionPolicy>().Use<SendMessageExecutionPolicy>();
        }

    }
}

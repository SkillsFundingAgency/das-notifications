using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Infrastructure.ExecutionPolicies;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public class NotifySmsService : ISmsService
    {
        private readonly INotifyClientWrapper _httpClientWrapper;
        private readonly ExecutionPolicy _executionPolicy;

        public NotifySmsService(
            INotifyClientWrapper httpClientWrapper,
            [RequiredPolicy(SendMessageExecutionPolicy.Name)]ExecutionPolicy executionPolicy)
        {
            if (httpClientWrapper == null)
                throw new ArgumentNullException(nameof(httpClientWrapper));

            _httpClientWrapper = httpClientWrapper;
            _executionPolicy = executionPolicy;
        }

        public Task SendAsync(SmsMessage message)
        {
            var notifyMessage = new NotifyMessage
            {
                To = message.RecipientsNumber,
                Template = message.TemplateId,
                Personalisation = (message.Tokens ?? new Dictionary<string, string>()).ToDictionary(item => item.Key.ToLower(), item => item.Value),
                SystemId = message.SystemId
            };

            return _executionPolicy.ExecuteAsync(() => _httpClientWrapper.SendSms(notifyMessage));
        }
    }
}

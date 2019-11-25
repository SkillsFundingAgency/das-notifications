using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Application2.Interfaces;
using SFA.DAS.Notifications.Domain2.Entities;
using SFA.DAS.Notifications.Infrastructure2.ExecutionPolicies;

namespace SFA.DAS.Notifications.Infrastructure2.NotifyEmailService
{
    public class NotifySmsService : ISmsService
    {
        private readonly INotifyHttpClientWrapper _httpClientWrapper;
        private readonly ExecutionPolicy _executionPolicy;

        public NotifySmsService(
            INotifyHttpClientWrapper httpClientWrapper,
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

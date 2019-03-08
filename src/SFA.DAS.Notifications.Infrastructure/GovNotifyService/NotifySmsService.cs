﻿using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Entities;

namespace SFA.DAS.Notifications.Infrastructure.GovNotifyService
{
    public class NotifySmsService : ISmsService
    {
        private readonly INotifyHttpClientWrapper _httpClientWrapper;

        public NotifySmsService(INotifyHttpClientWrapper httpClientWrapper)
        {
            if (httpClientWrapper == null)
                throw new ArgumentNullException(nameof(httpClientWrapper));

            _httpClientWrapper = httpClientWrapper;
        }

        public async Task SendAsync(SmsMessage message)
        {
            var notifyMessage = new NotifyEmailMessage
            {
                To = message.RecipientsNumber,
                Template = message.TemplateId,
                Personalisation = message.Tokens.ToDictionary(item => item.Key.ToLower(), item => item.Value)
            };

            await _httpClientWrapper.SendMessage(notifyMessage);
        }
    }
}

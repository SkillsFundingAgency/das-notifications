﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using SFA.DAS.Notifications.Application.Interfaces;
using SFA.DAS.Notifications.Domain.Entities;
using SFA.DAS.Notifications.Infrastructure.ExecutionPolicies;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public class NotifyEmailService : IEmailService
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly INotifyHttpClientWrapper _clientWrapper;
        private readonly ExecutionPolicy _executionPolicy;

        public NotifyEmailService(INotifyHttpClientWrapper clientWrapper, [RequiredPolicy(SendEmailExecutionPolicy.Name)]ExecutionPolicy executionPolicy)
        {
            if (clientWrapper == null)
                throw new ArgumentNullException(nameof(clientWrapper));
            _clientWrapper = clientWrapper;
            _executionPolicy = executionPolicy;
        }

        public async Task SendAsync(EmailMessage message)
        {
            Logger.Info($"Sending email to {message.RecipientsAddress}");

            var notifyMessage = new NotifyMessage
            {
                To = message.RecipientsAddress,
                Template = message.TemplateId,
                Personalisation = (message.Tokens ?? new Dictionary<string, string>()).ToDictionary(item => item.Key.ToLower(), item => item.Value),
                Reference = message.Reference
            };

            await _executionPolicy.ExecuteAsync(async () =>
            {
                await _clientWrapper.SendEmail(notifyMessage);
            });
            
        }
    }
}

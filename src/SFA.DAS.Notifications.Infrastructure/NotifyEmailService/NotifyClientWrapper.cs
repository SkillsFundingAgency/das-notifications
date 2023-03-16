﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NLog;
using Notify.Client;
using Notify.Exceptions;
using SFA.DAS.Notifications.Domain.Http;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public interface INotifyClientWrapper
    {
        Task SendEmail(NotifyMessage content);
        Task SendSms(NotifyMessage content);
    }

    public class NotifyClientWrapper : INotifyClientWrapper
    {
        private readonly NotificationServiceConfiguration _configuration;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public NotifyClientWrapper(NotificationServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmail(NotifyMessage content)
        {
            return SendMessage(content, CommunicationType.Email);
        }

        public Task SendSms(NotifyMessage content)
        {
            return SendMessage(content, CommunicationType.Sms);
        }

        private static JObject CreateRequestParams(string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null)
        {
            JObject value = new JObject();
            if (personalisation != null)
            {
                value = JObject.FromObject(personalisation);
            }

            JObject jObject = new JObject
            {
                { "template_id", templateId },
                { "personalisation", value }
            };
            if (clientReference != null)
            {
                jObject.Add("reference", clientReference);
            }

            return jObject;
        }

        private async Task SendMessage(NotifyMessage content, CommunicationType communicationType)
        {
            var notificationsClient = new NotificationClient(_configuration.NotificationServiceApiKey);

            // Needs to be a dictionary<string,dynamic> for the client.....
            var personalisationDictionary = content.Personalisation.ToDictionary(x => x.Key, x => x.Value as dynamic);

            foreach(var attachment in content.Attachments)
            {
                personalisationDictionary.Add(attachment.Key, NotificationClient.PrepareUpload(attachment.Value));
            }

            try
            {
                Logger.Info($"Sending communication request to Gov Notify");
                if (communicationType == CommunicationType.Email)
                {
                    var response = await notificationsClient.SendEmailAsync(content.To, content.Template, personalisationDictionary, content.Reference);
                }
                else if (communicationType == CommunicationType.Sms)
                {
                    var response = await notificationsClient.SendSmsAsync(content.To, content.Template, personalisationDictionary, content.Reference);
                }
            }
            catch (NotifyClientException notifyClientException)
            {
                Logger.Error(notifyClientException, $"Error sending communication {communicationType.ToString()} to Gov Notify with Gov.Notify Client");

                if (communicationType != CommunicationType.Sms || !SuppressSmsError(notifyClientException.Message))
                    throw;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Generic Error sending communication {communicationType.ToString()} to Gov Notify");
                throw;
            }
        }

        private bool SuppressSmsError(string message)
        {
            // raised when a non-valid uk mobile number is used or non-uk number with an invalid international prefix
            var invalidPhoneNumber = message.Contains("ValidationError");

            // raised when a non-uk international number is used and the GOVUK Notify account has been configured to not allow international numbers
            var internationalNumbersBarred = message.Contains("BadRequestError") && message.Contains("Cannot send to international mobile numbers");

            return invalidPhoneNumber || internationalNumbersBarred;
        }

        private enum CommunicationType
        {
            None,
            Email,
            Sms
        }
    }
}

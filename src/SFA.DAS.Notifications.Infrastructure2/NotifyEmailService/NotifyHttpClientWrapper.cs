﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Configuration;
using SFA.DAS.Notifications.Domain2.Http;
using SFA.DAS.Notifications.Infrastructure2.Configuration;

namespace SFA.DAS.Notifications.Infrastructure2.NotifyEmailService
{
    public interface INotifyHttpClientWrapper
    {
        Task SendEmail(NotifyMessage content);
        Task SendSms(NotifyMessage content);
    }

    public class NotifyHttpClientWrapper : INotifyHttpClientWrapper
    {
        private readonly IConfigurationService _configurationService;
        private readonly IDictionary<string, GovNotifyServiceCredentials> _consumerConfigurationLookup;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public NotifyHttpClientWrapper(IConfigurationService configurationService)
        {
            if (configurationService == null)
                throw new ArgumentNullException(nameof(configurationService));
            _configurationService = configurationService;
            _consumerConfigurationLookup = GetConsumerConfiguration();
        }

        public Task SendEmail(NotifyMessage content)
        {
            return SendMessage(content, "notifications/email");
        }

        public Task SendSms(NotifyMessage content)
        {
            return SendMessage(content, "notifications/sms");
        }
        
        private async Task SendMessage(NotifyMessage content, string notificationsEndPoint)
        {
            if (string.IsNullOrEmpty(notificationsEndPoint))
                throw new ArgumentNullException(nameof(notificationsEndPoint));
            if (notificationsEndPoint.StartsWith("/"))
                throw new ArgumentException("Cannot start with a /", nameof(notificationsEndPoint));

            var configuration = await _configurationService.GetAsync<NotificationServiceConfiguration>();
            content.Template = content.Template;

            using (var httpClient = CreateHttpClient(configuration.NotifyServiceConfiguration.ApiBaseUrl))
            {
                var serviceCredentials = GetServiceCredentials(configuration, content.SystemId);

                var token = JwtTokenUtility.CreateToken(serviceCredentials.ServiceId, serviceCredentials.ApiKey);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var serializeObject = JsonConvert.SerializeObject(content);
                var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");

                Logger.Info($"Sending communication request to Notify at {configuration.NotifyServiceConfiguration.ApiBaseUrl}/{notificationsEndPoint}");

                var request = new HttpRequestMessage(HttpMethod.Post, $"/{notificationsEndPoint}") {
                    Content = stringContent
                };
                var response = await httpClient.SendAsync(request);

                await EnsureSuccessfulResponse(response);
            }
        }
        
        private static HttpClient CreateHttpClient(string baseUrl)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        private GovNotifyServiceCredentials GetServiceCredentials(NotificationServiceConfiguration configuration, string systemId)
        {
            return _consumerConfigurationLookup.TryGetValue(systemId, out var serviceCredential)
                ? serviceCredential
                : new GovNotifyServiceCredentials(
                    configuration.NotifyServiceConfiguration.ServiceId,
                    configuration.NotifyServiceConfiguration.ApiKey);
        }

        private IDictionary<string, GovNotifyServiceCredentials> GetConsumerConfiguration()
        {
            var lookup = new Dictionary<string, GovNotifyServiceCredentials>();

            var consumerConfiguration = _configurationService
                .Get<NotificationServiceConfiguration>()
                .NotifyServiceConfiguration
                .ConsumerConfiguration;

            if (consumerConfiguration != null)
            {
                foreach (var config in consumerConfiguration)
                {
                    lookup.Add(config.ServiceName,
                        GovNotifyServiceCredentials.FromV2ApiKey(config.ApiKey));
                }
            }

            return lookup;
        }

        private async Task EnsureSuccessfulResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }
            switch ((int)response.StatusCode)
            {
                case 404:
                    throw new ResourceNotFoundException(response.RequestMessage.RequestUri.ToString());
                case 429:
                    throw new TooManyRequestsException();
                case 500:
                    throw new InternalServerErrorException();
                case 503:
                    throw new ServiceUnavailableException();
                default:
                    string responseContent = await response.Content.ReadAsStringAsync();
                    throw new HttpException((int)response.StatusCode, $"Unexpected HTTP exception - ({(int)response.StatusCode}): {response.ReasonPhrase})\r\n{responseContent}");
            }
        }
    }
}

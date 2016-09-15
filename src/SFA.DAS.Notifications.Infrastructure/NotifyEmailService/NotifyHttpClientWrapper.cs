﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Configuration;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public interface INotifyHttpClientWrapper
    {
        Task SendMessage(NotifyMessage content);
    }

    public class NotifyHttpClientWrapper : INotifyHttpClientWrapper
    {
        private readonly IConfigurationService _configurationService;

        public NotifyHttpClientWrapper(IConfigurationService configurationService)
        {
            if (configurationService == null)
                throw new ArgumentNullException(nameof(configurationService));
            _configurationService = configurationService;
        }

        public async Task SendMessage(NotifyMessage content)
        {
            var configuration = await _configurationService.GetAsync<NotificationServiceConfiguration>();

            content.Template = content.Template;

            using (var httpClient = CreateHttpClient(configuration.NotifyServiceConfiguration.ApiBaseUrl))
            {
                var token = JwtTokenUtility.CreateToken(configuration.NotifyServiceConfiguration.ServiceId, configuration.NotifyServiceConfiguration.ApiKey);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var serializeObject = JsonConvert.SerializeObject(content);
                var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/notifications/email")
                {
                    Content = stringContent
                });

                response.EnsureSuccessStatusCode();
            }
        }

        private HttpClient CreateHttpClient(string baseUrl)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Notifications.Domain.Http;
using SFA.DAS.Notifications.Infrastructure.Configuration;

namespace SFA.DAS.Notifications.Infrastructure.NotifyEmailService
{
    public interface INotifyHttpClientWrapper
    {
        Task SendEmail(NotifyMessage content);
        Task SendSms(NotifyMessage content);
    }

    public class NotifyHttpClientWrapper : INotifyHttpClientWrapper
    {
        private readonly NotifyServiceConfiguration _configuration;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public NotifyHttpClientWrapper(NotifyServiceConfiguration configuration)
        {
            _configuration = configuration;
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

            content.Template = content.Template;

            using (var httpClient = CreateHttpClient(_configuration.ApiBaseUrl))
            {
                var serviceCredentials = new GovNotifyServiceCredentials(_configuration.ServiceId, _configuration.ApiKey);
                var token = JwtTokenUtility.CreateToken(serviceCredentials.ServiceId, serviceCredentials.ApiKey);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var serializeObject = JsonConvert.SerializeObject(content);
                var stringContent = new StringContent(serializeObject, Encoding.UTF8, "application/json");

                Logger.Info($"Sending communication request to Notify at {_configuration.ApiBaseUrl}/{notificationsEndPoint}");

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

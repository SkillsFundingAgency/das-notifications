using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Configuration;
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
        private readonly IConfigurationService _configurationService;
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private const int ServiceIdStartPosition = 73;
        private const int ServiceApiKeyStartPosition = 36;
        private const int GuidLength = 36;

        public NotifyHttpClientWrapper(IConfigurationService configurationService)
        {
            if (configurationService == null)
                throw new ArgumentNullException(nameof(configurationService));
            _configurationService = configurationService;
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
                var serviceId = serviceCredentials.Item1;
                var apiKey = serviceCredentials.Item2;

                var token = JwtTokenUtility.CreateToken(serviceId, apiKey);
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

        private static Tuple<string, string> GetServiceCredentials(NotificationServiceConfiguration configuration, string systemId)
        {
            var service =
                configuration.NotifyServiceConfiguration.ConsumerConfiguration?.FirstOrDefault(s =>
                    s.ServiceName == systemId);

            if (service != null)
            {
                return ExtractServiceIdAndApiKey(service.ApiKey);
            }
            else
            {
                return Tuple.Create(configuration.NotifyServiceConfiguration.ServiceId,
                    configuration.NotifyServiceConfiguration.ApiKey);
            }
        }

        private static Tuple<string, string> ExtractServiceIdAndApiKey(string fromApiKey)
        {
            if (fromApiKey.Length < 74)
            {
                throw new ConfigurationErrorsException("The API Key provided is invalid. Please ensure you are using a v2 API Key that is not empty or null");
            }

            var serviceId = fromApiKey.Substring(fromApiKey.Length - ServiceIdStartPosition, GuidLength);
            var apiKey = fromApiKey.Substring(fromApiKey.Length - ServiceApiKeyStartPosition, GuidLength);

            return Tuple.Create(serviceId, apiKey);
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

using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.Notifications.Api.Client
{
    public class NotificationsApi : INotificationsApi
    {
        private readonly INotificationsApiClientConfiguration _configuration;
        private readonly SecureHttpClient _httpClient;

        public NotificationsApi(INotificationsApiClientConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new SecureHttpClient(configuration);
        }

        internal NotificationsApi(INotificationsApiClientConfiguration configuration, SecureHttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task SendEmail(Email email)
        {
            var baseUrl = _configuration.ApiBaseUrl.EndsWith("/")
                ? _configuration.ApiBaseUrl
                : _configuration.ApiBaseUrl + "/";

            var url = $"{baseUrl}api/email";

            await _httpClient.PostAsync(url, email);
        }
        
    }
}
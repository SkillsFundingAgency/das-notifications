using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Http;
using System.Net.Http;

namespace SFA.DAS.Notifications.Api.Client
{
    public class NotificationsApi : ApiClientBase, INotificationsApi
    {
        private readonly INotificationsApiClientConfiguration _configuration;

        public NotificationsApi(HttpClient client, INotificationsApiClientConfiguration configuration) : base(client)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(Email email)
        {
            var baseUrl = _configuration.ApiBaseUrl.EndsWith("/")
                ? _configuration.ApiBaseUrl
                : _configuration.ApiBaseUrl + "/";

            var url = $"{baseUrl}api/email";

            await base.PostAsync(url, JsonConvert.SerializeObject(email));
        }
    }
}
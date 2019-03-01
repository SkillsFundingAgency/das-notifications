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
        private readonly string ApiBaseUrl;

        public NotificationsApi(HttpClient client, INotificationsApiClientConfiguration configuration) : base(client)
        {
            ApiBaseUrl = configuration.ApiBaseUrl.EndsWith("/")
                ? configuration.ApiBaseUrl
                : configuration.ApiBaseUrl + "/";
        }

        public Task SendEmail(Email email)
        {
            var url = $"{ApiBaseUrl}api/email";
            return PostAsync(url, JsonConvert.SerializeObject(email));
        }

        public Task SendSms(Sms sms)
        {
            string url = $"{ApiBaseUrl}api/sms";
            return PostAsync(url, JsonConvert.SerializeObject(sms));
        }
    }
}
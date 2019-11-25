using Newtonsoft.Json;

namespace SFA.DAS.Notifications.Infrastructure2.NotifyEmailService
{
    public class GovNotifyPayload
    {
        [JsonProperty("iss")]
        public string Iss { get; set; }

        [JsonProperty("iat")]
        public long Iat { get; set; }
    }
}

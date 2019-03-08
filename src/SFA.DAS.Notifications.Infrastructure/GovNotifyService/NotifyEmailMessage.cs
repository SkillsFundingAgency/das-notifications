using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.Notifications.Infrastructure.GovNotifyService
{
    public class NotifyEmailMessage
    {
        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }
        [JsonProperty(PropertyName = "template")]
        public string Template { get; set; }
        [JsonProperty(PropertyName = "personalisation")]
        public Dictionary<string, string> Personalisation { get; set; }
        [JsonProperty(PropertyName = "reference")]
        public object Reference { get; set; }
    }
}